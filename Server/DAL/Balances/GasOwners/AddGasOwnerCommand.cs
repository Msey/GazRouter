using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.GasOwners;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.GasOwners
{
	public class AddGasOwnerCommand : CommandScalar<AddGasOwnerParameterSet, int>
    {
		public AddGasOwnerCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, AddGasOwnerParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("gas_owner_id");
            command.AddInputParameter("p_gas_owner_name", parameters.Name);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_is_local_contract", parameters.IsLocalContract);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(AddGasOwnerParameterSet parameters)
		{
			return "P_gas_owner.AddF";
		}
    }
}