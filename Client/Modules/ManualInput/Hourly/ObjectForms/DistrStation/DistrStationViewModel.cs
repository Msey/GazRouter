using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SeriesData.ValueMessages;

namespace GazRouter.ManualInput.Hourly.ObjectForms.DistrStation
{
    public class DistrStationViewModel : EntityForm
    {

        public DistrStationViewModel(ManualInputEntityNode node, DateTime date, int serieId)
            : base(node, date, serieId)
        {
            OutletList = new List<ManualInputDistrStationOutlet>();
            PressureInlet = new ManualInputPropertyValue();
            TemperatureInlet = new ManualInputPropertyValue();

            SetValidationRules();
        }

        
        public override async void Load()
        {
            Behavior.TryLock();
            try
            {
                // получаем список выходов ГРС
                var outletList = await new ObjectModelServiceProxy().GetDistrStationOutletListAsync(
                    new GetDistrStationOutletListParameterSet
                    {
                        DistrStationId = NodeId
                    });

                
                // получаем значения параметров
                var entityList = outletList.Select(o => o.Id).ToList();
                entityList.Add(NodeId);
                var propValues = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                    new GetEntityPropertyValueListParameterSet
                    {
                        EntityIdList = entityList,
                        StartDate = Date,
                        EndDate = Date,
                        PeriodType = PeriodType,
                        LoadMessages = true
                    });

                OutletList = outletList.Select(o => 
                    new ManualInputDistrStationOutlet
                    {
                        Id = o.Id,
                        Name = o.Name
                    }).ToList();
                OnPropertyChanged(() => OutletList);

                ExtractPropertyValue(PressureInlet, propValues, NodeId, PropertyType.PressureInlet, Date);
                ExtractPropertyValue(TemperatureInlet, propValues, NodeId, PropertyType.TemperatureInlet, Date);
                
                foreach (var outlet in OutletList)
                {
                    ExtractPropertyValue(outlet.PressureOutlet, propValues, outlet.Id, PropertyType.PressureOutlet, Date);
                    ExtractPropertyValue(outlet.TemperatureOutlet, propValues, outlet.Id, PropertyType.TemperatureOutlet, Date);
                    ExtractPropertyValue(outlet.Flow, propValues, outlet.Id, PropertyType.Flow, Date);
                    
                }
                
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
        

        /// <summary>
        /// Список выходов ГРС
        /// </summary>
        public List<ManualInputDistrStationOutlet> OutletList { get; set; }


        /// <summary>
        /// Давление газа на входе ГРС, кг/см²
        /// </summary>
        public ManualInputPropertyValue PressureInlet { get; set; }


        
        /// <summary>
        /// Температура газа на входе ГРС, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureInlet { get; set; }




        private void SetValidationRules()
        {
            PressureInlet.GetPropertyValidation()
                .When(
                    () =>
                        PressureInlet.Value < ValueRangeHelper.OldPressureRange.Min ||
                        PressureInlet.Value > ValueRangeHelper.OldPressureRange.Max)
                .Show(ValueRangeHelper.OldPressureRange.ViolationMessage);

            
            TemperatureInlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureInlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureInlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
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

                if (TemperatureInlet.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.TemperatureInlet,
                        Value = TemperatureInlet.Value.Value
                    });

                if (PressureInlet.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.PressureInlet,
                        Value = PressureInlet.Value.Value
                    });


                foreach (var outlet in OutletList)
                {
                    if (outlet.TemperatureOutlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = outlet.Id,
                            PropertyTypeId = PropertyType.TemperatureOutlet,
                            Value = outlet.TemperatureOutlet.Value.Value
                        });

                    if (outlet.PressureOutlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = outlet.Id,
                            PropertyTypeId = PropertyType.PressureOutlet,
                            Value = outlet.PressureOutlet.Value.Value
                        });

                    if (outlet.Flow.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = outlet.Id,
                            PropertyTypeId = PropertyType.Flow,
                            Value = outlet.Flow.Value.Value
                        });
                }

                await new SeriesDataServiceProxy().SetPropertyValueAsync(valueList);

                // Выполнение проверок
                var checkList = new List<PerformCheckingParameterSet>
                {
                    new PerformCheckingParameterSet
                    {
                        EntityId = NodeId,
                        SerieId = serie.Id,
                        SaveHistory = true
                    }
                };
                checkList.AddRange(OutletList.Select(outlet => new PerformCheckingParameterSet
                {
                    EntityId = outlet.Id,
                    SerieId = serie.Id
                }));

                await new SeriesDataServiceProxy().PerformCheckingAsync(checkList);


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