using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.DashboardGrants;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Dashboards.DashboardGrants
{
    public class AddDashboardPermissionCommand : CommandNonQuery<DashboardPermissionParameterSet>
    {
        public AddDashboardPermissionCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }
        protected override void BindParameters(OracleCommand command, DashboardPermissionParameterSet parameters)
        {
            command.AddInputParameter("p_dashboard_id", parameters.Id);
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_permission", parameters.Permission);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
        protected override string GetCommandText(DashboardPermissionParameterSet parameters)
        {
            return "P_DASHBOARD_PERMISSION.ADD";
        }
    }
}
