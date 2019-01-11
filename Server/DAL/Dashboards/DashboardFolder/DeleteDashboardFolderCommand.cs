using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.DashboardFolder;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dashboards.DashboardFolder
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
            command.AddInputParameter("p_dashboard_id", parameters.DashboardId);
            command.AddInputParameter("p_folder_id", parameters.FolderId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(DashboardFolderParameterSet parameters)
        {
            return "p_dashBoard_folder.Remove";
        }

    }
}