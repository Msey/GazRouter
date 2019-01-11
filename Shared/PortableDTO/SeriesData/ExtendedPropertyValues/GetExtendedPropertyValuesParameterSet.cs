using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.SeriesData.ExtendedPropertyValues
{
    public class GetExtendedPropertyValuesParameterSet
    {
        public int SeriesId { get; set; }
        public int SourceId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<EntityType> EntityType { get; set; }
        public SortBy SortBy { get; set; }
        public SortOrder SortOrder { get; set; }
    }
}