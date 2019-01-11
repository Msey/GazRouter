using System;
using GazRouter.DTO.Dictionaries.Targets;

namespace GazRouter.DTO.GasCosts
{
    public class GetGasCostSumVolumeParameterSet
    {
        public Target? Target { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? EntityId { get; set; }
        public Guid? SiteId { get; set; }
        public CostType? CostType { get; set; }
    }
}