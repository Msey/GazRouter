using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.Integro;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.DataExchange.Integro
{
    public class AddEditSummaryParamContentCommand : CommandNonQuery<AddEditSummaryPContentParameterSet>
    {
        public AddEditSummaryParamContentCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override string GetCommandText(AddEditSummaryPContentParameterSet parameters)
        {
            return "INTEGRO.P_SUMMARIES.AddOrUpdate_SUMMARY_P_CONTENT";
        }

        protected override void BindParameters(OracleCommand command, AddEditSummaryPContentParameterSet parameters)
        {
            command.AddInputParameter("p_SUMMARY_PARAMETER_ID", parameters.SummaryParamId);
            command.AddInputParameter("p_ENTITY_ID", parameters.EntityId);
            command.AddInputParameter("p_PROPERTY_TYPE_ID", parameters.PropertyTypeId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
    }
}
