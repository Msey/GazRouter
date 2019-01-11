using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Calculations;
using GazRouter.DTO;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Calculation;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.SysEventTypes;

namespace GazRouter.Modes.Calculations.Dialogs.AddEditCalc
{
    public class AddEditCalcViewModel : AddEditViewModelBase<CalculationDTO, int>
    {
        private PeriodType _selectedPeriodType;
        private string _description;
        private int _sortOrder;

        public AddEditCalcViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            SelectedPeriodType = PeriodType.Twohours;
            CalcStage = CalculationStage.AfterStandard;
            SetValidationRules();
        }


        public AddEditCalcViewModel(Action<int> actionBeforeClosing, CalculationDTO calc)
            : base(actionBeforeClosing, calc)
        {
            Id = calc.Id;
            SysName = calc.SysName;
            SelectedPeriodType = calc.PerionTypeId;
            Description = calc.Description;
            SortOrder = calc.SortOrder;
            CalcStage = calc.CalcStage;

            SetValidationRules();
        }


        private string _sysName;
        public string SysName
        {
            get { return _sysName; }
            set
            {
                if (SetProperty(ref _sysName, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


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
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (SetProperty(ref _description, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public int SortOrder
        {
            get { return _sortOrder; }
            set
            {
                if (SetProperty(ref _sortOrder, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public IEnumerable<CalculationStage> CalcStageList => Enum.GetValues(typeof(CalculationStage)).Cast<CalculationStage>();

        public CalculationStage CalcStage { get; set; }


        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }


        private void SetValidationRules()
        {
            AddValidationFor(() => SysName).When(() => string.IsNullOrEmpty(SysName)).Show("Укажите наименование расчета");
        }


        protected override Task<int> CreateTask => new CalculationServiceProxy().AddCalculationAsync(
            new AddCalculationParameterSet
            {
                Description = Description,
                PeriodTypeId = SelectedPeriodType,
                SysName = SysName,
                SortOrder = SortOrder,
                CalcStage = CalcStage
            });

        protected override Task UpdateTask => new CalculationServiceProxy().EditCalculationAsync(
            new EditCalculationParameterSet
            {
                CalculationId = Id,
                Description = Description,
                PeriodTypeId = SelectedPeriodType,
                Expression = Model.Expression,
                Sql = Model.Sql, 
                SysName = SysName,
                SortOrder = SortOrder,
                CalcStage = CalcStage
            });

        
        protected override string CaptionEntityTypeName => " расчета";


    }
}
