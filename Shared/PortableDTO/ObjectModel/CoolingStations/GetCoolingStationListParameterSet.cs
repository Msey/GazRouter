using System;

namespace GazRouter.DTO.ObjectModel.CoolingStations
{
    public class GetCoolingStationListParameterSet
    {
        public int? SystemId { get; set; }
        public Guid? SiteId { get; set; }
    }
}
