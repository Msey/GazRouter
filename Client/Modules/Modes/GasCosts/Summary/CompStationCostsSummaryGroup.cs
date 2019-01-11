using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.GasCosts;
namespace GazRouter.Modes.GasCosts.Summary
{
    public class CompStationCostsSummaryGroup : GasCostsSummaryGroup
    {
        public CompStationCostsSummaryGroup()
            : base("КС, КЦ")
        {
        }
        public override IEnumerable<CostType> CostTypes
        {
            get
            { 
                return CompStationConsumptionViewModel.GetStaticColumnCollection().Select(e => e.CostType);
            }
        }
    }
}