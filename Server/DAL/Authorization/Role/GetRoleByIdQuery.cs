using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.Role;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.Role
{
    public class GetRoleByIdQuery : QueryReader<int, RoleDTO>
    {
        public GetRoleByIdQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(int parameters)
        {
            return
                @"select t1.role_id,t1.sys_name,t1.description
  from V_ROLES t1 where t1.role_id = :p1";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p1", parameters);
        }

        protected override RoleDTO GetResult(OracleDataReader reader, int parameters)
        {
            RoleDTO role = null;
            if (reader.Read())
            {
                role =
                    new RoleDTO
                    {
                        Id = reader.GetValue<int>("role_id"),
                        Name = reader.GetValue<string>("sys_name"),
                        Description = reader.GetValue<string>("description"),
                    };
            }
            return role;
        }
    }
}