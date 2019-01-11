using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.Role;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Authorization.Permission
{
    public class EditPermissionCommand : CommandNonQuery<PermissionDTO>
    {
        public EditPermissionCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }
        protected override void BindParameters(OracleCommand command, PermissionDTO parameters)
        {
            command.AddInputParameter("p_permission_id", parameters.Id);
            command.AddInputParameter("p_permission", parameters.ItemId);
            command.AddInputParameter("p_role_id", parameters.RoleId);
            command.AddInputParameter("p_name", parameters.Name); 
            command.AddInputParameter("p_permission_value", parameters.Permission);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
        protected override string GetCommandText(PermissionDTO parameters)
        {
            return "P_PERMISSION.Edit";
        }
    }
}
#region trash
//command.AddInputParameter("p_parent_id", parameters.ParentI/);
//            command.AddInputParameter("p_action_id", parameters.Id);
//            command.AddInputParameter("p_action_path", parameters.Path);
//            command.AddInputParameter("p_app_host_name", Context.AppHostName);
//            command.AddInputParameter("p_description", parameters.Description);
//            command.AddInputParameter("p_service_description", parameters.ServiceDescription);
#endregion