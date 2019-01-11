using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.Action;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.Action
{
    public class GetActionsByUserIdQuery : QueryReader<int, List<ActionDTO>>
    {
        public GetActionsByUserIdQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(int parameters)
        {
            return @"select t1.action_id,t1.action_path,t1.action_description
  from V_USERROLEACTIONS t1 where t1.app_host_name = :p2 and user_id = :p1";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p1", parameters);
            command.AddInputParameter("p2", Context.AppHostName);
        }

        protected override List<ActionDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var actions = new List<ActionDTO>();
            while (reader.Read())
            {
                var action =
                    new ActionDTO
                    {
                        Id = reader.GetValue<int>("action_id"),
                        Path = reader.GetValue<string>("action_path"),
                        Description = reader.GetValue<string>("action_description")
                    };
                actions.Add(action);
            }
            return actions;
        }
    }
}