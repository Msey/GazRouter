using System;
using System.Collections.Generic;
using GazRouter.DAL.DataLoadMonitoring;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.MeasLine;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DTO.DataLoadMonitoring;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.SeriesData.Series;

namespace GazRouter.DataServices.DataLoadMonitoring
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class DataLoadService :ServiceBase, IDataLoadService
    {
         public List<SiteDataLoadStatistics> GetDataLoadSiteStatisticsTechData(DateTime dt)
         {
             using (var context = OpenDbContext())
             {
                 return  new GetSiteStatisticsByTechDataQuery(context).Execute(dt);
             }
         }
        public List<BaseEntityProperty> GetSiteTechData(EntityPropertyValueParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                return new GetEntityPropertyValues(context).Execute(parameters);
            }
        }

        public GasModeChangeData GetGasModeChangeData(GasModeChangeParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                //return new GetEntityPropertyValues(context).Execute(parameters);
                var chModeData = new GasModeChangeData
                {
                    CompShopChangedValues = new GetChangedValuesCompShop(context).Execute(new GasModeChangeParameterSet
                    {
                        KeyDate1 = parameters.KeyDate1,
                        KeyDate2 = parameters.KeyDate2,
                        PLimit = parameters.PLimit,
                        TLimit = parameters.TLimit
                    }),
                    MeasureLineChangedValues = new GetChangedValuesMeasureLine(context).Execute(new GasModeChangeParameterSet
                    {
                        KeyDate1 = parameters.KeyDate1,
                        KeyDate2 = parameters.KeyDate2,
                        QLimit = parameters.QLimit
                    }),
                    ConsumerChangedValues = new GetChangedValuesConsumer(context).Execute(new GasModeChangeParameterSet
                    {
                        KeyDate1 = parameters.KeyDate1,
                        KeyDate2 = parameters.KeyDate2,
                        QLimit = parameters.QLimit
                    }),
                    KeyDate1 = parameters.KeyDate1,
                    KeyDate2 = parameters.KeyDate2
                };
                return chModeData;
            }
        }

        public GasModeChangeData GetGasModeChangeDataLastSerie()
        {
            const double lim1 = 0.2; //по умолчанию по давлению 0.2 кг/см2
            const int    lim2 = 5;   //по умолчаню 5% на изменению расхода и 5грС на изменение температуры
            var chModeData = new GasModeChangeData();
            using (var context = OpenDbContext())
            {
               // GasModeChangeData chModeData = new GasModeChangeData();
                var lastSeries = new GetLastSeries(context).Execute(PeriodType.Twohours);

                if (lastSeries.LastSerieKeyDate != null && lastSeries.PreviousSerieKeyDate != null)
                {
                    chModeData = new GasModeChangeData
                    {
                        CompShopChangedValues = new GetChangedValuesCompShop(context).Execute(new GasModeChangeParameterSet
                        {
                            KeyDate1 = lastSeries.LastSerieKeyDate.Value,
                            KeyDate2 = lastSeries.PreviousSerieKeyDate.Value,
                            PLimit = lim1,
                            TLimit = lim2
                        }),
                        MeasureLineChangedValues = new GetChangedValuesMeasureLine(context).Execute(new GasModeChangeParameterSet
                        {
                            KeyDate1 = lastSeries.PreviousSerieKeyDate.Value,
                            KeyDate2 = lastSeries.LastSerieKeyDate.Value,
                            QLimit = lim2
                        }),
                        ConsumerChangedValues = new GetChangedValuesConsumer(context).Execute(new GasModeChangeParameterSet
                        {
                            KeyDate1 = lastSeries.PreviousSerieKeyDate.Value,
                            KeyDate2 = lastSeries.LastSerieKeyDate.Value,
                            QLimit = lim2
                        }),
                        KeyDate1 = lastSeries.LastSerieKeyDate.Value,
                        KeyDate2 = lastSeries.PreviousSerieKeyDate.Value
                    };

                }

                return chModeData;
            }

        }

        public GasSupplyDataSetDTO GetGasSupplyDataSet(int systemId)
        {
            using (var context = OpenDbContext())
            {
                GasSupplyDataSetDTO suplDTO = new GasSupplyDataSetDTO();
                var lastSerie =
                    new GetSeriesQuery(context).Execute(new GetSeriesParameterSet {PeriodType = PeriodType.Twohours});
                //suplDTO.CompShops = new GetCompShopListQuery(context).Execute(null);
                //suplDTO.CompStations = new GetCompStationListQuery(context).Execute(null);
                //suplDTO.MeasureLines = new GetMeasLineListQuery(context).Execute(new GetMeasLineListParameterSet
                //{
                //    SystemId = systemId
                //});
                //suplDTO.MeasureStations =
                //    new GetMeasStationListQuery(context).Execute(new GetMeasStationListParameterSet
                //    {
                //        SiteId = null,
                //        SystemId = systemId
                //    });
                suplDTO.Pipelines = new GetPipelineListQuery(context).Execute(new GetPipelineListParameterSet
                {
                    SiteId = null,
                    SystemId = systemId
                });
                

                if (lastSerie != null)
                {
                    suplDTO.Values = new GetGasSupplyValues(context).Execute(lastSerie.Id);
                    suplDTO.ValuesSerie = lastSerie;
                }

                return suplDTO;
            }
        }


        public List<GasSupplyValue> GetGasSupplyValues(DateTime dt)
        {
            using (var context = OpenDbContext())
            {
                
                var serie =
                    new GetSeriesQuery(context).Execute(new GetSeriesParameterSet
                    {
                        PeriodType = PeriodType.Twohours,
                        TimeStamp = dt
                    });
                if(serie != null)
                {
                    return new GetGasSupplyValues(context).Execute(serie.Id);
                }
                return new List<GasSupplyValue>() ;
            }
        }

        public List<GasSupplySumValueDTO> GetSumGasSupplyValuesByEnterprise(GasSupplySumParameterSet paramSet)
        {
            using (var context = OpenDbContext())
            {

                return new GetSumGasVolumeByEnterprise(context).Execute(paramSet);
            }
        }

        
    }
}
