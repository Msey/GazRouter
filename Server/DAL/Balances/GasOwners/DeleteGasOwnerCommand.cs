using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Balances.GasOwners
{
	public class DeleteGasOwnerCommand : CommandNonQuery<int>
    {
		public DeleteGasOwnerCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_gas_owner_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(int parameters)
        {
            return "P_gas_owner.Remove";
        }
    }
}