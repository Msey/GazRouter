using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.GasOwners;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.GasOwners
{
	public class SetGasOwnerSystemCommand : CommandNonQuery<SetGasOwnerSystemParameterSet>
	{
		public SetGasOwnerSystemCommand(ExecutionContext context)
			: base(context)
		{
			IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, SetGasOwnerSystemParameterSet parameters)
		{
			command.AddInputParameter("P_GAS_OWNER_ID", parameters.GasOwnerId);
			command.AddInputParameter("P_SYSTEM_ID", parameters.SystemId);
            command.AddInputParameter("P_IS_ACTIVE", parameters.IsActive);
            command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
		}

		protected override string GetCommandText(SetGasOwnerSystemParameterSet parameters)
		{
			return "P_SYSTEM_2_OWNER.Set_ACTIVE";

		}

	}
}