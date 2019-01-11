using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.Role;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Authorization.Role
{
    public class GetRolesAllQuery : QueryReader<List<RoleDTO>>
    {
        public GetRolesAllQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText()
        {
            return
                @"select t1.role_id,t1.sys_name,t1.description
  from V_ROLES t1";
        }

        protected override List<RoleDTO> GetResult(OracleDataReader reader)
        {
            var roles = new List<RoleDTO>();
            while (reader.Read())
            {
                var role =
                    new RoleDTO
                    {
                        Id = reader.GetValue<int>("role_id"),
                        Name = reader.GetValue<string>("sys_name"),
                        Description = reader.GetValue<string>("description")
                    };
                roles.Add(role);
            }
            return roles;
        }
    }
}