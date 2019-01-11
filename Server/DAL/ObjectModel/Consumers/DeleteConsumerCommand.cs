using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Consumers
{
    public class DeleteConsumerCommand : CommandNonQuery<DeleteEntityParameterSet>
    {
        public DeleteConsumerCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }
        
        protected override void BindParameters(OracleCommand command, DeleteEntityParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(DeleteEntityParameterSet parameters)
        {
            return "rd.P_GAS_CONSUMER.Remove";

        }
    }
}