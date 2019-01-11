using System;

namespace GazRouter.DTO.GasLeaks
{
    public class EditLeakParameterSet
    {
        public int LeakId { get; set; }
        public double Kilometer { get; set; }
        public string Place { get; set; }
        public string Reason { get; set; }
        public double VolumeDay { get; set; }
        public string RepairActivity { get; set; }
        public string Description { get; set; }
        public string ContactName { get; set; }
        public DateTime DiscoverDate { get; set; }
        public DateTime? RepairPlanDate { get; set; }
        public DateTime? RepairPlanFact { get; set; }
        public Guid EntityId { get; set; }
    }
}