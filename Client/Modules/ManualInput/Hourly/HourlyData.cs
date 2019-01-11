using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.EntityTypeProperties;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ManualInput.CompUnitStates;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.ManualInput.InputStory;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Entities;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DTO.SeriesData.EntityValidationStatus;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SeriesData.ValueMessages;
using GazRouter.ManualInput.Settings.InputOffEntities;


namespace GazRouter.ManualInput.Hourly
{
    public class HourlyData
    {
        public SeriesDTO Serie { get; set; }

        public Guid SiteId { get; set; }

        public List<EntityTypePropertyDTO> PropTypes { get; set; } 

        public TreeDataDTO Tree { get; set; }

        public List<ValveDTO> Valves { get; set; } 

        public Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> PropValues { get; set; }

        public List<CompUnitStateDTO> CompUnitStates { get; set; } 

        public bool IsChecked { get; set; }
        
        public List<EntityValidationStatusDTO> StatusList { get; set; }

        public Dictionary<Guid, Dictionary<PropertyType, List<PropertyValueMessageDTO>>> MsgList { get; set; }
        
        public ManualInputStateDTO InputState { get; set; }

        public List<CommonEntityDTO> InputOffEntities { get; set; }


        public static async Task<HourlyData> LoadData(DateTime timestamp, Guid siteId)
        {
            var data = new HourlyData
            {
                SiteId = siteId
            };

            data.Serie = await new SeriesDataServiceProxy().AddSerieAsync(
                new AddSeriesParameterSet
                {
                    KeyDate = timestamp,
                    PeriodTypeId = PeriodType.Twohours
                });

            data.PropTypes = await new ObjectModelServiceProxy().GetEntityTypePropertiesAsync(null);

            data.InputOffEntities = await new ObjectModelServiceProxy().GetEntityListAsync(
                new GetEntityListParameterSet
                {
                    SiteId = siteId,
                    ShowVirtual = true,
                    IsInputOff = true
                });

            data.Tree = await new ObjectModelServiceProxy().GetFullTreeAsync(
                new EntityTreeGetParameterSet
                {
                    SiteId = siteId,
                    Filter =
                        EntityFilter.CompShops | 
                        EntityFilter.CompStations | 
                        EntityFilter.CompUnits |
                        EntityFilter.DistrStationOutlets | 
                        EntityFilter.DistrStations |
                        EntityFilter.MeasStations |
                        EntityFilter.MeasLines | 
                        EntityFilter.ReducingStations |
                        EntityFilter.MeasPoints 
                });

            data.Valves = await new ObjectModelServiceProxy().GetValveListAsync(
                new GetValveListParameterSet
                {
                    SiteId = siteId,
                    IsControlPoint = true
                });


            data.PropValues = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                new GetEntityPropertyValueListParameterSet
                {
                    StartDate = timestamp.AddHours(-2),
                    EndDate = timestamp,
                    PeriodType = PeriodType.Twohours,
                    LoadMessages = true,
                    CreateEmpty = true
                });

            
            data.CompUnitStates = await new ManualInputServiceProxy().GetCompUnitStateListAsync(
                new GetCompUnitStateListParameterSet
                {
                    SiteId = siteId,
                    Timestamp = timestamp
                });

            data.IsChecked =
                (await
                    new ManualInputServiceProxy().GetInputStoryAsync(new GetManualInputStoryParameterSet
                    {
                        SerieId = data.Serie.Id
                    })).Any();

            data.StatusList = await new ManualInputServiceProxy().GetEntityValidationStatusListAsync(
                new GetEntityValidationStatusListParameterSet
                {
                    SiteId = siteId,
                    SerieId = data.Serie.Id
                });

            
            data.MsgList = await new SeriesDataServiceProxy().GetPropertyValueMessageListAsync(
                new GetPropertyValueMessageListParameterSet
                {
                    SerieId = data.Serie.Id,
                    PeriodType = PeriodType.Twohours,
                    SiteId = siteId
                });

            var stateList = await new ManualInputServiceProxy().GetInputStateListAsync(
                new GetManualInputStateListParameterSet
                {
                    SerieId = data.Serie.Id,
                    SiteId = siteId
                });

            data.InputState = stateList.FirstOrDefault() ?? new ManualInputStateDTO();

            return data;
        }
    }


}
