using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.ConsumerContracts
{
    public class DeleteConsumerContractCommand : CommandNonQuery<int>
    {
        public DeleteConsumerContractCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }
        
        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_consumer_contract_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int parameters)
        {
            return "rd.P_CONSUMER_CONTRACT.Remove";

        }
    }
}