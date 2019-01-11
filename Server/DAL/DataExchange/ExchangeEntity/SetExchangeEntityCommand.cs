using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.ExchangeEntity
{
    public class SetExchangeEntityCommand : CommandNonQuery<AddEditExchangeEntityParameterSet>
    {
        public SetExchangeEntityCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddEditExchangeEntityParameterSet parameters)
        {
            command.AddInputParameter("p_exchange_task_id", parameters.ExchangeTaskId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_ext_id", parameters.ExtId);
            command.AddInputParameter("p_is_active", parameters.IsActive);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(AddEditExchangeEntityParameterSet parameters)
        {
            return "rd.P_EXCHANGE_ENTITY.Set_it";

        }

    }
}