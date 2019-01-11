using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Contracts
{
	public class DeleteContractCommand : CommandNonQuery<int>
    {
		public DeleteContractCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_contract_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(int parameters)
        {
            return "P_BL_Contract.Remove";

        }

    }
}