using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.Bindings.EntityPropertyBindings
{
    public abstract class EntityPropertyBindingParameterSet
    {
        public int SourceId { get; set; }
        public Guid EntityId { get; set; }
        public string ExtKey { get; set; }
        public PeriodType PeriodTypeId { get; set; }
        public PropertyType PropertyId { get; set; }
    }
}