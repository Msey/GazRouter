using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.Role;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Authorization.Permission
{
    public class GetPermissionsQuery : QueryReader<List<PermissionDTO>>
    {
        public GetPermissionsQuery(ExecutionContext context) : base(context){ }
        protected override string GetCommandText()
        {
            return "select * from V_PERMISSIONS t1";
        }
        /// <summary>
        /// 
        /// t1.permission_id,
        /// t1.permission,
        /// t1.role_id,
        /// t1.parent_id,
        /// t1.name,
        /// t1.permission_value
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override List<PermissionDTO> GetResult(OracleDataReader reader)
        {
            var list = new List<PermissionDTO>();
            while (reader.Read())
            {                
                var role = new PermissionDTO
                {
                    Id         = reader.GetValue<int>("permission_id"),
                    ItemId     = reader.GetValue<int>("permission"),
                    ParentId   = reader.GetValue<int>("parent_id"),
                    RoleId     = reader.GetValue<int>("role_id"),
                    Name       = reader.GetValue<string>("name"),
                    Permission = (byte)reader.GetValue<int>("permission_value")
                };
                list.Add(role);
            }
            return list;
        }
    }
}