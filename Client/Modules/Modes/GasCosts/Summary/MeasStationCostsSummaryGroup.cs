using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.GasCosts;

namespace GazRouter.Modes.GasCosts.Summary
{
    public class MeasStationCostsSummaryGroup : GasCostsSummaryGroup
    {
        public MeasStationCostsSummaryGroup()
            : base("ГИС")
        {
        }

        public override IEnumerable<CostType> CostTypes
        {
            get
            {
                return MeasStationConsumptionViewModel.GetStaticColumnCollection().Select(e => e.CostType);
            }
        }
    }
}