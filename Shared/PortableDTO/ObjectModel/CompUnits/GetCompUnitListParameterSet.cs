using System;

namespace GazRouter.DTO.ObjectModel.CompUnits
{
    public class GetCompUnitListParameterSet
    {
        public Guid? UnitId { get; set; }
        public Guid? ShopId { get; set; }
        public Guid? StationId { get; set; }
        public int? SystemId { get; set; }

        public Guid? SiteId { get; set; }

    }
}
