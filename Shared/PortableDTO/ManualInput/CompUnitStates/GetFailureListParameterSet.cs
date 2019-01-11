using System;

namespace GazRouter.DTO.ManualInput.CompUnitStates
{
    public class GetFailureListParameterSet
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public Guid? SiteId { get; set; }
    }
}