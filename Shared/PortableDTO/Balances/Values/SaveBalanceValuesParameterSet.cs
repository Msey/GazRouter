using System.Collections.Generic;
using GazRouter.DTO.Balances.Swaps;


namespace GazRouter.DTO.Balances.Values
{
    public class SaveBalanceValuesParameterSet
    {
        public SaveBalanceValuesParameterSet()
        {
            ValueList = new List<SetBalanceValueParameterSet>();
            SwapList = new List<AddSwapParameterSet>();
        }

        public int ContractId { get; set; }

        public List<SetBalanceValueParameterSet> ValueList { get; set; }

        public List<AddSwapParameterSet> SwapList { get; set; }
    }
}