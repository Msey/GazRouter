using System;
using System.Collections.Generic;
using GazRouter.Application.Helpers;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SeriesData.ValueMessages;

namespace GazRouter.ManualInput.Hourly.ObjectForms.ReducingStation
{
    public class ReducingStationViewModel : EntityForm
    {

        public ReducingStationViewModel(ManualInputEntityNode node, DateTime date, int serieId)
            : base(node, date, serieId)
        {
            PressureInlet = new ManualInputPropertyValue();
            TemperatureInlet = new ManualInputPropertyValue();
            PressureOutlet = new ManualInputPropertyValue();
            TemperatureOutlet = new ManualInputPropertyValue();

            SetValidationRules();
        }


        public override async void Load()
        {
            Behavior.TryLock();
            try
            {
                // получаем значения параметров
                var entityList = new List<Guid> { NodeId };
                var propValues = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                    new GetEntityPropertyValueListParameterSet
                    {
                        EntityIdList = entityList,
                        StartDate = Date,
                        EndDate = Date,
                        PeriodType = PeriodType,
                        LoadMessages = true
                    });

                ExtractPropertyValue(PressureInlet, propValues, NodeId, PropertyType.PressureInlet, Date);
                ExtractPropertyValue(TemperatureInlet, propValues, NodeId, PropertyType.TemperatureInlet, Date);
                ExtractPropertyValue(PressureOutlet, propValues, NodeId, PropertyType.PressureOutlet, Date);
                ExtractPropertyValue(TemperatureOutlet, propValues, NodeId, PropertyType.TemperatureOutlet, Date);
                
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
        
        
        /// <summary>
        /// Давление газа на входе пункта редуцирования, кг/см2
        /// </summary>
        public ManualInputPropertyValue PressureInlet { get; set; }

        /// <summary>
        /// Температура газа на входе пункта редуцирования, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureInlet { get; set; }

        /// <summary>
        /// Давление газа на выходе пункта редуцирования, кг/см2
        /// </summary>
        public ManualInputPropertyValue PressureOutlet { get; set; }

        /// <summary>
        /// Температура газа на выходе пункта редуцирования, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureOutlet { get; set; }
        


        private void SetValidationRules()
        {
            PressureInlet.GetPropertyValidation()
                .When(
                    () =>
                        PressureInlet.Value < ValueRangeHelper.OldPressureRange.Min 
                        || PressureInlet.Value > ValueRangeHelper.OldPressureRange.Max)
                .Show(ValueRangeHelper.OldPressureRange.ViolationMessage);

            
            TemperatureInlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureInlet.Value < ValueRangeHelper.OldTemperatureRange.Min 
                        || TemperatureInlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);


            PressureOutlet.GetPropertyValidation()
                .When(
                    () =>
                        PressureOutlet.Value < ValueRangeHelper.OldPressureRange.Min
                        || PressureOutlet.Value > ValueRangeHelper.OldPressureRange.Max)
                .Show(ValueRangeHelper.OldPressureRange.ViolationMessage);

            TemperatureOutlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureOutlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureOutlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);


        }


        public override async void Save()
        {
            Behavior.TryLock();
            try
            {
                var serie = await new SeriesDataServiceProxy().AddSerieAsync(
                    new AddSeriesParameterSet
                    {
                        KeyDate = Date,
                        PeriodTypeId = PeriodType
                    });
            

                var valueList = new List<SetPropertyValueParameterSet>();

                if (PressureInlet.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.PressureInlet,
                        Value = PressureInlet.Value.Value
                    });

                if (TemperatureInlet.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.TemperatureInlet,
                        Value = TemperatureInlet.Value.Value
                    });

                if (PressureOutlet.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.PressureOutlet,
                        Value = PressureOutlet.Value.Value
                    });

                if (TemperatureOutlet.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.TemperatureOutlet,
                        Value = TemperatureOutlet.Value.Value
                    });
                

                await new SeriesDataServiceProxy().SetPropertyValueAsync(valueList);

                await new SeriesDataServiceProxy().PerformCheckingAsync(new List<PerformCheckingParameterSet>
                {
                    new PerformCheckingParameterSet
                    {
                        EntityId = NodeId,
                        SerieId = serie.Id,
                        SaveHistory = true
                    }
                });

                if (UpdateNodeStatus != null)
                    UpdateNodeStatus();
                
                Load();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
    }


    
}