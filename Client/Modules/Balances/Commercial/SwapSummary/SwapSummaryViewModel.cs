using System.Collections.Generic;
using GazRouter.Balances.Commercial.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Balances.Swaps;

namespace GazRouter.Balances.Commercial.SwapSummary
{ 
    public class SwapSummaryViewModel : ViewModelBase
    {
        public SwapSummaryViewModel(BalanceDataBase data)
        {
            Items = new List<SwapDTO>(data.SwapList);
        }

        public List<SwapDTO> Items { get; set; }
    }
}
