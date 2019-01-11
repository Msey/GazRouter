using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.Balances.Swaps
{
    [DataContract]
    public class SwapDTO
    {
        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public string EntityName { get; set; }

        [DataMember]
        public EntityType EntityType { get; set; }

        [DataMember]
        public BalanceItem BalItem { get; set; }

        [DataMember]
        public int SrcOwnerId { get; set; }

        [DataMember]
        public string SrcOwnerName { get; set; }

        [DataMember]
        public int DestOwnerId { get; set; }

        [DataMember]
        public string DestOwnerName { get; set; }

        [DataMember]
        public double Volume { get; set; }
    }
}