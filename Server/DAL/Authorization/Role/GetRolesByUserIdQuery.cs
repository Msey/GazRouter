using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.Role;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.Role
{
    public class GetRolesByUserIdQuery : QueryReader<int, List<RoleDTO>>
    {
        public GetRolesByUserIdQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(int parameters)
        {
            return
                @"select t1.role_id,t1.role_sys_name,t1.role_description
  from V_USERROLES t1 where t1.user_id = :p1";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p1", parameters);
        }

        protected override List<RoleDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var roles = new List<RoleDTO>();
            while (reader.Read())
            {
                var role =
                    new RoleDTO
                    {
                        Id = reader.GetValue<int>("role_id"),
                        Name = reader.GetValue<string>("role_sys_name"),
                        Description = reader.GetValue<string>("role_description")
                    };
                roles.Add(role);
            }
            return roles;
        }
    }
}