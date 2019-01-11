using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;


namespace GazRouter.DTO.SeriesData.ValueMessages
{
    public class GetPropertyValueMessageListParameterSet
    {
        public GetPropertyValueMessageListParameterSet()
        {
            EntityIdList = new List<Guid>();
        }

        public List<Guid> EntityIdList { get; set; }
        public Guid? SiteId { get; set; }

        public PropertyType? PropertyType { get; set; }

        //Можно выбирать либо по serieId, либо по комбинации Date и PeriodType
        public int? SerieId { get; set; }
        public PeriodType? PeriodType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
