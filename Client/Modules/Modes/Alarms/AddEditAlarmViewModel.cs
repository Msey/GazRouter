using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs;
using GazRouter.DataProviders.Alarms;
using GazRouter.DTO;
using GazRouter.DTO.Alarms;
using GazRouter.DTO.Dictionaries.AlarmTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Microsoft.Practices.Prism.Commands;
using Utils.Units;


namespace GazRouter.Modes.Alarms
{
    public class AddEditAlarmViewModel : AddEditViewModelBase<AlarmDTO, int>
    {
        
        public AddEditAlarmViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {

            SelectPropertyCommand = new DelegateCommand(OnSelectPropertyCommand);
            SetValidationRules();

            ActivationDate = DateTime.Now;
            ExpirationDate = ActivationDate.AddDays(7);
        }


        public AddEditAlarmViewModel(Action<int> actionBeforeClosing, AlarmDTO alrm)
            : base(actionBeforeClosing, alrm)
        {
            SelectPropertyCommand = new DelegateCommand(OnSelectPropertyCommand);
            SetValidationRules();
            
            Id = alrm.Id;
            _entityId = alrm.EntityId;
            _entityPath = alrm.EntityName;
            _propertyType =
                ClientCache.DictionaryRepository.PropertyTypes.Single(pt => pt.PropertyType == alrm.PropertyTypeId);
            SelectedAlarmType =
                ClientCache.DictionaryRepository.AlarmTypes.Single(at => at.AlarmType == alrm.AlarmTypeId);
            _setting = alrm.Setting;
            ActivationDate = alrm.ActivationDate;
            ExpirationDate = alrm.ExpirationDate;
            Description = alrm.Description;

        }

        
        public DelegateCommand SelectPropertyCommand { get; set; }


        /// <summary>
        /// Наименование свойста, для отображения в контроле выбора свойства
        /// </summary>
        public string PropertyName
        {
            get
            {
                return _entityPath != null ? _entityPath + ". " + _propertyType.Name : "";
            }
        }

        public bool IsPropertySelected
        {
            get { return _propertyType != null; }
        }
        


        public List<AlarmTypeDTO> AlarmTypeList
        {
            get { return ClientCache.DictionaryRepository.AlarmTypes; }
        }

