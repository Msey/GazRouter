using System;
using System.Runtime.Serialization;
using GazRouter.DTO;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DAL.Bindings.EntityPropertyBindings
{
    [DataContract]
    public class SimpleEntityPropertyBindingDTO : BaseDto<Guid>
    {
        [DataMember]
        public int SourceId { get; set; }

        [DataMember]
        public string ExtKey { get; set; }

        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public PropertyType PropertyTypeId { get; set; }

        [DataMember]
        public PeriodType PeriodTypeId { get; set; }

        [DataMember]
        public EntityType EntityTypeId { get; set; }
    }
}