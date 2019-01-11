using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.ConsumerContracts;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.ConsumerContracts
{
    public class EditConsumerContractCommand : CommandNonQuery<EditConsumerContractParameterSet>
    {
        public EditConsumerContractCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditConsumerContractParameterSet parameters)
        {
            command.AddInputParameter("p_consumer_contract_id", parameters.ConsumerContractId);
            command.AddInputParameter("p_gas_owner_id", parameters.GasOwnerId);
            command.AddInputParameter("p_gas_consumer_id", parameters.ConsumerId);
            command.AddInputParameter("p_start_date", parameters.StartDate);
            command.AddInputParameter("p_end_date", parameters.EndDate);
            command.AddInputParameter("p_is_active", parameters.IsActive);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditConsumerContractParameterSet parameters)
        {
            return "rd.P_CONSUMER_CONTRACT.Edit";
        }

    }
}
