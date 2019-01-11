using GazRouter.Balances.Commercial.Common;

namespace GazRouter.Balances.Commercial.OwnersSummary
{
    public class OwnerSummary
    {
        public OwnerSummary()
        {
            Values = new SummaryValues();
        }

        public string OwnerName { get; set; }

        public SummaryValues Values { get; set; }
    }
}
