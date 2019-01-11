using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using GazRouter.Common;
using GazRouter.Common.Ui.Behaviors;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Appearance.Versions;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.VM.Model;
using GazRouter.Repair.Converters;
using Telerik.Windows.Media.Imaging;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.Flobus.Primitives;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using System.Collections.Generic;
using GazRouter.Flobus.VM;
using JetBrains.Annotations;
using System.Collections.ObjectModel;

namespace GazRouter.Repair.Plan
{
    public class PlanSchemeViewModel : FloControlViewModelBase, ITabItem
    {
        public PlanViewModel MainViewModel { get; set; }

        public PlanSchemeViewModel([NotNull] PlanViewModel repairMainViewModel)
        {
            MainViewModel = repairMainViewModel;

            IsEditPermission = Authorization2.Inst.IsEditable(LinkType.RepairPlan);

            MainViewModel.PropertyChanged += MainViewModelOnPropertyChanged;

            MonthColorList.ColorChanged += MonthColorListOnColorChanged;

            ExportPDFCommand = new DelegateCommand<FrameworkElement>(ExportSchemeToPDF/*, x => !IsBusyLoading*/);

            LoadSchemeVersionList();
        }

        private void Schema_DialogWindowCall(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(Guid))
                new GazRouter.Repair.ReqWorks.Dialogs.AddEditRepairView { DataContext = new GazRouter.Repair.ReqWorks.Dialogs.AddEditRepairViewModel(RedrawRepairList, (Guid)sender, DateTime.Now.Year) }.ShowDialog();
        }    

        private void RedrawRepairList(int obj)
        {
            MainViewModel.LoadData();
            RedrawRepairList  ();
        }

        private readonly StartDateToColorConverter _startDateToColorConverter = new StartDateToColorConverter();

        private bool _activated;

        public DelegateCommand<FrameworkElement> ExportPDFCommand { get; set; }

        private bool _isEditPermission;
        public bool IsEditPermission
        {
            get { return _isEditPermission; }
            set
            {
                _isEditPermission = value;
                OnPropertyChanged(() => IsEditPermission);
            }
        }


        private SchemeVersionItemDTO _selectedSchemeVersion;
        private List<SchemeVersionItemDTO> _schemeVersionList = new List<SchemeVersionItemDTO>();
        /// <summary>
        ///     Список версий схем, доступный для выбора
        /// </summary>
        public List<SchemeVersionItemDTO> SchemeVersionList
        {
            get { return _schemeVersionList; }
            set {
                SetProperty(ref _schemeVersionList, value);
            }
        }

