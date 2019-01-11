using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.DashboardGrants;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Dashboards.DashboardGrants
{
    public class AddFolderPermissionCommand : CommandNonQuery<DashboardPermissionParameterSet>
    {
        public AddFolderPermissionCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }
        protected override void BindParameters(OracleCommand command, DashboardPermissionParameterSet parameters)
        {
            command.AddInputParameter("p_folder_id",  parameters.Id);
            command.AddInputParameter("p_site_id",    parameters.SiteId);
            command.AddInputParameter("p_permission", parameters.Permission);
            command.AddInputParameter("p_user_name",  Context.UserIdentifier);
        }
        protected override string GetCommandText(DashboardPermissionParameterSet parameters)
        {
            return "P_FOLDER_PERMISSION.ADD";
        }
    }
}
