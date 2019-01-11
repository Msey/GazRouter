using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.DistrNetworks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.DistrNetworks
{
	public class EditDistrNetworkCommand : CommandNonQuery<EditDistrNetworkParameterSet>
    {
        public EditDistrNetworkCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, EditDistrNetworkParameterSet parameters)
        {
            command.AddInputParameter("p_distr_network_id", parameters.Id);
            command.AddInputParameter("p_distr_network_name", parameters.Name);
            command.AddInputParameter("p_region_id", parameters.RegionId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(EditDistrNetworkParameterSet parameters)
        {
            return "rd.P_DISTR_NETWORK.Edit";
        }

    }

}