        /// <summary>
        ///     Выбранная версия схемы
        /// </summary>
        public SchemeVersionItemDTO SelectedSchemeVersion
        {
            get { return _selectedSchemeVersion; }
            set
            {
                if (SetProperty(ref _selectedSchemeVersion, value))
                {
                    if (_selectedSchemeVersion != null)
                    {
                        LoadModel(_selectedSchemeVersion.Id);
                    }
                }
            }
        }
        private async void LoadSchemeVersionList()
        {
            try
            {
                Behavior.TryLock("Загрузка версий схем");
                SchemeVersionList = await new SchemeServiceProxy().GetPublishedSchemeVersionListAsync();
            }
            finally
            {
                Behavior.TryUnlock();
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
        void ExportSchemeToPDF(FrameworkElement content)
        {
            if (IsBusyLoading || !IsModelLoaded)
            {
                MessageBoxProvider.Alert("Схема ещё не загружена!", "Внимание");
                return;
            }
            var schema = FindChild(content, x => x is Flobus.Schema) as Flobus.Schema;
            var dlg = new SaveFileDialog() { Filter = "Pdf files (*.pdf)|*.pdf|All files (*.*)|*.*", FilterIndex = 1, /*DefaultFileName = Header*/ };
            if (dlg.ShowDialog() == true)
            {
                var stream = dlg.OpenFile();
                Behavior.TryLock("Экспорт в PDF...");
                OnPropertyChanged(() => IsBusyLoading);
                var bw = new System.ComponentModel.BackgroundWorker();
                bw.DoWork += (s, e) =>
                {
                    System.Threading.Thread.Sleep(1000);
                    content.Dispatcher.BeginInvoke(() => {
                        try
                        {
                            using (stream)
                            {
                                var document = new RadFixedDocument();
                                var pageSize = new Size(1056, 1488.58);//new Size(796.8, 1123.2)

                                var rect = schema.CalculateEnclosingBounds();
                                var magicK = 1.0;

                                for (var y = Math.Max(rect.Top - 100, 0); y <= rect.Bottom; y += pageSize.Height * magicK)
                                {
                                    for (var x = Math.Max(rect.Left - 100, 0); x <= rect.Right; x += pageSize.Width * magicK)
                                    {

                                        var page = new RadFixedPage();
                                        var vvv = schema.CreateDiagramImage(new Rect(x, y, pageSize.Width * magicK, pageSize.Height * magicK), pageSize);
                                        var source = new Telerik.Windows.Documents.Fixed.Model.Resources.ImageSource(vvv);
                                        page.Size = new Size(source.Width, source.Height);
                                        new FixedContentEditor(page).DrawImage(source);
                                        document.Pages.Add(page);
                                    }
                                }
                                new PdfFormatProvider().Export(document, stream);//ExportSettings.ImageQuality = ImageQuality.High;
                                stream.Flush();
                            }
                        }
                        finally
                        {
                            Behavior.TryUnlock();
                            OnPropertyChanged(() => IsBusyLoading);
                        }
                    });
                };
                bw.RunWorkerAsync();
            }
        }
        public string Header => "Схема";
        public MonthToColorList MonthColorList { get; set; } = new MonthToColorList();

        public void Activate()
        {            
            if (_activated)
            {
                return;
            }
            _activated = true;           
            LoadSchemeVersions();
        }

        public void Deactivate()
        {            
        }

        public void LoadRepairScheme()
        {

        }

        protected override void CloseLoadSchemeFormCallback(SchemeVersionItemDTO schemeDTO)
        {
            LoadModel(schemeDTO.Id);
            //ExportPDFCommand.RaiseCanExecuteChanged();
        }

        protected override void AfterModelLoaded(SchemeViewModel viewModel)
        {
            FillRepairs(viewModel);
            GazRouter.Flobus.Schema.IsRepair = true;
            GazRouter.Flobus.Schema.DialogWindowCall += Schema_DialogWindowCall;
            //ExportPDFCommand.RaiseCanExecuteChanged();
        }

        private static void ClearRepairs(ISchemaSource schemasource)
        {
            foreach (var pipeline in schemasource.Pipelines)
            {
                pipeline.Markups.Clear();
                foreach (var distStation in pipeline.DistributingStations)
                {
                    distStation.Data = null;
                }
            }

            foreach (var compressorShop in schemasource.CompressorShops)
            {
                compressorShop.Data = null;
            }
        }

        private void RedrawRepairList()
        {
            if (Model == null)
            {
                return;
            }

            ClearRepairs(Model);

            foreach (var newItem in MainViewModel.RepairList)
            {
                AddRepairMarkup(Model, (Repair)newItem);
            }
        }

        private void RepairListOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (Model == null)
            {
                return;
            }

           
            if (args.NewItems != null)
            {
                foreach (var newItem in args.NewItems)
                {
                    AddRepairMarkup(Model, (Repair)newItem);
                }
            }
            if (args.OldItems != null)
            {
                foreach (var item in args.OldItems)
                {
                    RemoveRepairMarkup((Repair)item);
                }
            }

            if (args.Action == NotifyCollectionChangedAction.Reset && Model != null)
            {
                ClearRepairs(Model);
            }
            
        }

        private void MonthColorListOnColorChanged(object sender, EventArgs eventArgs)
        {
            foreach (var item in MainViewModel.RepairList)
            {
                item.Dto.StartDate = item.Dto.StartDate;
            }

        }

        private void FillRepairs(SchemeViewModel viewModel)
        {
            ClearRepairs(viewModel);

            foreach (var repairItem in MainViewModel.RepairList)
            {
                AddRepairMarkup(viewModel, repairItem);
            }            
        }

        private void AddRepairMarkup(SchemeViewModel viewModel, Repair repair)
        {
            if (viewModel == null)
            {
                return;
            }
            ISchemaSource schemaSource = viewModel;
            //try
            //{
            switch (repair.Dto.EntityType)
            {

                case EntityType.Pipeline:
                    foreach (var work in repair.Dto.Works)
                    {
                        var tooltip =
                            $"{repair.Dto.StartDate:d} - {repair.Dto.EndDate:d}\r\n{repair.Dto.Description}";
                        var pipeline = schemaSource.Pipelines.FirstOrDefault(p => p.Id == repair.Dto.EntityId);
                        pipeline?.Markups.Add(new PipelineMarkup
                        {
                            StartKm = work.KilometerStart ?? 0,
                            EndKm = work.KilometerEnd ?? 0,
                            Data = work,
                            Color = _startDateToColorConverter.MonthToColor(repair.Dto.StartDate.Month),
                            Tooltip = tooltip
                        });
                    }
                    break;
                case EntityType.CompShop:
                    var cs = schemaSource.CompressorShops.FirstOrDefault(c => c.Id == repair.Dto.EntityId);
                    if (cs != null)
                        cs.Data = repair;
                    break;
                case EntityType.DistrStation:
                    var firstOrDefault =
                        schemaSource.Pipelines.SelectMany(c => c.DistributingStations)
                            .FirstOrDefault(ds => ds.Id == repair.Dto.EntityId);
                    if (firstOrDefault != null)
                    {
                        firstOrDefault.Data = repair;
                    }
                    break;

            }
            //}
            //catch { }
        } 

        private void RemoveRepairMarkup(Repair repair)
        {
            ISchemaSource schemaSource = Model;
            switch (repair.Dto.EntityType)
            {
                case EntityType.Pipeline:
                    var pipeline = schemaSource.Pipelines.FirstOrDefault(p => p.Id == repair.Dto.EntityId);
                    if (pipeline != null)
                    {
                        foreach (var work in repair.Dto.Works)
                        {
                            pipeline.Markups.Remove(pipeline.Markups.First(m => m.Data == work));
                        }
                    }
                    break;

                case EntityType.CompShop:
                    schemaSource.CompressorShops.FirstOrDefault(c => c.Id == repair.Dto.EntityId).Data = null;
                    break;
                case EntityType.DistrStation:
                    var firstOrDefault =
                        schemaSource.Pipelines.SelectMany(c => c.DistributingStations)
                            .FirstOrDefault(ds => ds.Id == repair.Dto.EntityId);
                    if (firstOrDefault != null)
                    {
                        firstOrDefault.Data = null;
                    }
                    break;
            }
        }

        private void MainViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (Model == null)
            {
                return;
            }
            switch (args.PropertyName)
            {
                case nameof(MainViewModel.SelectedSystem):
                    Model = null;
                    LoadSchemeVersions();
                    break;
                /*    case nameof(MainViewModel.SelectedYear):
                    FillRepairs(Model);
                    break;*/
                case nameof(MainViewModel.RepairList):
                    RedrawRepairList();
                    break;
            }
        }

        private async void LoadSchemeVersions()
        {
            var list = await new SchemeServiceProxy().GetPublishedSchemeVersionListAsync();
            var version =
                list.OrderByDescending(c => c.Id).FirstOrDefault(c => c.SystemId == MainViewModel.SelectedSystem.Id);
            if (version != null)
            {
                LoadModel(version.Id);
            }
            //ExportPDFCommand.RaiseCanExecuteChanged();
        }
    }
}