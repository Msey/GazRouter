using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.Action;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.Action
{
    public class GetActionsAllQuery : QueryReader<List<ActionDTO>>
    {
        public GetActionsAllQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText()
        {
            return
                @"select t1.action_id,t1.action_path,t1.description,t1.service_description
  from V_ACTIONS t1 where t1.app_host_name = :p1";
        }

        protected override void BindParameters(OracleCommand command)
        {
            command.AddInputParameter("p1", Context.AppHostName);
        }

        protected override List<ActionDTO> GetResult(OracleDataReader reader)
        {
            var actions = new List<ActionDTO>();
            while (reader.Read())
            {
                var action =
                    new ActionDTO
                    {
                        Id = reader.GetValue<int>("action_id"),
                        Path = reader.GetValue<string>("action_path"),
                        Description = reader.GetValue<string>("description"),
                        ServiceDescription = reader.GetValue<string>("service_description")
                    };
                actions.Add(action);
            }
            return actions;
        }
    }
}