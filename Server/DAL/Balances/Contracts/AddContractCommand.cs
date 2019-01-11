using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Contracts;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Contracts
{
	public class AddContractCommand : CommandScalar<AddContractParameterSet, int>
    {
		public AddContractCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, AddContractParameterSet parameters)
        {
			OutputParameter = command.AddReturnParameter<int>("p_contract_id");
            command.AddInputParameter("p_period_type_id", parameters.PeriodTypeId);
            command.AddInputParameter("p_target_id", parameters.TargetId);
            command.AddInputParameter("p_contract_date", parameters.ContractDate);
            command.AddInputParameter("p_system_id", parameters.GasTransportSystemId);
            command.AddInputParameter("p_is_final", parameters.IsFinal);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(AddContractParameterSet parameters)
		{
			return "P_BL_CONTRACT.AddF";
		}

    }
}