

using System;

namespace GazRouter.DTO.Repairs.Plan
{
    public class GetRepairPlanParameterSet
    {
        public int Year { get; set; }
        public int SystemId { get; set; }
        public Guid? SiteId { get; set; }
    }
}