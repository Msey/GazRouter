
using System;
using GazRouter.DTO.Dictionaries.BalanceItems;

namespace GazRouter.DTO.Balances.Swaps
{
    public class AddSwapParameterSet
    {
        public int ContractId { get; set; }

        public Guid EntityId { get; set; }
        
        public BalanceItem BalItem { get; set; }
        
        public int SrcOwnerId { get; set; }
        
        public int DestOwnerId { get; set; }
        
        public double Volume { get; set; }
    }

}