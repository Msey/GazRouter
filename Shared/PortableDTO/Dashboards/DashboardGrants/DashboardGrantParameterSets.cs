using System;
using System.Collections.Generic;
namespace GazRouter.DTO.Dashboards.DashboardGrants
{
	public class DeleteDashboardGrantParameterSet
	{
		public int DashboardId { get; set; }
		public int UserId { get; set; }
	}
    public class DeleteDashboardPermissionParameterSet
    {
        public int Id { get; set; }
        public Guid SiteId { get; set; }
    }
    public class DashboardPermissionParameterSet : DeleteDashboardPermissionParameterSet
    {
        public int Permission { get; set; }
    }

    public class DashboardGrantParameterSet
	{
		public int DashboardId { get; set; }
		public int UserId { get; set; }
		public bool IsEditable { get; set; }
		public bool IsGrantable { get; set; }
	}

	public class UpdateDashboardGrantParameterSet
	{
	    public int DashboardId { get; set; }
        public List<DashboardGrantDTO> ChangedGrants { get; set; }
    }
    public class UpdateDashboardPermissionsParameterSet
    {
        public int DashboardId { get; set; }
        public List<DashboardGrantDTO2> ChangedPermissions { get; set; }
    }
}