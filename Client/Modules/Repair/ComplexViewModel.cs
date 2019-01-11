using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GazRouter.Common.Ui.Behaviors;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.Repair.Dialogs;
using JetBrains.Annotations;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using ViewModelBase = GazRouter.Common.ViewModel.ViewModelBase;
using GazRouter.Common;
using System;
using GazRouter.Application;

namespace GazRouter.Repair
{
    public class ComplexViewModel : ViewModelBase, ITabItem
    {
        private readonly RepairMainViewModel _mainViewModel;
        private List<ComplexItem> _complexList;
        private ComplexItem _selectedComplex;

        public ComplexViewModel([NotNull] RepairMainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            AddComplexCommand = new DelegateCommand(AddComplex, () => ChangesAllowed && _mainViewModel.IsEditPermission);
            EditComplexCommand = new DelegateCommand(EditComplex,
                () => ChangesAllowed && SelectedComplex != null && SelectedComplex.IsLocal && _mainViewModel.IsEditPermission);
            DeleteComplexCommand = new DelegateCommand(DeleteComplex,
                () => ChangesAllowed && SelectedComplex != null && SelectedComplex.IsLocal && _mainViewModel.IsEditPermission);
            ExportExcelCommand = new DelegateCommand(ExportToExcel, () => LocalComplexList.Count > 0 && EnterpriseComplexList.Count > 0/*true*/);
        }

        public string Header => "Комплексы";
        public List<ComplexItem> ComplexList => _complexList;

        /// <summary>
        /// Список комплексов ПАО "Газпром"
        /// </summary>
        public List<ComplexItem> EnterpriseComplexList
        {
            get { return _complexList.Where(c => !c.IsLocal).ToList(); }
        }

        /// <summary>
        ///     Список локальных комплексов
        /// </summary>
        public List<ComplexItem> LocalComplexList
        {
            get { return _complexList.Where(c => c.IsLocal).ToList(); }
        }

        /// <summary>
        ///     Создать новый локальный комплекс
        /// </summary>
        public DelegateCommand AddComplexCommand { get; private set; }

        /// <summary>
        ///     Изменить данные по новому локальному комплексу
        /// </summary>
        public DelegateCommand EditComplexCommand { get; }

        /// <summary>
        ///     Удалить локальный комплекс
        /// </summary>
        public DelegateCommand DeleteComplexCommand { get; }
        
        /// <summary>
        ///     Экспорт в Ms Excel
        /// </summary>
        public DelegateCommand ExportExcelCommand { get; }

        ExcelReport excelReport;  DateTime date;

        public bool ChangesAllowed => _mainViewModel.ChangesAllowed;

        public GasTransportSystemDTO SelectedSystem => _mainViewModel.SelectedSystem;

        public int SelectedYear => _mainViewModel.SelectedYear;

        /// <summary>
        ///     Выбранный комплекс
        /// </summary>
        public ComplexItem SelectedComplex
        {
            get { return _selectedComplex; }
            set
            {
                SetProperty(ref _selectedComplex, value);
                OnPropertyChanged(() => ComplexRepairList);
                RefreshCommands();
            }
        }

        /// <summary>
        ///     Список ремонтов входящих в выбранный комплекс
        /// </summary>
        public List<RepairItem> ComplexRepairList
        {
            get
            {
                return _mainViewModel.RepairList != null && SelectedComplex != null
                    ? _mainViewModel.RepairList.Where(r => r.ComplexId == SelectedComplex.Id).ToList()
                    : null;
            }
        }

        public ICommand RemoveFromComplexCommand => _mainViewModel.RemoveFromComplexCommand;

        public ICommand RefreshCommand => _mainViewModel.RefreshCommand;

        public void Activate()
        {
        }

        public void Deactivate()
        {
        }

        public ComplexItem GetComplexById(int complexId)
        {
            return _complexList.Single(c => c.Id == complexId);
        }

        public void RefreshCommands()
        {
            EditComplexCommand.RaiseCanExecuteChanged();
            DeleteComplexCommand.RaiseCanExecuteChanged();
        }

        public void RefreshComplexes(List<ComplexDTO> data)
        {
            var currentComplex = SelectedComplex;
            _complexList = data.Select(c => new ComplexItem(c, this._mainViewModel)).ToList();
            OnPropertyChanged(() => EnterpriseComplexList);
            OnPropertyChanged(() => LocalComplexList);
            OnPropertyChanged(() => ComplexList);
            SelectedComplex = currentComplex != null && _complexList.Count > 0
                ? _complexList.SingleOrDefault(c => c.Id == currentComplex.Id)
                : null;
            CheckComplexes();
            ExportExcelCommand.RaiseCanExecuteChanged();
        }

        public void SelectComplexById(int? complexId)
        {
            if (!complexId.HasValue)
            {
                SelectedComplex = null;
            }
            else if (SelectedComplex == null ||
                     SelectedComplex.Id != complexId)
            {
                SelectedComplex = _complexList.Single(c => c.Id == complexId);
            }
        }

        /// <summary>
        ///     Проверка ремонтов включенных в комплекс
        ///     Ремонты входящие в состав комплекса, должны выполняться в период проведения комплекса,
        ///     т.е. дата начала и окончания работы внутри комплекса, должны лежать в диапазоне,
        ///     определенном датами комплекса
        /// </summary>
        public void CheckComplexes()
        {
            if (_mainViewModel.RepairList == null || _complexList == null)
            {
                return;
            }
            _complexList.ForEach(
                c => c.HasErrors = _mainViewModel.RepairList.Any(r => r.ComplexId == c.Id && r.HasComplexError));
        }

