
using GazRouter.DTO.Dashboards.DashboardFolder;
namespace GazRouter.DTO.ExcelReports
{
    public class MoveDashboardFolderParameterSet : DashboardFolderParameterSet
    {
        public int? OldFolderId { get; set; }
    }
}