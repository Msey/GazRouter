using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.BalanceItems;

namespace GazRouter.DTO.Balances.GasOwners
{
	[DataContract]
	public class GasOwnerDisableDTO
	{
        [DataMember]
        public int OwnerId { get; set; }

        [DataMember]
		public Guid EntityId { get; set; }

        [DataMember]
        public BalanceItem BalanceItem { get; set; }

    }
}