using System;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.DTO.Balances.BalanceGroups
{
    public class AddBalanceGroupParameterSet
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }

        public int SystemId { get; set; }
    }
}
