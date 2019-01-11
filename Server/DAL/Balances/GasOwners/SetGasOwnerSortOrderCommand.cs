using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.GasOwners;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.GasOwners
{
	public class SetGasOwnerSortOrderCommand : CommandNonQuery<SetGasOwnerSortOrderParameterSet>
    {
		public SetGasOwnerSortOrderCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, SetGasOwnerSortOrderParameterSet parameters)
        {
			command.AddInputParameter("P_GAS_OWNER_ID", parameters.GasOwnerId);
			command.AddInputParameter("P_UP_OR_DN", parameters.UpDown);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(SetGasOwnerSortOrderParameterSet parameters)
        {
			return "P_gas_owner.EDIT_SORT_ORDER";
        }

    }

}
