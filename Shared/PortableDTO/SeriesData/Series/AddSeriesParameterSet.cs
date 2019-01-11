using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;


namespace GazRouter.DTO.SeriesData.Series
{
    public class AddSeriesParameterSet
    {
        public PeriodType PeriodTypeId { get; set; }
        public string Description { get; set; }
        public DateTime KeyDate { get; set; }

        public int Day { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
    }
}
