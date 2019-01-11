using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;
namespace GazRouter.DTO.Dashboards.Dashboard
{
	public class EditDashboardParameterSet : AddDashboardParameterSet
	{
		public int DashboardId { get; set; }
        public int IsDeleted { get; set; }
    }
	public class AddDashboardParameterSet : DashboardParameterSet
	{
		public int? FolderId { get; set; }
	}
    public class AddDashboardPermissionParameterSet 
    {
        public int DashboardId { get; set; }
        public int? FolderId { get; set; }
        public Guid Site { get; set; }
        public string DashboardName { get; set; }
        public PeriodType PeriodTypeId { get; set; }
        public int? SortOrder { get; set; }
    }
    public class DashboardParameterSet
	{
		public PeriodType PeriodTypeId { get; set; }
		public string DashboardName { get; set; }
		public int? SortOrder { get; set; }
	}
	public class ShareDashboardParameterSet
	{
		public int DashboardId { get; set; }
		public int UserId { get; set; }
		public bool IsEditable { get; set; }
		public bool IsGrantable { get; set; }
	}
}