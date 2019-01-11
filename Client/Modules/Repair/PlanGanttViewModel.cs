using GazRouter.Common.Ui.Behaviors;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.Repair.Dialogs;
using GazRouter.Repair.Gantt;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Controls.Scheduling;
using Telerik.Windows.Media.Imaging;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.Export;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GazRouter.Repair
{
    public class PlanGanttViewModel : ViewModelBase, ITabItem
    {
        private readonly RepairMainViewModel _mainViewModel;
        private GanttRepairTask _selectedGanttRepair;
        private bool _showGantt = true;
        private VisibleRange _visibleRange;

        public PlanGanttViewModel([NotNull] RepairMainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            DatesEditedCommand = new Telerik.Windows.Controls.DelegateCommand(OnTaskEdited);
            ExportPDFCommand = new DelegateCommand<FrameworkElement>(ExportToPDF);
        }

        public DelegateCommand<FrameworkElement> ExportPDFCommand { get; set; }

        public void ExportToPDF(FrameworkElement content)
        {
            var ganttView = FindChild(content, x => x is Telerik.Windows.Controls.RadGanttView) as Telerik.Windows.Controls.RadGanttView;
            var dlg = new SaveFileDialog() { Filter = "Pdf files (*.pdf)|*.pdf|All files (*.*)|*.*", FilterIndex = 1, /*DefaultFileName = Header*/ };
            if (dlg.ShowDialog() == true)
            {
                var stream = dlg.OpenFile();
                _mainViewModel.Lock("Экспорт в PDF...");
                var bw = new System.ComponentModel.BackgroundWorker();
                bw.DoWork += (s,e) => {
                    System.Threading.Thread.Sleep(1000);
                    ganttView.Dispatcher.BeginInvoke(() =>  { 
                    try
                    {
                        using (stream)
                        {
                            var document = new RadFixedDocument();
                            var pageSize = new Size(1056, 1488.58);//new Size(796.8, 1123.2)
                            using (var export = ganttView.ExportingService.BeginExporting(new ImageExportSettings(pageSize, true, GanttArea.AllAreas)))
                            {
                                foreach (var img in export.ImageInfos.Select(info => info.Export()))
                                {
                                    var page = new RadFixedPage();
                                    page.Size = pageSize;
                                    page.Content.AddImage(new Telerik.Windows.Documents.Fixed.Model.Resources.ImageSource(img, ImageQuality.High));
                                    /*using (var ms = new System.IO.MemoryStream())
                                    {
                                        var encoder = new PngBitmapEncoder();
                                        encoder.Frames.Add(BitmapFrame.Create(img));
                                        encoder.Save(ms);
                                        page.Content.AddImage(new Telerik.Windows.Documents.Fixed.Model.Resources.ImageSource(ms, ImageQuality.High));
                                    }*/
                                    document.Pages.Add(page);
                                }
                            }
                            new PdfFormatProvider().Export(document, stream);//ExportSettings.ImageQuality = ImageQuality.High;
                            stream.Flush();
                        }
                    }
                    finally
                    {
                        _mainViewModel.Unlock();
                    }});
              };
              bw.RunWorkerAsync();  
            }
        }

        private static DependencyObject FindChild(DependencyObject dObject, Func<DependencyObject, bool> predicate)
        {
            var fElement = dObject as FrameworkElement;
            if (fElement != null)
            {
                int cCount = VisualTreeHelper.GetChildrenCount(fElement);
                for (int i = 0; i < cCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(fElement, i);
                    if (predicate(child)) return child;
                    var v = FindChild(child, predicate);
                    if (v != null) return v;
                }
            }
            return null;
        }

        public string Header => "Гант";

        public Telerik.Windows.Controls.DelegateCommand DatesEditedCommand { get; private set; }

        /// <summary>
        ///     Список ремонтов для отображения на Ганте
        /// </summary>
        public ObservableCollection<GanttRepairTask> GanttRepairList { get; } = new ObservableCollection<GanttRepairTask>();

        public TimeSpan PixelLength => TimeSpan.FromTicks(47000000000);

        /// <summary>
        ///     Выбранный ремонт на диаграмме ганта
        /// </summary>
        public GanttRepairTask SelectedGanttRepair
        {
            get { return _selectedGanttRepair; }
            set
            {
                if (!SetProperty(ref _selectedGanttRepair, value))
                {
                    return;
                }

                if (value.RepairItem != null)
                {
                    _mainViewModel.SelectedRepair = value.RepairItem;
                }
            }
        }

        /// <summary>
        ///     Отображать диаграмму ганта
        /// </summary>
        public bool ShowGantt
        {
            get { return _showGantt; }
            set { SetProperty(ref _showGantt, value); }
        }

        /// <summary>
        ///     Диапазон времени отображаемый на диаграмме ганта
        /// </summary>
        public VisibleRange VisibleRange
        {
            get { return _visibleRange; }
            set { SetProperty(ref _visibleRange, value); }
        }

        public bool ChangesAllowed => _mainViewModel.ChangesAllowed;

        public void Activate()
        {
        }

        public void Deactivate()
        {
        }

        public void Refresh(List<RepairItem> repairs)
        {
            GanttRepairList.Clear();
            var groups = repairs.Select(r => r.GroupObject).Distinct();
            foreach (var group in groups)
            {
                var groupRepaiItems = repairs.Where(r => r.GroupObject == @group).ToList();
                var groupItem = new GanttRepairTask(
                    groupRepaiItems.Min(r => r.StartDatePlan),
                    groupRepaiItems.Max(r => r.EndDatePlan),
                    group);
                foreach (var r in groupRepaiItems)
                {
                    groupItem.Children.Add(new GanttRepairTask(r));
                }
                GanttRepairList.Add(groupItem);
            }
        }

        private void OnTaskEdited(object obj)
        {
            var cp = (TaskEditedEventArgs) obj;
            var gt = (GanttRepairTask) cp.Task;
            if (gt.RepairItem != null &&
                (gt.RepairItem.StartDatePlan != gt.Start || gt.RepairItem.EndDatePlan != gt.End))
            {
                // Если ремонт включен в комплекс и после изменения даты ремонта не соответсвуют датам комплекса  
                if (gt.RepairItem.ComplexId > 0 &&
                    (gt.Start < gt.RepairItem.Dto.Complex.StartDate || gt.End > gt.RepairItem.Dto.Complex.EndDate) &&
                    gt.RepairItem.StartDatePlan >= gt.RepairItem.Dto.Complex.StartDate &&
                    gt.RepairItem.EndDatePlan <= gt.RepairItem.Dto.Complex.EndDate)
                {
                    var vm = new QuestionViewModel(num =>
                    {
                        switch (num)
                        {
                            case 1:
                                _mainViewModel.UpdateRepairDates(gt.RepairItem, gt.Start, gt.End);
                                break;

                            case 2:
                                _mainViewModel.UpdateRepairDates(gt.RepairItem, gt.Start, gt.End);
                                // Исключить работу из комплекса
                                _mainViewModel.AddRepairToComplex(gt.RepairItem, null);
                                break;

                            case 3:
                                _mainViewModel.EditRepairDates(new EditRepairDatesParameterSet
                                {
                                    DateStart = gt.Start,
                                    DateEnd = gt.End,
                                    DateType = DateTypes.Plan,
                                    RepairId = gt.RepairItem.Id
                                }, () =>
                                {
                                    var shift = gt.Start - gt.RepairItem.StartDatePlan;

                                    if (gt.End - gt.Start ==
                                        gt.RepairItem.EndDatePlan - gt.RepairItem.StartDatePlan)
                                    {
                                        // Если работа просто передвинута
                                        // Сдвигаем комплекс и все работы внутри, на тоже значение
                                        _mainViewModel.MoveComplex(gt.RepairItem.Dto.Complex,
                                            gt.RepairItem.Dto.Complex.StartDate + shift,
                                            gt.RepairItem.Dto.Complex.EndDate + shift);
                                    }
                                    else
                                    {
                                        //Если у работы увеличилась продолжительность
                                        if (gt.End - gt.Start >
                                            gt.RepairItem.EndDatePlan - gt.RepairItem.StartDatePlan)
                                        {
                                            // Просто увеличиваем продолжительность комплекса
                                            _mainViewModel.MoveComplex(gt.RepairItem.Dto.Complex,
                                                gt.Start < gt.RepairItem.Dto.Complex.StartDate
                                                    ? gt.Start
                                                    : gt.RepairItem.Dto.Complex.StartDate,
                                                gt.End > gt.RepairItem.Dto.Complex.EndDate
                                                    ? gt.End
                                                    : gt.RepairItem.Dto.Complex.EndDate);
                                        }
                                    }
                                },
                                    Behavior);

                                break;

                            default:
                                gt.Start = gt.RepairItem.Dto.StartDate;
                                gt.End = gt.RepairItem.Dto.EndDate;
                                break;
                        }
                    })
                    {
                        Header = "Дата ремонта не соответствуют дате комплекса",
                        Question =
                            "Новые даты проведения ремонта не соответствуют датам проведения комплекса. Что делать?",
                        AnswerList = new List<Answer>
                        {
                            new Answer {Num = 1, Text = "Только передвинуть"},
                            new Answer {Num = 2, Text = "Передвинуть и исключить из комплекса"},
                            new Answer
                            {
                                Num = 3,
                                Text = "Передвинуть ремонт вместе с комплексом (включая все добавленные ремонты)"
                            },
                            new Answer {Num = 4, Text = "Ничего не делать"}
                        }
                    };
                    var v = new QuestionView {DataContext = vm};
                    v.ShowDialog();
                }
                else
                {
                    _mainViewModel.UpdateRepairDates(gt.RepairItem, gt.Start, gt.End);
                }
            }
        }
    }
}