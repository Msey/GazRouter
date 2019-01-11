using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.Dictionaries.InconsistencyTypes
{
	[DataContract]
	public class InconsistencyTypeDTO : BaseDictionaryDTO
	{

		public InconsistencyType InconsistencyType => (InconsistencyType)Id;

	    [DataMember]
        public EntityType EntityTypeId { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string SystemName { get; set; }

        [DataMember]
        public bool IsCritical { get; set; }
    }
}