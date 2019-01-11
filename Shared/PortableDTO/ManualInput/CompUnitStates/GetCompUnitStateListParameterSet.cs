using System;

namespace GazRouter.DTO.ManualInput.CompUnitStates
{
    public class GetCompUnitStateListParameterSet
    {
        public Guid? SiteId { get; set; }
        public Guid? ShopId { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}