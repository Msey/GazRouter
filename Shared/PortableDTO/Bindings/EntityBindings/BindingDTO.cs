using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.Bindings.EntityBindings
{
    [DataContract]
    public class BindingDTO : NamedDto<Guid>
    {

        [DataMember]
        public string ExtEntityId { get; set; }

        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public PropertyType? PropertyId { get; set; }
        
        [DataMember]
        public EntityType? EntityType { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public bool IsActive { get; set; }
    }
}