using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Controls.Measurings;
using GazRouter.Controls.Trends;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.Prism.Commands;
using Telerik.Charting;
using ViewModelBase = GazRouter.Common.ViewModel.ViewModelBase;

namespace GazRouter.Controls.Dialogs.ObjectDetails.Measurings
{
    public class MeasuringsViewModel : ViewModelBase
    {
        private Guid _entityId;
        private EntityType _entityType;


        public MeasuringsViewModel()
        {
            _selectedPeriod = PeriodTypeList.Single(p => p.PeriodType == PeriodType.Twohours);
            _timestamp = SeriesHelper.GetLastCompletedSession();

            ToTrendCommand = new DelegateCommand(ToTrend, () => _selectedItem != null);
            ToValueListCommand = new DelegateCommand(ToValueList, () => _selectedItem != null);
        }



        public MeasuringsViewModel(Guid entityId, EntityType entityType)
        {
            _entityId = entityId;
            _entityType = entityType;

            SelectedPeriod = PeriodTypeList.Single(p => p.PeriodType == PeriodType.Twohours);

            ToTrendCommand = new DelegateCommand(ToTrend, () => _selectedItem != null);
            ToValueListCommand = new DelegateCommand(ToValueList, () => _selectedItem != null);
        }


        public void SetEntity(Guid entityId, EntityType entityType)
        {
            _entityId = entityId;
            _entityType = entityType;
            Refresh();
        }


        public DelegateCommand ToTrendCommand { get; set; }
        public DelegateCommand ToValueListCommand { get; set; }


        private void ToValueList()
        {
            //todo: Реализовать выгрузку значений в таблицу и Excel
            //var vm = new SeriesViewModel(() => { }, ParentEntity, SelectedItem.PropertyType);
            //var dialog = new SeriesView { DataContext = vm };
            //dialog.ShowDialog();
        }

        private void ToTrend()
        {
            TrendsHelper.ShowTrends(_entityId, SelectedItem.PropertyType.PropertyType, _selectedPeriod.PeriodType);
        }



        // Список типов периодов
        public List<PeriodTypeDTO> PeriodTypeList
        {
            get
            {
                return ClientCache.DictionaryRepository.PeriodTypes.Where(
                    p => p.PeriodType == PeriodType.Twohours || p.PeriodType == PeriodType.Day).ToList();
            }
        }

        private PeriodTypeDTO _selectedPeriod;
        /// <summary>
        /// Выбранный тип периода
        /// </summary>
        public PeriodTypeDTO SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                {
                    OnPropertyChanged(() => IsHourSelected);
                    switch (value.PeriodType)
                    {
                        case PeriodType.Twohours:
                            Timestamp = SeriesHelper.GetLastCompletedSession();
                            break;

                        case PeriodType.Day:
                            Timestamp = SeriesHelper.GetPastDispDay();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Выбранные период - час
        /// </summary>
        public bool IsHourSelected => _selectedPeriod?.PeriodType == PeriodType.Twohours;



        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (SetProperty(ref _timestamp, value))
                {
                    Refresh();
                }
            }
        }


        public DateTime MaxAllowedDate => SeriesHelper.GetCurrentDispDay();


        public async void Refresh()
        {
            try
            {
                Behavior.TryLock();
                
                var data = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                    new GetEntityPropertyValueListParameterSet
                    {
                        EntityIdList = { _entityId },
                        PeriodType = _selectedPeriod.PeriodType,
                        StartDate = _timestamp,
                        EndDate = _timestamp,
                        LoadMessages = true,
                        CreateEmpty = true
                    });

                Items = new List<MeasuringBase>();
                var entityType = ClientCache.DictionaryRepository.EntityTypes.Single(et => et.EntityType == _entityType);

                foreach (var prop in entityType.EntityProperties)
                {
                    Items.Add(MeasuringBase.Create(_entityId, prop.PropertyType, _selectedPeriod.PeriodType, data, _timestamp));
                }

                OnPropertyChanged(() => Items);

                SelectedItem = Items.FirstOrDefault();
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }
        
        
        public List<MeasuringBase> Items { get; set; }

        private MeasuringBase _selectedItem;
        public MeasuringBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    OnPropertyChanged(() => IsTrendAllowed);
                    if (_showTrend)
                        LoadTrend();

                    ToValueListCommand.RaiseCanExecuteChanged();
                    ToTrendCommand.RaiseCanExecuteChanged();
                }
            }
        }






        public bool IsTrendAllowed
        {
            get
            {
                return _selectedItem != null && _selectedItem.PropertyType.PhysicalType.TrendAllowed;
            }
        }


        private bool _showTrend;
        public bool ShowTrend
        {
            get { return _showTrend; }
            set
            {
                if (SetProperty(ref _showTrend, value))
                {
                    if (_showTrend)
                        LoadTrend();
                }
            }
        }


        private async void LoadTrend()
        {
            TrendValues = new List<PropertyValueDoubleDTO>();

            if (_selectedItem != null)
            {
                if (!_selectedItem.PropertyType.PhysicalType.TrendAllowed)
                    return;

                try
                {
                    Behavior.TryLock();

                    var deepInDays = _selectedPeriod.PeriodType == PeriodType.Twohours ? 1 : 14;
                    TimeStepUnit = _selectedPeriod.PeriodType == PeriodType.Twohours
                        ? TimeInterval.Hour
                        : TimeInterval.Day;
                    TimeStep = _selectedPeriod.PeriodType == PeriodType.Twohours ? 6 : 7; 

                    var values = (await new SeriesDataServiceProxy().GetPropertyValueListAsync(
                        new GetPropertyValueListParameterSet
                        {
                            EntityId = _selectedItem.EntityId,
                            PropertyTypeId = _selectedItem.PropertyType.PropertyType,
                            PeriodTypeId = _selectedPeriod.PeriodType,
                            EndDate = _timestamp,
                            StartDate = _timestamp.AddDays(-deepInDays)
                        })).OfType<PropertyValueDoubleDTO>().ToList();



                    values.ForEach(
                        v =>
                            v.Value =
                                Math.Round(UserProfile.ToUserUnits(v.Value, _selectedItem.PropertyType.PropertyType),
                                    _selectedItem.PropertyType.PhysicalType.DefaultPrecision));
                    
                    TrendValues = values;
                }
                finally
                {
                    Behavior.TryUnlock();
                }
            }

            OnPropertyChanged(() => TrendValues);
            OnPropertyChanged(() => TimeStepUnit);
            OnPropertyChanged(() => TimeStep);
            OnPropertyChanged(() => TrendMin);
            OnPropertyChanged(() => TrendMax);
        }

        
        public List<PropertyValueDoubleDTO> TrendValues { get; set; }

        public TimeInterval TimeStepUnit { get; set; }
        public int TimeStep { get; set; }


        /// <summary>
        /// Минимальное значение тренда
        /// </summary>
        public double? TrendMin
        {
            get
            {
                return _selectedItem != null && TrendValues != null && TrendValues.Any()
                    ? Math.Round(TrendValues.Min(v => v.Value), _selectedItem.PropertyType.PhysicalType.DefaultPrecision)
                    : (double?) null;
            }
        }

        /// <summary>
        /// Максимальное значение тренда
        /// </summary>
        public double? TrendMax
        {
            get
            {
                return _selectedItem != null && TrendValues != null && TrendValues.Any()
                    ? Math.Round(TrendValues.Max(v => v.Value), _selectedItem.PropertyType.PhysicalType.DefaultPrecision)
                    : (double?) null;
            }
        }
        
    }
}