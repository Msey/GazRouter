using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.GasCosts;

namespace GazRouter.Modes.GasCosts.Summary
{
    public class DistrStationCostsSummaryGroup : GasCostsSummaryGroup
    {
        public DistrStationCostsSummaryGroup()
            : base("ГРС")
        {
        }

        public override IEnumerable<CostType> CostTypes
        {
            get
            {
                return DistrStationConsumptionViewModel.GetStaticColumnCollection().Select(e => e.CostType);
            }
        }
    }
}