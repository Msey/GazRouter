using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.ExchangeTask
{
    public class DeleteExchangeTaskCommand : CommandNonQuery<int>
    {
        public DeleteExchangeTaskCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_exchange_task_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(int parameters)
        {
            return "rd.P_EXCHANGE_TASK.Remove";

        }

    }
}