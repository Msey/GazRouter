using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.GasOwners;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.GasOwners
{
	public class SetGasOwnerDisableCommand : CommandNonQuery<SetGasOwnerDisableParameterSet>
    {
		public SetGasOwnerDisableCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, SetGasOwnerDisableParameterSet parameters)
        {
			command.AddInputParameter("p_gas_owner_id", parameters.GasOwnerId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_item_id", parameters.BalanceItem);
            command.AddInputParameter("p_is_disable", parameters.IsDisable);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(SetGasOwnerDisableParameterSet parameters)
		{
			return "P_GAS_OWNER.Set_Owner_Disable";
		}
    }
}