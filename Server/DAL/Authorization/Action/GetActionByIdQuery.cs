using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.Action;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.Action
{
    public class GetActionByIdQuery : QueryReader<int, ActionDTO>
    {
        public GetActionByIdQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(int parameters)
        {
            return
                @"select t1.action_id,t1.action_path,t1.description
  from V_ACTIONS t1 where t1.app_host_name = :p2 and t1.action_id = :p1";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p1", parameters);
            command.AddInputParameter("p2", Context.AppHostName);
        }

        protected override ActionDTO GetResult(OracleDataReader reader, int parameters)
        {
            ActionDTO user = null;
            if (reader.Read())
            {
                user =
                    new ActionDTO
                    {
                        Id = reader.GetValue<int>("action_id"),
                        Path = reader.GetValue<string>("action_path"),
                        Description = reader.GetValue<string>("description")
                    };
            }
            return user;
        }
    }
}