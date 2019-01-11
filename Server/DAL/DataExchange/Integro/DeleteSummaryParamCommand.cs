using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.DataExchange.Integro
{
    public class DeleteSummaryParamCommand : CommandNonQuery<Guid>
    {
        public DeleteSummaryParamCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override string GetCommandText(Guid parameters)
        {
            return "INTEGRO.P_SUMMARIES.Remove_SUMMARY_PARAMETER";
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("p_SUMMARY_PARAMETER_ID", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
    }
}
