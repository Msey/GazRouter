using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.OperConsumers
{
    public class DeleteOperConsumerCommand : CommandNonQuery<DeleteEntityParameterSet>
    {
         public DeleteOperConsumerCommand(ExecutionContext context)
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
            return "P_OPER_CONSUMER.Remove";
        }
    }
}