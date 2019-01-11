using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Contracts
{
	public class CleanContractCommand : CommandNonQuery<int>
    {
        public CleanContractCommand(ExecutionContext context)
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
            return "P_BL_CONTRACT.Remove_Child";

        }

    }
}