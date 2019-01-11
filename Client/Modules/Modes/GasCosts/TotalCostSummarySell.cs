using System.Linq;
using GazRouter.DTO.Dictionaries.Targets;
namespace GazRouter.Modes.GasCosts
{
    public class TotalCostSummarySell : ICostSummarySell
    {
        private readonly EntitySummaryRow _entitySummaryRow;
        public TotalCostSummarySell(EntitySummaryRow entitySummaryRow)
        {
            _entitySummaryRow = entitySummaryRow;
        }
        public bool MayContainValue
        {
            get { return _entitySummaryRow.Cells.Any(c => c.MayContainValue); }
        }
        public bool IsEditable
        {
            get { return false; }
        }
        public double this[Target target]
        {
            get { return _entitySummaryRow.Cells.Sum(c => c[target]); }
        }
    }
}