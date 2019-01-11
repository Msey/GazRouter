using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.Dictionaries.EntityTypeProperties
{
	[DataContract]
	public class EntityTypePropertyDTO
	{
        [DataMember]
        public EntityType EntityType { get; set; }

        [DataMember]
        public PropertyType PropertyType { get; set; }

        [DataMember]
        public bool IsMandatory { get; set; }

        [DataMember]
        public bool IsInput { get; set; }

    }
}