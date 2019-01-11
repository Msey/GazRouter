using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.DashboardGrants;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dashboards.DashboardGrants
{
	public class EditDashboardGrantCommand : CommandNonQuery<DashboardGrantParameterSet>
    {
		public EditDashboardGrantCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, DashboardGrantParameterSet parameters)
        {

            command.AddInputParameter("P_DASHBOARD_ID", parameters.DashboardId);
            command.AddInputParameter("P_USER_ID", parameters.UserId);
            command.AddInputParameter("P_IS_EDITABLE", parameters.IsEditable);
            command.AddInputParameter("P_IS_GRANTABLE", parameters.IsGrantable);
            command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
        }

		protected override string GetCommandText(DashboardGrantParameterSet parameters)
        {
            return @"P_dashBoard_grant.Edit";
        }

    }

}
