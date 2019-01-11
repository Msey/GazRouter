using System;
using System.Collections.Generic;
using GazRouter.Controls.Measurings;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using ViewModelBase = GazRouter.Common.ViewModel.ViewModelBase;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.Modes.ProcessMonitoring.ObjectStory.MeasStation
{
    public class MeasStationStoryViewModel : ViewModelBase
    {
        private readonly Guid _Id;
        private readonly EntityType _type;
        private readonly DateTime _timestamp;
        private readonly double? _plan;
        private readonly double? _capacityrated;

        public MeasStationStoryViewModel(Guid Id, EntityType type, double? plan, DateTime timestamp)
        {
            _Id = Id;
            _type = type;
            _plan = plan;
            _timestamp = timestamp;
            Refresh();
        }

        public async void Refresh()
        {
            try
            {
                Behavior.TryLock();

                var data = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                    new GetEntityPropertyValueListParameterSet
                    {
                        EntityIdList = { _Id },
                        PeriodType = PeriodType.Twohours,
                        StartDate = _timestamp.AddDays(-1),
                        EndDate = _timestamp,
                        LoadMessages = true
                    });


                Items = new List<StationItem>();

                for (var t = _timestamp; t > _timestamp.AddDays(-1); t = t.AddHours(-2))
                {
                    Items.Add(new StationItem(_Id, _type, _plan, t, data));
                }

                OnPropertyChanged(() => Items);

            }
            finally 
            {
                Behavior.TryUnlock();
            }
            
        }

        public List<StationItem> Items { get; set; }
        
    }

    public class StationItem : StoryItemBase
    {


        public StationItem(Guid id, EntityType type, double? plan, DateTime timestamp, Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> propValues)
        {

            Timestamp = timestamp;

            Plan = plan;

            Pressure = new DoubleMeasuring(id, PropertyType.PressureInlet, PeriodType.Twohours, true);
            Temperature = new DoubleMeasuring(id, PropertyType.PressureInlet, PeriodType.Twohours, true);
            Flow = new DoubleMeasuring(id, PropertyType.Flow, PeriodType.Twohours, true);


            Pressure.Extract(propValues, timestamp);
            Temperature.Extract(propValues, timestamp);
            Flow.Extract(propValues, timestamp);

        }


        public DoubleMeasuring Pressure { get; set; }
        public DoubleMeasuring Temperature { get; set; }

        public DoubleMeasuring Flow { get; set; }

        public double? Plan { get; set; }

        public double? PlanDelta => Plan.HasValue && Flow.Value.HasValue ? Flow.Value - Plan : null;

        /// <summary>
        /// Технически возможная производительность ГРС, тыс.м3
        /// </summary>
        public double FlowPossible { get; set; }

        /// <summary>
        /// Загрузка от технически возможной величины, %
        /// </summary>
        public double UtilizationPossible => FlowPossible != 0 ? Math.Round(Flow.Value / FlowPossible * 100 ?? 0) : 0;





    }
}