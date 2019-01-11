using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.ExchangeProperty
{
    public class SetExchangePropertyCommand : CommandNonQuery<SetExchangePropertyParameterSet>
    {
        public SetExchangePropertyCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, SetExchangePropertyParameterSet parameters)
        {
            command.AddInputParameter("p_exchange_task_id", parameters.ExchangeTaskId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_property_type_id", parameters.PropertyTypeId);
            command.AddInputParameter("p_ext_id", parameters.ExtId);
            command.AddInputParameter("p_coeff", parameters.Coeff);
            
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(SetExchangePropertyParameterSet parameters)
        {
            return "rd.P_EXCHANGE_PROPERTY.Set_It";

        }

    }
}