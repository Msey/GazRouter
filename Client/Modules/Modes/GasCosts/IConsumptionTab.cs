using GazRouter.DTO.GasCosts;
using System.Collections.Generic;

namespace GazRouter.Modes.GasCosts
{
    public interface IConsumptionTab
    {
        string Header { get; }

        List<CostType> GetCostTypeCollection();
    }
}