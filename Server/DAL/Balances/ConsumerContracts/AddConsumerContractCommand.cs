using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.ConsumerContracts;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.ConsumerContracts
{
    public class AddConsumerContractCommand : CommandScalar<AddConsumerContractParameterSet, int>
    {
        public AddConsumerContractCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddConsumerContractParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_consumer_contract_id");
            command.AddInputParameter("p_gas_owner_id", parameters.GasOwnerId);
            command.AddInputParameter("p_gas_consumer_id", parameters.ConsumerId);
            command.AddInputParameter("p_start_date", parameters.StartDate);
            command.AddInputParameter("p_end_date", parameters.EndDate);
            command.AddInputParameter("p_is_active", parameters.IsActive);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddConsumerContractParameterSet parameters)
        {
            return "rd.P_CONSUMER_CONTRACT.AddF";
        }

    }

}