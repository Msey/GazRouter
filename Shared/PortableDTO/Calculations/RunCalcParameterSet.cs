using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;


namespace GazRouter.DTO.Calculations
{
    public class RunCalcParameterSet
    {
        public int SeriesId { get; set; }
        public PeriodType PeriodType { get; set; }
        public DateTime StartTimeStamp { get; set; }
        public DateTime? EndTimeStamp { get; set; }
    }
}