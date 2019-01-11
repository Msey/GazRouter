using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.DashboardGrants;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dashboards.DashboardGrants
{
	public class DeleteDashboardGrantCommand : CommandNonQuery<DeleteDashboardGrantParameterSet>
    {
		public DeleteDashboardGrantCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, DeleteDashboardGrantParameterSet parameters)
        {
            command.AddInputParameter("P_DASHBOARD_ID", parameters.DashboardId);
            command.AddInputParameter("P_USER_ID", parameters.UserId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(DeleteDashboardGrantParameterSet parameters)
        {
            return "P_dashBoard_grant.Remove";

        }

    }
}