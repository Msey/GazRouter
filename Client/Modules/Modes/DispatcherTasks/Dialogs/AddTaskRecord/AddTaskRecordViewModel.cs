using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DispatcherTask;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.Prism.Commands;


namespace GazRouter.Modes.DispatcherTasks.Dialogs.AddTaskRecord
{
    public class AddTaskRecordViewModel : DialogViewModel
    {
        
        private TaskRecordDTO _record;
        private Guid _taskId;
        private List<Tuple<Guid, PropertyType>> _exceptPropList;


        public AddTaskRecordViewModel(TaskDTO task, Action actionBeforeClosing, List<Tuple<Guid, PropertyType>> exceptPropList)
            : base(actionBeforeClosing)
        {
            SaveCommand = new DelegateCommand(Save, SaveCommandCanExecute);
            SetValidationRules();

            _taskId = task.Id;
            _description = task.Description;
            _completionDate = DateTime.Now.AddMinutes(10);
            _exceptPropList = exceptPropList;
        }


        public AddTaskRecordViewModel(TaskRecordDTO record, Action actionBeforeClosing, List<Tuple<Guid, PropertyType>> exceptPropList)
            : base(actionBeforeClosing)
        {
            SaveCommand = new DelegateCommand(Save, SaveCommandCanExecute);
            SetValidationRules();

            _record = record;
            _exceptPropList = exceptPropList;
            SelectedEntity = record.Entity;
            SelectedProperty =
                ClientCache.DictionaryRepository.PropertyTypes.SingleOrDefault(pt => pt.PropertyType == record.PropertyTypeId);
            _targetValue = record.TargetValue;
            _description = record.Description;
            _completionDate = record.CompletionDate ?? DateTime.Now.AddMinutes(10);
        }


        #region ENTITY

        private CommonEntityDTO _selectedEntity;

        public CommonEntityDTO SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                if (SetProperty(ref _selectedEntity, value))
                {
                    LoadPropertyList();
                    GetEntitySite();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public List<EntityType> AllowedTypes { get; } = new List<EntityType>
        {
            EntityType.CompShop,
            EntityType.CompUnit,
            EntityType.CoolingStation,
            EntityType.CoolingUnit,
            EntityType.DistrStation,
            EntityType.DistrStationOutlet,
            EntityType.MeasStation,
            EntityType.MeasLine,
            EntityType.ReducingStation,
            EntityType.Site,
            EntityType.Valve
        };

        private Guid? _siteId;
        private async void GetEntitySite()
        {
            if (_selectedEntity == null) return;
            _siteId = _selectedEntity.EntityType != EntityType.Site
                ? await new ObjectModelServiceProxy().FindSiteAsync(_selectedEntity.Id)
                : _selectedEntity.Id;
        }

        #endregion

        
        #region PROPERTY

        public List<PropertyTypeDTO> PropertyList { get; set; }

        private void LoadPropertyList()
        {
            var exceptedProps = _exceptPropList.Where(p => p.Item1 == _selectedEntity?.Id).Select(p => p.Item2);
            PropertyList = _selectedEntity != null
                ? ClientCache.DictionaryRepository.EntityTypes.Single(et => et.EntityType == _selectedEntity.EntityType)
                    .EntityProperties.Where(p => exceptedProps.All(ep => ep != p.PropertyType)).ToList()
                : new List<PropertyTypeDTO>();
            OnPropertyChanged(() => PropertyList);
        }

        private PropertyTypeDTO _selectedProperty;

        public PropertyTypeDTO SelectedProperty
        {
            get { return _selectedProperty;}
            set
            {
                if (SetProperty(ref _selectedProperty, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                    LoadCurrentValue();
                }
            }
        }

        public string PropertyPrecisionFormat => $"n{_selectedProperty?.PhysicalType?.DefaultPrecision ?? 2}";

        #endregion
        
        
        
        #region TARGET VALUE

        private string _targetValue;
        public string TargetValue
        {
            get { return _targetValue; }
            set
            {
                _targetValue = value;
                OnPropertyChanged(() => TargetValue);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion



        #region DESCRIPTION

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

        #endregion


        #region COMPLETION DATE

        private DateTime _completionDate;
        public DateTime CompletionDate
        {
            get { return _completionDate; }
            set
            {
                if (SetProperty(ref _completionDate, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region CURRENT VALUE

        public string CurrentValue { get; set; }

        private async void LoadCurrentValue()
        {
            CurrentValue = "";

            if (_selectedProperty == null) return;

            var dto = await new SeriesDataServiceProxy().GetPropertyValueAsync(
                new GetPropertyValueParameterSet
                {
                    EntityId = SelectedEntity.Id,
                    PeriodTypeId = PeriodType.Twohours,
                    PropertyTypeId = SelectedProperty.PropertyType
                });

            var value = dto as PropertyValueDoubleDTO;
            if (value != null)
                CurrentValue =
                    $"{Math.Round(UserProfile.ToUserUnits(value.Value, SelectedProperty.PropertyType), SelectedProperty.PhysicalType.DefaultPrecision)}{UserProfile.UserUnitName(SelectedProperty.PropertyType)} ({value.Date:dd.MM.yyyy HH:mm})";
            OnPropertyChanged(() => CurrentValue);
            
        }

        

        #endregion


        
        public DelegateCommand SaveCommand { get; }
        
        public string Caption => "Задание для ЛПУ";

        protected bool IsEdit => _record != null;

        private bool SaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }

        private void Save()
        {
            if (IsEdit)
                EditTaskRecordPds();
            else
                AddPdsTaskRecord();
        }

        private async void EditTaskRecordPds()
        {
            await new DispatcherTaskServiceProxy().EditTaskRecordPDSAsync(
                new EditTaskRecordPdsParameterSet
                {
                    TaskId = _record.TaskVersionId,
                    EntityId = SelectedEntity.Id,
                    PropertyTypeId = SelectedProperty.PropertyType,
                    PeriodTypeId = PeriodType.Twohours,
                    TargetValue = TargetValue,
                    Description = Description,
                    CompletionDate = CompletionDate,
                    SiteId = _siteId.Value,
                    RowId = _record.Id
                });
            DialogResult = true;
        }

        private async void AddPdsTaskRecord()
        {
            await new DispatcherTaskServiceProxy().AddTaskRecordPDSAsync(
                new AddTaskRecordPdsParameterSet
                {
                    TaskId = _taskId,
                    EntityId = SelectedEntity.Id,
                    PropertyTypeId = SelectedProperty.PropertyType,
                    PeriodTypeId = PeriodType.Twohours,
                    TargetValue = TargetValue,
                    Description = Description,
                    CompletionDate = CompletionDate,
                    SiteId = _siteId.Value
                });
            DialogResult = true;
        }

        
        private void SetValidationRules()
        {
            AddValidationFor(() => SelectedEntity)
                .When(() => SelectedEntity == null)
                .Show("Не выбран объект");

            AddValidationFor(() => SelectedProperty)
                .When(() => SelectedProperty == null)
                .Show("Не выбрано свойство");

            AddValidationFor(() => Description)
                .When(() => string.IsNullOrEmpty(Description))
                .Show("Введите описание для задания");

            AddValidationFor(() => CompletionDate)
                .When(() => CompletionDate < DateTime.Now)
                .Show("Нельзя устанавливать прошедшую дату и время для требуемого срока выполнения");
        }
    }
}