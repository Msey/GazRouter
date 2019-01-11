using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;

namespace GazRouter.DTO.ObjectModel.Inconsistency
{
    [DataContract]
	public class InconsistencyDTO : BaseDto<Guid>
    {
        [DataMember]
		public InconsistencyType InconsistencyTypeId { get; set; }
        
        [DataMember]
		public Guid EntityId { get; set; }

		[DataMember]
		public string EntityName { get; set; }
    }
}
