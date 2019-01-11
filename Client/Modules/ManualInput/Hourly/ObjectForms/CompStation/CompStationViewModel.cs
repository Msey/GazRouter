using System;
using System.Collections.Generic;
using GazRouter.Application.Helpers;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SeriesData.ValueMessages;

namespace GazRouter.ManualInput.Hourly.ObjectForms.CompStation
{
    public class CompStationViewModel : EntityForm
    {

        public CompStationViewModel(ManualInputEntityNode node, DateTime date, int serieId) 
            : base(node, date, serieId)
        {
            PressureAir = new ManualInputPropertyValue();
            TemperatureAir = new ManualInputPropertyValue();
            TemperatureEarth = new ManualInputPropertyValue();

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
                
                ExtractPropertyValue(PressureAir, propValues, NodeId, PropertyType.PressureAir, Date);
                ExtractPropertyValue(TemperatureAir, propValues, NodeId, PropertyType.TemperatureAir, Date);
                ExtractPropertyValue(TemperatureEarth, propValues, NodeId, PropertyType.TemperatureEarth, Date);
                
                
                
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
        

        
        /// <summary>
        /// Атмосферное давление, мм рт.ст.
        /// </summary>
        public ManualInputPropertyValue PressureAir { get; set; }
        
        /// <summary>
        /// Температура воздуха, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureAir { get; set; }

        /// <summary>
        /// Температура грунта, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureEarth { get; set; }




        private void SetValidationRules()
        {
            PressureAir.GetPropertyValidation()
                .When(
                    () =>
                        PressureAir.Value < ValueRangeHelper.PressureAirRange.Min ||
                        PressureAir.Value > ValueRangeHelper.PressureAirRange.Max)
                .Show(ValueRangeHelper.PressureAirRange.ViolationMessage);

            
            TemperatureAir.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureAir.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureAir.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            TemperatureEarth.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureEarth.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureEarth.Value > ValueRangeHelper.OldTemperatureRange.Max)
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

                if (PressureAir.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.PressureAir,
                        Value = PressureAir.Value.Value
                    });

                if (TemperatureAir.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.TemperatureAir,
                        Value = TemperatureAir.Value.Value
                    });

                if (TemperatureEarth.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.TemperatureEarth,
                        Value = TemperatureEarth.Value.Value
                    });
                

                await new SeriesDataServiceProxy().SetPropertyValueAsync(valueList);

                await new SeriesDataServiceProxy().PerformCheckingAsync(
                    new List<PerformCheckingParameterSet>
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