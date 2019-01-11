using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using System;

namespace GazRouter.DAL.DataExchange.Integro
{
    public class DeleteSummaryCommand : CommandNonQuery<Guid>
    {
        public DeleteSummaryCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override string GetCommandText(Guid parameters)
        {
            return "INTEGRO.P_SUMMARIES.Remove_SUMMARY";
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("p_SUMMARY_ID", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
    }
}
