using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SeriesData.ValueMessages;

namespace GazRouter.ManualInput.Hourly.ObjectForms.MeasStation
{
    public class MeasStationViewModel : EntityForm
    {

        public MeasStationViewModel(ManualInputEntityNode node, DateTime date, int serieId)
            : base(node, date, serieId)
        {
            LineList = new List<ManualInputMeasLine>();
        }

        
        public override async void Load()
        {
            Behavior.TryLock();
            try
            {
                // получаем список выходов ГРС
                var lineList = await new ObjectModelServiceProxy().GetMeasLineListAsync(
                    new GetMeasLineListParameterSet
                    {
                        MeasStationId = NodeId
                    });

                
                // получаем значения параметров
                var entityList = lineList.Select(l => l.Id).ToList();
                var propValues = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                    new GetEntityPropertyValueListParameterSet
                    {
                        EntityIdList = entityList,
                        StartDate = Date,
                        EndDate = Date,
                        PeriodType = PeriodType,
                        LoadMessages = true
                    });

                LineList = lineList.Select(l => 
                    new ManualInputMeasLine
                    {
                        Id = l.Id,
                        Name = l.Name
                    }).ToList();
                OnPropertyChanged(() => LineList);
                

                foreach (var line in LineList)
                {
                    ExtractPropertyValue(line.PressureInlet, propValues, line.Id, PropertyType.PressureInlet, Date);
                    ExtractPropertyValue(line.TemperatureInlet, propValues, line.Id, PropertyType.TemperatureInlet, Date);
                    ExtractPropertyValue(line.Flow, propValues, line.Id, PropertyType.Flow, Date);
                }
                
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        

        /// <summary>
        /// Список замерных линий
        /// </summary>
        public List<ManualInputMeasLine> LineList { get; set; }



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
                

                foreach (var line in LineList)
                {
                    if (line.TemperatureInlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = line.Id,
                            PropertyTypeId = PropertyType.TemperatureInlet,
                            Value = line.TemperatureInlet.Value.Value
                        });

                    if (line.PressureInlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = line.Id,
                            PropertyTypeId = PropertyType.PressureInlet,
                            Value = line.PressureInlet.Value.Value
                        });
                   

                    if (line.Flow.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = line.Id,
                            PropertyTypeId = PropertyType.Flow,
                            Value = line.Flow.Value.Value
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
                checkList.AddRange(LineList.Select(line => new PerformCheckingParameterSet
                {
                    EntityId = line.Id, 
                    SerieId = serie.Id
                }));

                await new SeriesDataServiceProxy().PerformCheckingAsync(checkList);

                UpdateNodeStatus?.Invoke();

                Load();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
    }


    
}