        private void AddComplex()
        {
            var viewModel = new AddEditComplexViewModel(id => _mainViewModel.Refresh(), SelectedYear, SelectedSystem.Id);
            var view = new AddEditComplexView { DataContext = viewModel };
            view.ShowDialog();
        }

        private void DeleteComplex()
        {
            var msg = _mainViewModel.RepairList.Any(r => r.ComplexId == SelectedComplex.Id)
                ? "Вы пытаетесь удалить комплекс, в котором есть работы. При удалении такого комплекса все работы, входящие в него, будут исключены из этого комплекса. "
                : string.Empty;
            msg += "Вы уверены что хотите удалить комплекс?";
            RadWindow.Confirm(
                new DialogParameters
                {
                    Closed = async (s, e) =>
                    {
                        if (e.DialogResult.HasValue && e.DialogResult.Value)
                        {
                            Behavior.TryLock();
                            try
                            {
                                await new RepairsServiceProxy().DeleteComplexAsync(SelectedComplex.Id);

                            }
                            finally
                            {
                                Behavior.TryUnlock();
                            }
                            _mainViewModel.Refresh();

                        }
                    },
                    Content = new TextBlock
                    {
                        Text = msg,
                        TextWrapping = TextWrapping.Wrap,
                        Width = 250
                    },
                    Header = "Удаление комплекса",
                    OkButtonContent = "Да",
                    CancelButtonContent = "Нет"
                });
        }

        private void EditComplex()
        {
            var viewModel = new AddEditComplexViewModel(id => _mainViewModel.Refresh(), SelectedComplex.Dto,
                SelectedYear)
            {
                HasRepairs = _mainViewModel.RepairList.Any(r => r.ComplexId == SelectedComplex.Id)
            };
            var view = new AddEditComplexView { DataContext = viewModel };
            view.ShowDialog();
        }

        public void ExportToExcel()
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FilterIndex = 1,
                //DefaultFileName = Header
            };
            if (dialog.ShowDialog() == true)
            {
                excelReport = new ExcelReport("ЛОКАЛЬНЫЕ КОМПЛЕКСЫ");
                complexesToExcel(LocalComplexList);
                excelReport.Move(0, 0, "КОМПЛЕКСЫ ПАО ГАЗПРОМ");
                complexesToExcel(EnterpriseComplexList);
                using (var stream = dialog.OpenFile())
                {
                    excelReport.Save(stream);
                }
            }
        }
        void complexesToExcel(List<ComplexItem> complexes)
        {
            date = DateTime.Now;
            excelReport.Write("Дата:").Write(date.Date).NewRow();
            excelReport.Write("Время:").Write(date.ToString("HH:mm")).NewRow();
            excelReport.Write("ФИО:").Write(UserProfile.Current.UserName).NewRow();
            excelReport.Write("КОМПЛЕКСЫ ППР:").Write(SelectedYear + " г.").NewRow();
            excelReport.NewRow();
            excelReport.WriteHeader("Наименование", 120);
            excelReport.WriteHeader("Дата начала", 120);
            excelReport.WriteHeader("Дата окончания", 120);
            excelReport.WriteHeader("Объект", 150); 
            excelReport.WriteHeader("Работы", 150); 
            excelReport.WriteHeader("ЛПУ", 150); 
            excelReport.WriteHeader("Вид", 100); 
            excelReport.WriteHeader("Начало", 100); 
            excelReport.WriteHeader("Окончание", 100); 
            excelReport.WriteHeader("Длительность", 100); 
            excelReport.WriteHeader("Описание", 150);
            excelReport.WriteHeader("Технологический коридор", 200); 
            excelReport.WriteHeader("Способ ведения работ", 150); 
            excelReport.WriteHeader("Дата поставки МТР", 150);
            excelReport.WriteHeader("Примечания от ГТП", 150); 
            foreach (var c in complexes)
            {
                string cn = c.Name, cdn = String.Format("{0:dd.MM.yyyy}", c.StartDate), cdk = String.Format("{0:dd.MM.yyyy}", c.EndDate);
                bool printed = false;
                foreach (RepairItem r in c.RepairMainViewModel.RepairList)
                {
                    if (r.ComplexId == c.Id)
                    {
                        printed = printComplexInfo(cn, cdn, cdk, ref excelReport);
                        excelReport.WriteCell(r.GroupObject + "\n" + r.ObjectName);
                        excelReport.WriteCell(r.RepairWorks);
                        excelReport.WriteCell(r.SiteName);
                        excelReport.WriteCell(r.RepairTypeName);
                        excelReport.WriteCell(String.Format("{0:dd.MM.yyyy}", r.StartDatePlan));
                        excelReport.WriteCell(String.Format("{0:dd.MM.yyyy}", r.EndDatePlan));
                        excelReport.WriteCell(String.Format("{0} ч.", r.DurationPlan));
                        excelReport.WriteCell(r.Description);
                        excelReport.WriteCell(r.PipelineGroupName);
                        excelReport.WriteCell(r.ExecutionMeansName);
                        excelReport.WriteCell(r.PartsDeliveryDateString);
                        excelReport.WriteCell(r.CommentGto);
                    }
                }
                if (!printed) { printComplexInfo(cn, cdn, cdk, ref excelReport); for (int i=1; i<13; i++) excelReport.WriteCell(""); }
            }
        }
        bool printComplexInfo(string name, string dtn, string dtk, ref ExcelReport excelReport)
        {
            excelReport.NewRow();
            excelReport.WriteCell(name);
            excelReport.WriteCell(dtn);
            excelReport.WriteCell(dtk);
            return true;
        }
    }
}