using System;
using GazRouter.DTO.Dictionaries.BalanceItems;

namespace GazRouter.Balances.Commercial.Common
{
    public class ItemActions
    {
        public Action<Guid, BalanceItem> ShowHideOwnerAction { get; set; }

        public Action<OwnerItem> SwapAction { get; set; }

        public Action<OwnerItem> UnswapAction { get; set; }

        public Action<OwnerItem> RedistrAction { get; set; }
        
    }
}