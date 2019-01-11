using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.BalanceItems;

namespace GazRouter.DTO.Balances.Values
{
    public class ClearBalanceValuesParameterSet
    {
        public ClearBalanceValuesParameterSet()
        {
            OwnerIdList = new List<int>();
            BalanceItemList = new List<BalanceItem>();
        }

        public int ContractId { get; set; }

        public Guid? SiteId { get; set; }

        public List<int> OwnerIdList { get; set; }

        public List<BalanceItem> BalanceItemList { get; set; }
    }
}