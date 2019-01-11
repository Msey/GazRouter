using System;
using GazRouter.DTO.ObjectModel.Entities;

namespace GazRouter.DTO.ObjectModel.DistrStations
{
    public class GetDistrStationListParameterSet : GetEntityListParameterSetBase
    {
        public Guid? SiteId { get; set; }

        public int? SystemId { get; set; }

        public Guid? StationId { get; set; }

        public bool? UseInBalance { get; set; }

        public bool ThisEnterprise { get; set; }

        public Guid? EnterpriseId { get; set; }
    }
}