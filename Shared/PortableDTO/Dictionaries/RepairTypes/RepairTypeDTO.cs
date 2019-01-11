using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.RepairWorksType;

namespace GazRouter.DTO.Dictionaries.RepairTypes
{
    [DataContract]
    public class RepairTypeDTO : BaseDictionaryDTO
    {
        public RepairTypeDTO()
        {
            RepairWorkTypes = new List<RepairWorkTypeDTO>();
        }

        [DataMember]
        public EntityType EntityType { get; set; }

		[DataMember]
		public string SystemName { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int SortOrder { get; set; }
        
        [DataMember]
        public List<RepairWorkTypeDTO> RepairWorkTypes { get; set; }
    }
}
