using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;


namespace GazRouter.DTO.SeriesData.Series
{
    public class GetSeriesParameterSet
    {
        public PeriodType? PeriodType { get; set; }
        
        public DateTime? TimeStamp { get; set; }

        public int? Id { get; set; }
    }
}
