using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Transport;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Transport
{
    public class AddTransportCommand : CommandNonQuery<AddTransportParameterSet>
    {
        public AddTransportCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, AddTransportParameterSet parameters)
        {
            
            command.AddInputParameter("p_contract_id", parameters.ContractId);
            command.AddInputParameter("p_gas_owner_id", parameters.OwnerId);
            command.AddInputParameter("p_inlet_id", parameters.InletId);
            command.AddInputParameter("p_outlet_id", parameters.OutletId);
            command.AddInputParameter("p_item_id", parameters.BalanceItem);
            command.AddInputParameter("p_volume", parameters.Volume);
            command.AddInputParameter("p_leng", parameters.Length);
            command.AddInputParameter("p_route_id", parameters.RouteId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);

        }

        protected override string GetCommandText(AddTransportParameterSet parameters)
		{
            return "P_BL_TRANSPORT.Add";
		}

    }
}