using System;
using GazRouter.DTO.Dictionaries.Targets;
using Utils.Extensions;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.GasCosts
{
    public class GetGasCostListParameterSet
    {
        public GetGasCostListParameterSet()
        {
            StartDate = DateTime.Today.MonthStart();
            EndDate = DateTime.Today.MonthEnd();
        }

        public Target Target { get; set; }
        
        public Guid? SiteId { get; set; }

        public int? SystemId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? SeriesId { get; set; }

        public PeriodType? PrdType { get; set; }

        public int? BalanceGroupId { get; set; }
    }
}