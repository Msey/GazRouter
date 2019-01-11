using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Sites;

namespace GazRouter.DTO.SeriesData
{
    [DataContract]
    public class CompUnitsOperatingTimeDto
    {
        [DataMember]
        public Dictionary<SiteDTO, Dictionary<CompStationDTO, Dictionary<CompShopDTO, List<CompUnitDTO>>>> EntityTree { get; set; }

        [DataMember]
        public Dictionary<Guid, Dictionary<CompUnitState, List<DateIntervalDTO>>> OperatingTime { get; set; }
    }
}
