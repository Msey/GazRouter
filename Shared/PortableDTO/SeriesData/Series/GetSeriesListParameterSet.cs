using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.SeriesData.Series
{
    public class GetSeriesListParameterSet
    {
        public GetSeriesListParameterSet()
        {
            PeriodStart = DateTime.Now.AddDays(-1);
            PeriodEnd = DateTime.Now;
        }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public PeriodType? PeriodType { get; set; }
    }
}
