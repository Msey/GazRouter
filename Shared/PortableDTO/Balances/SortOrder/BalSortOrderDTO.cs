using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.BalanceItems;

namespace GazRouter.DTO.Balances.SortOrder
{
    [DataContract]
    public class BalSortOrderDTO
    {
        [DataMember]
        public Guid EntityId { get; set; }
        
        [DataMember]
        public BalanceItem BalItem{ get; set; }

        [DataMember]
        public int SortOrder{ get; set; }
    }
    
}