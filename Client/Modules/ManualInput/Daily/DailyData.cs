using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Balances.BalanceMeasurings;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.Entities;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.Series;
using Utils.ValueExtrators;

namespace GazRouter.ManualInput.Daily
{
    public class DailyData
    {

        public SeriesDTO Serie { get; set; }

        public SiteDTO Site { get; set; }

        public EntityPropertyValueExtractor ValueExtractor { get; set; }

        public List<CommonEntityDTO> DisabledEntities { get; set; }

        public ManualInputStateDTO InputState { get; set; }

        public TreeDataDTO MeasStationTree { get; set; }

        public TreeDataDTO DistrStationTree { get; set; }

        public List<OperConsumerDTO> OperConsumers { get; set; } 

        public List<ReducingStationDTO> ReducingStations { get; set; }


        public async static Task<DailyData> Load(DateTime date, SiteDTO site)
        {
            var data = new DailyData();

            data.Site = site;

            data.Serie = await new SeriesDataServiceProxy().AddSerieAsync(
                    new AddSeriesParameterSet
                    {
                        Year = date.Year,
                        Month = date.Month,
                        Day = date.Day,
                        PeriodTypeId = PeriodType.Day
                    });

            data.Serie.KeyDate = date;

            var values = await new BalancesServiceProxy().GetBalanceMeasuringsAsync(
                new GetBalanceMeasuringsParameterSet
                {
                    Year = date.Year,
                    Month = date.Month,
                    Day = date.Day
                });

            data.ValueExtractor = new EntityPropertyValueExtractor(values);

            data.DisabledEntities = await new ObjectModelServiceProxy().GetEntityListAsync(
                new GetEntityListParameterSet
                {
                    SiteId = site.Id,
                    IsInputOff = true
                });

            if (data.Serie != null)
            {
                var stateList = (await new ManualInputServiceProxy().GetInputStateListAsync(
                    new GetManualInputStateListParameterSet
                    {
                        SerieId = data.Serie.Id,
                        SiteId = site.Id
                    }));

                data.InputState = stateList.FirstOrDefault();
            }

            if (data.InputState == null)
                data.InputState = new ManualInputStateDTO();


           data.DistrStationTree = await new ObjectModelServiceProxy().GetDistrStationTreeAsync(
                new GetDistrStationListParameterSet
                {
                    SiteId = site.Id
                });

            data.MeasStationTree = await new ObjectModelServiceProxy().GetMeasStationTreeAsync(
                new GetMeasStationListParameterSet
                {
                    SiteId = site.Id
                });

            data.OperConsumers = await new ObjectModelServiceProxy().GetOperConsumersAsync(
                new GetOperConsumerListParameterSet
                {
                    SiteId = site.Id
                });

            data.ReducingStations =
                await new ObjectModelServiceProxy().GetReducingStationListAsync(
                    new GetReducingStationListParameterSet
                    {
                        SiteId = site.Id
                    });

            return data;
        }
    }
}
