using System;
using System.Collections.Generic;
using GazRouter.DTO.ObjectModel.CompStations;

namespace GazRouter.DTO.ObjectModel.CompShops
{
    public class GetCompShopListParameterSet
    {
        public int? SystemId { get; set; }
        public List<Guid> StationIdList { get; set; }

        public Guid? PipelineId { get; set; }

        public Guid? SiteId { get; set; }

    }
}