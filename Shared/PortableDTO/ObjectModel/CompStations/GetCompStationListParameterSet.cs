using System;
using GazRouter.DTO.ObjectModel.Entities;

namespace GazRouter.DTO.ObjectModel.CompStations
{
    public class GetCompStationListParameterSet : GetEntityListParameterSetBase
    {
        public Guid? SiteId { get; set; }
        public int? SystemId { get; set; }

        public bool? UseInBalance { get; set; }
    }
}