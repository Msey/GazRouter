using System;

namespace GazRouter.DTO.Bindings.EntityBindings
{
    public  class EntityBindingParameterSet
    {
        public int SourceId { get; set; }
        public Guid EntityId { get; set; }
        public string ExtEntityId { get; set; }
        public bool IsActive { get; set; }
    }
}