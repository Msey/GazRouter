using System;

namespace GazRouter.DTO.ManualInput.ValveSwitches
{
    public class GetValveSwitchListParameterSet
    {
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? SiteId { get; set; }
        public Guid? ValveId { get; set; }
    }
}