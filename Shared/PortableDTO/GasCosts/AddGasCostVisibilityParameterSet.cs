using System;
namespace GazRouter.DTO.GasCosts
{
    public class AddGasCostVisibilityParameterSet
    {
        public Guid SiteId { get; set; }
        public int CostType { get; set; }
        public int? Visibility { get; set; }
    }
}