        private AlarmTypeDTO _selectedAlarmType;
        public AlarmTypeDTO SelectedAlarmType
        {
            get { return _selectedAlarmType; }
            set
            {
                _selectedAlarmType = value;
                OnPropertyChanged(() => SelectedAlarmType);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }


        public string EU
        {
            get
            {
                if (IsPropertySelected)
                {
                    switch (_propertyType.PhysicalType.PhysicalType)
                    {
                        case PhysicalType.Pressure:
                            return Pressure.GetAbbreviation(UserProfile.Current.UserSettings.PressureUnit);
                            
                        case PhysicalType.Temperature:
                            return Temperature.GetAbbreviation(UserProfile.Current.UserSettings.TemperatureUnit);
                            
                        case PhysicalType.Volume:
                            return "тыс.м3";
                    }
                    
                }

                return "";
            }
        }


        private double _setting;
        public double Setting
        {
            get
            {
                return IsPropertySelected
                    ? Math.Round(UserProfile.ToUserUnits(_setting, _propertyType.PropertyType),
                        _propertyType.PhysicalType.DefaultPrecision)
                    : _setting;
            }
            set
            {
                if (IsPropertySelected)
                {
                    _setting = UserProfile.ToServerUnits(value, _propertyType.PropertyType);
                }
                
                OnPropertyChanged(() => Setting);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }


        private DateTime _activationDate;
        public DateTime ActivationDate
        {
            get { return _activationDate; }
            set
            {
                _activationDate = value;
                OnPropertyChanged(() => ActivationDate);
                if (ExpirationDate <= value)
                    ExpirationDate = value.AddDays(7);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }


        public DateTime ActivationDateRangeStart
        {
            get { return DateTime.Now; }
        }


        private DateTime _expirationDate;
        public DateTime ExpirationDate
        {
            get { return _expirationDate; }
            set
            {
                _expirationDate = value;
                OnPropertyChanged(() => ExpirationDate);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(() => Description);
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
            AddValidationFor(() => ExpirationDate)
                .When(() => ExpirationDate <= ActivationDate)
                .Show("Дата окончания действия уставки должна быть больше даты начала действия уставки!");

            AddValidationFor(() => SelectedAlarmType)
                .When(() => IsPropertySelected && SelectedAlarmType == null)
                .Show("Не выбран тип уставки!");

            AddValidationFor(() => PropertyName)
                .When(() => !IsPropertySelected)
                .Show("Не выбран контролируемый параметр!");

            
            
            AddValidationFor(() => Setting)
                .When(() => IsPropertySelected 
                    && _propertyType.PhysicalType.PhysicalType == PhysicalType.Pressure
                    && (_setting < ValueRangeHelper.OldPressureRange.Min || _setting > ValueRangeHelper.OldPressureRange.Max))
                .Show(ValueRangeHelper.OldPressureRange.ViolationMessage);

            AddValidationFor(() => Setting)
                .When(() => IsPropertySelected
                    && _propertyType.PhysicalType.PhysicalType == PhysicalType.Temperature
                    && (_setting < ValueRangeHelper.OldTemperatureRange.Min || _setting > ValueRangeHelper.OldTemperatureRange.Max))
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            AddValidationFor(() => Setting)
                .When(() => IsPropertySelected
                    && _propertyType.PhysicalType.PhysicalType == PhysicalType.Volume
                    && (_setting < 0))
                .Show("Недопустимое значение уставки. Должно быть больше 0.");


        }


        private SelectEntityPropertyViewModel _vm;
        private Guid _entityId;
        private string _entityPath;
        private PropertyTypeDTO _propertyType;
        private PeriodType _periodType;
        private void OnSelectPropertyCommand()
        {
            _vm = new SelectEntityPropertyViewModel(() =>
            {
                if (_vm.SelectedEntity != null)
                {
                    _entityId = _vm.SelectedEntity.Id;
                    _entityPath = _vm.SelectedEntity.ShortPath;
                    _propertyType = _vm.SelectedEntityProperty;
                    _periodType = _vm.SelectedPeriodType.PeriodType;

                    OnPropertyChanged(() => PropertyName);
                    OnPropertyChanged(() => IsPropertySelected);
                    OnPropertyChanged(() => EU);

                    SaveCommand.RaiseCanExecuteChanged();
                }
            }) {AllowedPhisicalTypes = {PhysicalType.Pressure, PhysicalType.Temperature, PhysicalType.Volume}};
            var view = new SelectEntityProperty { DataContext = _vm };
            view.ShowDialog();
        }
        

        protected override string CaptionEntityTypeName
        {
            get { return " уставки"; }
        }


        protected override Task<int> CreateTask
        {
            get
            {
                return new AlarmsServiceProxy().AddAlarmAsync(
                    new AddAlarmParameterSet
                    {
                        EntityId = _entityId,
                        PropertyTypeId = _propertyType.PropertyType,
                        PeriodTypeId = _periodType,
                        AlarmTypeId = SelectedAlarmType.AlarmType,
                        Setting = _setting,
                        ActivationDate = ActivationDate,
                        ExpirationDate = ExpirationDate,
                        Description = Description
                    });
            }
        }

        protected override Task UpdateTask
        {
            get
            {
                return new AlarmsServiceProxy().EditAlarmAsync(
                    new EditAlarmParameterSet
                    {
                        AlarmId = Id,
                        EntityId = _entityId,
                        PropertyTypeId = _propertyType.PropertyType,
                        PeriodTypeId = _periodType,
                        AlarmTypeId = SelectedAlarmType.AlarmType,
                        Setting = _setting,
                        ActivationDate = ActivationDate,
                        ExpirationDate = ExpirationDate,
                        Description = Description
                    });
            }
        }

        
        
    }
}
