using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.Bindings.EntityPropertyBindings
{
    [DataContract]
    public class EntityPropertyBindingDTO : BaseDto<Guid>
    {

        [DataMember]
        public string ExtKey { get; set; }

        [DataMember]
        public PropertyType PropertyId { get; set; }

        [DataMember]
        public string PropertyName { get; set; }

        [DataMember]
        public Guid EntityId { get; set; }
    }
}