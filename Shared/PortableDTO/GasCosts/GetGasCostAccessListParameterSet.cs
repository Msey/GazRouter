using System;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.GasCosts
{
    public class GetGasCostAccessListParameterSet
    {
        public GetGasCostAccessListParameterSet()
        {
            Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }

        public DateTime Date { get; set; }
        public Guid? EnterpriseId { get; set; }
        public Guid? SiteId { get; set; }
        public PeriodType PeriodType { get; set; }
    }
}