using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs;
using GazRouter.DataProviders.Calculations;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Calculation;
using GazRouter.DTO.Calculations.Parameter;
using GazRouter.DTO.Dictionaries.ParameterTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.Calculations.Dialogs.AddEditVar
{
    public class AddEditVarViewModel : AddEditViewModelBase<CalculationParameterDTO, int>
    {
        private string _alias;
        private ParameterTypeDTO _selectedParameterType;

        private int _timeShiftValue;
        private TimeShiftUnit _selectedTimeShiftUnit;
        private string _testValue;


        public AddEditVarViewModel(Action<int> actionBeforeClosing, CalculationDTO calc)
            : base(actionBeforeClosing)
        {
            Model.CalculationId = calc.Id;
            SelectedParameterType = ParameterTypeList.First();
            SelectedTimeShiftUnit = TimeShiftUnit.H;
            TimeShiftValue = 0;
            TestValue = "0";

            Init();
        }


        public AddEditVarViewModel(Action<int> actionBeforeClosing, CalculationParameterDTO dto)
            : base(actionBeforeClosing, dto)
        {
            
            Alias = dto.Alias;
            SelectedParameterType = ParameterTypeList.Single(t => t.ParameterType == dto.ParameterTypeId);
            _entityId = dto.EntityId;
            _entityPath = dto.Path;
            _propertyType = dto.PropertyTypeId;

            SelectedTimeShiftUnit = (TimeShiftUnit) Enum.Parse(typeof(TimeShiftUnit), dto.TimeShiftUnit, true);
            TimeShiftValue = dto.TimeShiftValue;
            TestValue = dto.Value;
            

            Init();
        }

        private void Init()
        {
            SelectEntityPropertyCommand = new DelegateCommand(SelectEntityProperty);

            SetValidationRules();
            ValidateAll();
        }


        public string Alias
        {
            get { return _alias; }
            set
            {
                if (SetProperty(ref _alias, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


        public List<ParameterTypeDTO> ParameterTypeList => ClientCache.DictionaryRepository.ParameterTypes;

        public ParameterTypeDTO SelectedParameterType
        {
            get { return _selectedParameterType; }
            set
            {
                if (SetProperty(ref _selectedParameterType, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }



        public DelegateCommand SelectEntityPropertyCommand { get; set; }
        
        private SelectEntityPropertyViewModel _vm;
        private Guid _entityId;
        private string _entityPath;
        private PropertyType? _propertyType;

        public string SelectedPropertyPath =>
            _propertyType.HasValue
                ? $"{_entityPath}. {ClientCache.DictionaryRepository.PropertyTypes.Single(pt => pt.PropertyType == _propertyType).Name}"
                : string.Empty;

        private void SelectEntityProperty()
        {
            _vm = new SelectEntityPropertyViewModel(() =>
            {
                if (_vm.EntitySelector.Entity != null)
                {
                    _entityId = _vm.EntitySelector.Entity.Id;
                    _entityPath = _vm.SelectedEntity.ShortPath;
                    _propertyType = _vm.SelectedEntityProperty.PropertyType;

                    OnPropertyChanged(() => SelectedPropertyPath);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            });
            var view = new SelectEntityProperty { DataContext = _vm };
            view.ShowDialog();
        }

        


        public List<TimeShiftUnit> TimeShiftUnitList => Enum.GetValues(typeof(TimeShiftUnit)).Cast<TimeShiftUnit>().ToList();

        public TimeShiftUnit SelectedTimeShiftUnit
        {
            get { return _selectedTimeShiftUnit; }
            set
            {
                if (SetProperty(ref _selectedTimeShiftUnit, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public int TimeShiftValue
        {
            get { return _timeShiftValue; }
            set
            {
                if (SetProperty(ref _timeShiftValue, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        


        public string TestValue
        {
            get { return _testValue; }
            set
            {
                if (SetProperty(ref _testValue, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        
        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }


        private void SetValidationRules()
        {
            AddValidationFor(() => Alias)
                .When(() => string.IsNullOrEmpty(Alias))
                .Show("Введите имя переменной");

            AddValidationFor(() => SelectedPropertyPath)
                .When(() => !_propertyType.HasValue)
                .Show("Не выбран параметр");
        }


        protected override Task<int> CreateTask => 
            new CalculationServiceProxy().AddCalculationParameterAsync(
                new AddEditCalculationParameterParameterSet
                {
                    CalculationId = Model.CalculationId,
                    ParameterTypeId = SelectedParameterType.ParameterType,
                    Alias = Alias,
                    TimeShiftValue = TimeShiftValue,
                    TimeShiftUnit = SelectedTimeShiftUnit,
                    EntityId = _entityId,
                    PropertyTypeId = _propertyType,
                    Value = TestValue
                });

        protected override Task UpdateTask => 
            new CalculationServiceProxy().EditCalculationParameterAsync(
                new AddEditCalculationParameterParameterSet
                {
                    Id = Model.Id,
                    CalculationId = Model.CalculationId,
                    ParameterTypeId = SelectedParameterType.ParameterType,
                    Alias = Alias,
                    TimeShiftValue = TimeShiftValue,
                    TimeShiftUnit = SelectedTimeShiftUnit,
                    EntityId = _entityId,
                    PropertyTypeId = _propertyType,
                    Value = TestValue
                });

        protected override string CaptionEntityTypeName
        {
            get { return " переменной"; }
        }
        


        
    }
}