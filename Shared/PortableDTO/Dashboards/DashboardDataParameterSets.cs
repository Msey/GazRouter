using System;
namespace GazRouter.DTO.Dashboards
{
    public class DashboardDataParameterSets
    {
        public int UserId { get; set; }
        public Guid SiteId { get; set; }
        public bool Filter { get; set; }
    }
}
