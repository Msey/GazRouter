using System;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.SeriesData.PropertyValues
{
    public class SetPropertyValueParameterSet
    {
        public int SeriesId { get; set; }
        public Guid EntityId { get; set; }
        public PropertyType PropertyTypeId { get; set; }
        public object Value { get; set; }

        public string Annotation { get; set; }
    }
}
