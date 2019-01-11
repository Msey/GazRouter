using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Balances.DistrNetworks
{
	public class DeleteDistrNetworkCommand : CommandNonQuery<int>
    {
		public DeleteDistrNetworkCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_distr_network_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(int parameters)
        {
            return "rd.P_DISTR_NETWORK.Remove";
        }
    }
}