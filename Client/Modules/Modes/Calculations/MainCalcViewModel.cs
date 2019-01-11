using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Trends;
using GazRouter.DataProviders.Calculations;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Calculation;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.Modes.Calculations.Dialogs.AddEditCalc;
using GazRouter.Modes.Calculations.Dialogs.AddEditVar;
using GazRouter.Modes.Calculations.Dialogs.ErrorLog;
using GazRouter.Modes.Calculations.Dialogs.GetCalculationsByVar;
using GazRouter.Modes.Calculations.Dialogs.RunCalc;
using Microsoft.Practices.Prism.Regions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
namespace GazRouter.Modes.Calculations
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class MainCalcViewModel : MainViewModelBase
    {
        private CalculationItem _selectedCalc;
        private VarItem _selectedVar;
        private string _error;
        private PeriodType _selectedPeriodType;

        public MainCalcViewModel()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.ConstructCalc);

            SelectedPeriodType = PeriodType.Twohours;

            RefreshCalcCommand = new DelegateCommand(RefreshCalc, () => editPermission);

            AddCalcCommand = new DelegateCommand(AddCalc, () => editPermission);
            EditCalcCommand = new DelegateCommand(EditCalc, () => SelectedCalc != null && editPermission);
            DeleteCalcCommand = new DelegateCommand(DeleteCalc, () => SelectedCalc != null && editPermission);
            RunCalcCommand = new DelegateCommand(RunCalc, () => SelectedCalc != null && !SelectedCalc.Dto.IsInvalid && editPermission);
            ShowAllLogCommand = new DelegateCommand(ShowAllLogs);
            ShowCalcLogCommand = new DelegateCommand(ShowCalcLogs, () => SelectedCalc != null);


            AddVarCommand = new DelegateCommand(AddVar, () => SelectedCalc != null && editPermission);
            EditVarCommand = new DelegateCommand(EditVar, () => SelectedCalc != null && SelectedVar != null && editPermission);
            DeleteVarCommand = new DelegateCommand(DeleteVar, () => SelectedVar != null && editPermission);

            AddToTrendCommand = new DelegateCommand(AddToTrend, () => SelectedVar != null);


            SaveFormulaCommand = new DelegateCommand(SaveFormula, () => SelectedCalc != null && editPermission);
            TestCalcCommand = new DelegateCommand(TestCalc,
                () => SelectedCalc != null && !string.IsNullOrEmpty(SelectedCalc.Dto.Expression));
            
            
            GetCalcsByVarCommand = new DelegateCommand(GetCalcsByVar, () => SelectedVar != null && SelectedVar.IsUsingInOtherCalc);
            
            Error = null;
        }


        #region PERIOD TYPE LIST
        public IEnumerable<PeriodType> PeriodTypeList
        {
            get
            {
                yield return PeriodType.Twohours;
                yield return PeriodType.Day;
            }
        }

        
        public PeriodType SelectedPeriodType
        {
            get { return _selectedPeriodType; }
            set
            {
                if (SetProperty(ref _selectedPeriodType, value))
                    LoadCalcList();
            }
        }
        #endregion


        #region CALCULATION_LIST
        public List<CalculationItem> CalcList { get; set; }

        public async void LoadCalcList(int? calcId = null)
        {
            Error = string.Empty;

            Lock();

            var calcs = await new CalculationServiceProxy().GetCalculationListAsync(
                new GetCalculationListParameterSet
                {
                    PeriodType = SelectedPeriodType
                });
            CalcList = calcs.Select(c => new CalculationItem(c)).ToList();

            OnPropertyChanged(() => CalcList);

            if (calcId.HasValue)
                SelectedCalc = CalcList.SingleOrDefault(c => c.Dto.Id == calcId.Value);

            Unlock();
        }



        public CalculationItem SelectedCalc
        {
            get { return _selectedCalc; }
            set
            {
                if (SetProperty(ref _selectedCalc, value))
                {
                    OnPropertyChanged(() => IsCalcSelected);
                    if (_selectedCalc != null)
                    {
                        OnPropertyChanged(() => SelectedCalc.ExpressionOriginal);
                        LoadVarList();
                        Error = string.Empty;
                    }
                }
            }
        }

        public bool IsCalcSelected => SelectedCalc != null;
        

        #endregion


        #region CALC_COMMANDS

        //Обновить список
        public DelegateCommand RefreshCalcCommand { get; set; }

        private void RefreshCalc()
        {
            LoadCalcList();
        }

        // Добавить расчет
        public DelegateCommand AddCalcCommand { get; set; }
        private void AddCalc()
        {
            var vm = new AddEditCalcViewModel(id => LoadCalcList(id));
            var v = new AddEditCalcView { DataContext = vm };
            v.ShowDialog();
        }

        // Изменить расчет
        public DelegateCommand EditCalcCommand { get; set; }
        private void EditCalc()
        {
            var vm = new AddEditCalcViewModel(id => LoadCalcList(id), SelectedCalc.Dto);
            var v = new AddEditCalcView {DataContext = vm};
            v.ShowDialog();
        }

        // Удалить расчет
        public DelegateCommand DeleteCalcCommand { get; set; }
        private void DeleteCalc()
        {
            MessageBoxProvider.Confirm(
                $"Внимание! Вы собираетесь удалить расчет '{SelectedCalc.Dto.SysName}'. Необходимо Ваше подтверждение.",
                async res =>
                {
                    if (res)
                    {
                        await new CalculationServiceProxy().DeleteCalculationAsync(SelectedCalc.Dto.Id);
                        LoadCalcList();
                    }
                },
                "Подтверждение удаления расчета",
                "Удалить",
                "Отмена");
        }



        // Запуск расчета
        public DelegateCommand RunCalcCommand { get; set; }
        private void RunCalc()
        {
            var v = new RunCalcView { DataContext = new RunCalcViewModel(_selectedCalc.Dto, () => LoadCalcList(SelectedCalc.Dto.Id))};
            v.ShowDialog();
        }

        // Лог ошибок по всем расчетам
        public DelegateCommand ShowAllLogCommand { get; private set; }
        private void ShowAllLogs()
        {
            var vm = new ErrorLogViewModel();
            var v = new ErrorLogView { DataContext = vm };
            v.ShowDialog();
        }

        // Лог ошибок по выбранному расчету
        public DelegateCommand ShowCalcLogCommand { get; private set; }
        private void ShowCalcLogs()
        {
            var vm = new ErrorLogViewModel(SelectedCalc.Dto.Id);
            var v = new ErrorLogView { DataContext = vm };
            v.ShowDialog();
        }


        #endregion


        private void RefreshCommands()
        {
            AddVarCommand.RaiseCanExecuteChanged();
            EditCalcCommand.RaiseCanExecuteChanged();
            DeleteCalcCommand.RaiseCanExecuteChanged();

            SaveFormulaCommand.RaiseCanExecuteChanged();
            TestCalcCommand.RaiseCanExecuteChanged();
            RunCalcCommand.RaiseCanExecuteChanged();
            
            EditVarCommand.RaiseCanExecuteChanged();
            DeleteVarCommand.RaiseCanExecuteChanged();

            AddToTrendCommand.RaiseCanExecuteChanged();
            GetCalcsByVarCommand.RaiseCanExecuteChanged();
        }


        #region VAR LIST

        public List<VarItem> VarList { get; set; }

        public VarItem SelectedVar
        {
            get { return _selectedVar; }
            set
            {
                if (SetProperty(ref _selectedVar, value))
                    RefreshCommands();
            }
        }

        private async void LoadVarList()
        {
            Lock();

            var vars = await new CalculationServiceProxy().GetCalculationParameterByIdAsync(SelectedCalc.Dto.Id);
            VarList = vars.Select(v => new VarItem(v)).ToList();
            OnPropertyChanged(() => VarList);

            RefreshCommands();

            Unlock();
        }

        #endregion



        #region VAR COMMANDS

        public DelegateCommand AddVarCommand { get; }
        private void AddVar()
        {
            var vm = new AddEditVarViewModel(id => LoadVarList(), SelectedCalc.Dto);
            var v = new AddEditVarView { DataContext = vm };
            v.ShowDialog();
        }


        public DelegateCommand EditVarCommand { get; }
        private void EditVar()
        {
            var vm = new AddEditVarViewModel(id => LoadVarList(), SelectedVar.Dto);
            var v = new AddEditVarView { DataContext = vm };
            v.ShowDialog();
        }

        
        public DelegateCommand DeleteVarCommand { get; }
        private void DeleteVar()
        {
            MessageBoxProvider.Confirm(
                $"Внимание! Вы собираетесь удалить переменную '{SelectedVar.Dto.Alias}'. Необходимо Ваше подтверждение.",
                async res =>
                {
                    if (res)
                    {
                        await new CalculationServiceProxy().DeleteCalculationParameterAsync(SelectedVar.Dto.Id);
                        LoadVarList();
                    }
                },
                "Подтверждение удаления переменной",
                "Удалить",
                "Отмена");
        }


        public DelegateCommand AddToTrendCommand { get; }
        private void AddToTrend()
        {
            TrendsHelper.ShowTrends(SelectedVar.Dto.EntityId, SelectedVar.Dto.PropertyTypeId, SelectedCalc.Dto.PerionTypeId);
        }


        public DelegateCommand GetCalcsByVarCommand { get; set; }
        private void GetCalcsByVar()
        {
            var v = new GetCalculationsByVarDialog
            {
                DataContext = new GetCalculationsByVarViewModel(SelectedVar.Dto)
            };
            v.ShowDialog();
        }

        #endregion



        #region OTHER COMMANDS

        public DelegateCommand SaveFormulaCommand { get; set; }

        private async void SaveFormula()
        {
            Lock();

            await new CalculationServiceProxy().EditCalculationAsync(
                new EditCalculationParameterSet
                {
                    CalculationId = SelectedCalc.Dto.Id,
                    SortOrder = SelectedCalc.Dto.SortOrder,
                    Description = SelectedCalc.Dto.Description,
                    PeriodTypeId = SelectedCalc.Dto.PerionTypeId,
                    SysName = SelectedCalc.Dto.SysName,
                    Expression = SelectedCalc.Dto.Expression,
                    ExpressionOriginal = SelectedCalc.ExpressionOriginal,
                    CalcStage = SelectedCalc.Dto.CalcStage
                });

            Unlock();

            LoadCalcList(SelectedCalc.Dto.Id);
            Error = string.Empty;
        }

        public DelegateCommand TestCalcCommand { get; set; }

        private async void TestCalc()
        {
            Lock();
            
            var result = await new CalculationServiceProxy().TestExecuteAsync(
                new TestCalculationParameterSet
                {
                    CalculationId = SelectedCalc.Dto.Id
                });

            if (string.IsNullOrEmpty(result.Error))
            {

                var tmp = new List<VarItem>();
                VarList.ForEach(
                    v =>
                    {
                        v.Value = result.Parameters.Where(p => p.Id == v.Dto.Id).Select(p => p.Value).FirstOrDefault();
                    });
                tmp.AddRange(VarList);
                VarList = tmp;
                OnPropertyChanged(() => VarList);
            }
            else
                Error = result.Error;

            Unlock();
            
        }


        #endregion


        public string Error
        {
            get { return _error; }
            set { SetProperty(ref _error, value); }
        }
        
        

        

        
        
    }
}