using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.Integro;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.DataExchange.Integro
{
    public class AddEditSummaryParamCommand : CommandNonQuery<AddEditSummaryPParameterSet>
    {
        public AddEditSummaryParamCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override string GetCommandText(AddEditSummaryPParameterSet parameters)
        {
            return "INTEGRO.P_SUMMARIES.AddOrUpdate_SUMMARY_PARAMETER";
        }

        protected override void BindParameters(OracleCommand command, AddEditSummaryPParameterSet parameters)
        {
            command.AddInputParameter("p_SUMMARY_PARAMETER_ID", parameters.SummaryParamId);
            command.AddInputParameter("p_SUMMARY_ID", parameters.SummaryId);
            command.AddInputParameter("p_NAME", parameters.Name);
            command.AddInputParameter("p_PARAMETER_GID", parameters.ParametrGid);
            command.AddInputParameter("p_AGGREGATE", parameters.Aggregate);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
    }
}
