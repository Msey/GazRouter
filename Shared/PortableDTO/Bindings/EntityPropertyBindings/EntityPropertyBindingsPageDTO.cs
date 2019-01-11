using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Bindings.EntityPropertyBindings
{
    [DataContract]
	public class EntityPropertyBindingsPageDTO
	{
		[DataMember]
		public int TotalCount { get; set; }

		[DataMember]
		public List<EntityPropertyBindingDTO> Entities { get; set; }
	}
}