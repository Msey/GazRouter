using System;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities
{
    public class CheckEntityQuery : QueryReader<Guid, bool>
    {
        public CheckEntityQuery(ExecutionContext context) : base(context)
        {
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter(":entity_id", parameters);
        }

        protected override string GetCommandText(Guid parameters)
        {
            return @"select count(*) as result from dual where exists (select * from v_entities e where e.entity_id = :entity_id)";
        }

        protected override bool GetResult(OracleDataReader reader, Guid parameters)
        {
            if (reader.Read())
            {
                return reader.GetValue<bool>("result");
            }
            return false;
        }
    }

}