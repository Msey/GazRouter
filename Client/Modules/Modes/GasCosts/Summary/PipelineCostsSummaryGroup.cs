using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.GasCosts;
namespace GazRouter.Modes.GasCosts.Summary
{
    public class PipelineCostsSummaryGroup : GasCostsSummaryGroup
    {
        public PipelineCostsSummaryGroup()
            : base("Линейная часть")
        {
        }
        public override IEnumerable<CostType> CostTypes
        {
            get
            {
                return PipelineConsumptionViewModel.GetStaticColumnCollection().Select(e => e.CostType);
            }
        }
    }
}