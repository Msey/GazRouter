using System;
using GazRouter.DTO.Dictionaries.BalanceItems;

namespace GazRouter.DTO.Balances.SortOrder
{
    public class SetBalSortOrderParameterSet
    {
        public Guid EntityId { get; set; }

        public BalanceItem BalItem { get; set; }

        public int SortOrder { get; set; }
    }
}
