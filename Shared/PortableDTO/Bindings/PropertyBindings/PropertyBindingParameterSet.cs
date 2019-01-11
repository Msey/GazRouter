using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.Bindings.PropertyBindings
{
    public abstract class PropertyBindingParameterSet
    {
        public int SourceId { get; set; }
        public PropertyType PropertyId { get; set; }
        public string ExtEntityId { get; set; }
    }
}