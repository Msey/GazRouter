using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.Role;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Authorization.Permission
{
    public class GetPermissionsByRoleIdQuery : QueryReader<int, List<PermissionDTO>>
    {
        public GetPermissionsByRoleIdQuery(ExecutionContext context) : base(context) { }
        protected override string GetCommandText(int parameters)
        {
            return "select " +
                   "t1.permission_id, t1.permission, t1.role_id, t1.name, t1.permission_value " +
                   "from V_PERMISSIONS t1 " +
                   "where t1.role_id = :p1";
        }
        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p1", parameters);
        }
        protected override List<PermissionDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var list = new List<PermissionDTO>();
            while (reader.Read())
            {
                var role = new PermissionDTO
                {
                    Id = reader.GetValue<int>("permission_id"),
                    ItemId = reader.GetValue<int>("permission"),
                    RoleId = reader.GetValue<int>("role_id"),
                    Name = reader.GetValue<string>("name"),
                    Permission = (byte)reader.GetValue<int>("permission_value")
                };
                list.Add(role);
            }
            return list;
        }
    }
}
