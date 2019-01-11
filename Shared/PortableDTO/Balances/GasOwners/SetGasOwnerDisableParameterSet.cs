using System;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.GasLeaks;

namespace GazRouter.DTO.Balances.GasOwners
{
	public class SetGasOwnerDisableParameterSet
    {
	    public int GasOwnerId { get; set; }

        public Guid EntityId { get; set; }

        public bool IsDisable { get; set; }

        public BalanceItem BalanceItem { get; set; }
    }
}