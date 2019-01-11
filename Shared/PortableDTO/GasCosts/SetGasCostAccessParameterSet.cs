using System;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.GasCosts
{
    public class SetGasCostAccessParameterSet
    {
        public DateTime Date { get; set; }

        public Guid SiteId { get; set; }
        
        public Target Target { get; set; }

        public bool IsRestricted { get; set; }

        public PeriodType PeriodType { get; set; }
    }
}