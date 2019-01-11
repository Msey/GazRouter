using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Balances.GasOwners
{
	[DataContract]
	public class GasOwnerDTO : NamedDto<int>
	{
        public GasOwnerDTO()
        {
            SystemList = new List<int>();
			DisableList = new List<GasOwnerDisableDTO>();
        }

		[DataMember]
		public int SortOrder { get; set; }

		[DataMember]
		public string Description { get; set; }

        [DataMember]
        public bool IsLocalContract { get; set; }

        [DataMember]
        public List<int> SystemList { get; set; }
        
        [DataMember]
        public List<GasOwnerDisableDTO> DisableList { get; set; }
	}
}