using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Swaps;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Swaps
{
    public class AddSwapCommand : CommandScalar<AddSwapParameterSet, int>
    {
		public AddSwapCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, AddSwapParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_value_swap_id");
            command.AddInputParameter("p_contract_id", parameters.ContractId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_item_id", parameters.BalItem);
            command.AddInputParameter("p_src_gas_owner_id", parameters.SrcOwnerId);
            command.AddInputParameter("p_dest_gas_owner_id", parameters.DestOwnerId);
            command.AddInputParameter("p_volume", parameters.Volume);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddSwapParameterSet parameters)
		{
			return "P_BL_VALUE_SWAP.AddF";
		}
    }
}