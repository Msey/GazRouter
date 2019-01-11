using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Balances.BalanceGroups
{
    [DataContract]
    public class BalanceGroupDTO
	{
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int SystemId { get; set; }

        [DataMember]
        public string SystemName { get; set; }

        [DataMember]
        public int SortOrder { get; set; }
    }
}