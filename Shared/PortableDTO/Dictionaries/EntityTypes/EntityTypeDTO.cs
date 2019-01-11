using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypeProperties;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.Dictionaries.EntityTypes
{
	[DataContract]
	public class EntityTypeDTO : BaseDictionaryDTO
	{
        public EntityTypeDTO()
        {
            EntityProperties = new List<PropertyTypeDTO>();
        }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string ShortName { get; set; }

        [DataMember]
		public string SystemName { get; set; }

	    public EntityType EntityType => (EntityType) Id;

	    [DataMember]
        public List<PropertyTypeDTO> EntityProperties { get; set; }

    }
}