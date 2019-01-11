using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.Role;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Authorization.Permission
{    
    public class AddPermissionsCommand : CommandScalar<PermissionDTO, int>
    {
        public AddPermissionsCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, PermissionDTO permission)
        {
            OutputParameter = command.AddReturnParameter<int>("permission_id");
            command.AddInputParameter("p_permission", permission.ItemId);
            command.AddInputParameter("p_role_id", permission.RoleId);
//            command.AddInputParameter("p_parent_id", permission.ParentId);
            command.AddInputParameter("p_name", permission.Name);
            command.AddInputParameter("p_permission_value", permission.Permission);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
        protected override string GetCommandText(PermissionDTO parameters)
        {
            return "P_PERMISSION.AddF";
        }
    }
}
#region trash
//            command.AddInputParameter("p_permission_id", permission.Id);
// 
// function AddF
// (p_permission_id       in  rdi.PERMISSIONS.permission_id%type := null --уникальный идентификатор для добавляемого разрешения.отладка
//, p_permission          in  rdi.PERMISSIONS.permission%type := null --разрешения
//, p_role_id             in  rdi.PERMISSIONS.role_id%type --уникальный идентификатор роли. ref to ROLES.role_id
//, p_parent_id           in  rdi.PERMISSIONS.parent_id%type := null --уникальный идентификатор родительского разрешения. ret to PERMISSIONS.permission_id
//, p_name                in  rdi.PERMISSIONS.name%type := null --наименование разрешения
//, p_permission_value    in  rdi.PERMISSIONS.permission_value%type := null --права доступа (0,1,2)
//,p_user_name           in  varchar2 := null --пользовтель выполнивший действие
//) return rdi.PERMISSIONS.permission_id%type;

//            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
//p_permission_id       in  rdi.PERMISSIONS.permission_id % type := null--уникальный идентификатор для добавляемого разрешения.отладка
//,p_role_id             in  rdi.PERMISSIONS.role_id % type := null--уникальный идентификатор роли. ref to ROLES.role_id
//,p_permission          in  rdi.PERMISSIONS.permission % type := null--права доступа (0, 1, 2)
//,p_parent_id           in  rdi.PERMISSIONS.parent_id % type := null--уникальный идентификатор родительского разрешения. ret to PERMISSIONS.permission_id
//,p_name                in  rdi.PERMISSIONS.name % type := null--наименование разрешения
// , p_user_name           in  varchar2:= null--пользовтель выполнивший действие
// command.AddInputParameter("", Context.UserIdentifier);
#endregion
