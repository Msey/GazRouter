using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.DashboardFolder;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.ExcelReport.Folders
{
    public class DeleteDashboardFromFolderCommand : CommandNonQuery<DashboardFolderParameterSet>
    {
        public DeleteDashboardFromFolderCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }
        protected override void BindParameters(OracleCommand command, DashboardFolderParameterSet parameters)
        {
            command.AddInputParameter("P_DASHBOARD_ID", parameters.DashboardId);
            command.AddInputParameter("P_FOLDER_ID", parameters.FolderId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
        protected override string GetCommandText(DashboardFolderParameterSet parameters)
        {
            return "P_dashBoard_folder.Remove";
        }
    }
}