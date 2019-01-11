using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.DistrNetworks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.DistrNetworks
{
	public class AddDistrNetworkCommand : CommandScalar<AddDistrNetworkParameterSet, int>
    {
		public AddDistrNetworkCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, AddDistrNetworkParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_distr_network_id");
            command.AddInputParameter("p_distr_network_name", parameters.Name);
            command.AddInputParameter("p_region_id", parameters.RegionId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(AddDistrNetworkParameterSet parameters)
		{
			return "rd.P_DISTR_NETWORK.AddF";
		}
    }
}