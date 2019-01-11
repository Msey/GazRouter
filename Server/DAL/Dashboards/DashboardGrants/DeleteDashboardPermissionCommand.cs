using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.DashboardGrants;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Dashboards.DashboardGrants
{
    public class DeleteDashboardPermissionCommand : CommandNonQuery<DeleteDashboardPermissionParameterSet>
    {
        public DeleteDashboardPermissionCommand(ExecutionContext context) : base(context)
		{
            IsStoredProcedure = true;
        }
        protected override void BindParameters(OracleCommand command, 
                                               DeleteDashboardPermissionParameterSet parameters)
        {
            command.AddInputParameter("p_dashboard_id", parameters.Id);
            command.AddInputParameter("p_site_id",      parameters.SiteId);
            command.AddInputParameter("p_user_name",    Context.UserIdentifier);
        }
        protected override string GetCommandText(DeleteDashboardPermissionParameterSet parameters)
        {
            return "P_DASHBOARD_PERMISSION.REMOVE";
        }
    }
}
