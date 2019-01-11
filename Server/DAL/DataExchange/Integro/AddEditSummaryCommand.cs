using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.Integro;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.Integro
{
    public class AddEditSummaryCommand : CommandNonQuery<AddEditSummaryParameterSet>
    {
        public AddEditSummaryCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override string GetCommandText(AddEditSummaryParameterSet parameters)
        {
            return "INTEGRO.P_SUMMARIES.AddOrUpdate_SUMMARY";
        }

        protected override void BindParameters(OracleCommand command, AddEditSummaryParameterSet parameters)
        {
            command.AddInputParameter("p_SUMMARY_ID", parameters.Id);
            command.AddInputParameter("p_NAME", parameters.Name);
            command.AddInputParameter("p_DESCRIPTOR", parameters.Descriptor);
            command.AddInputParameter("p_TRANSFORM_FILE_NAME", parameters.TransformFileName);
            command.AddInputParameter("p_PERIOD_TYPE_ID", parameters.PeriodType);
            command.AddInputParameter("p_SYSTEM_ID", parameters.SystemId);
            command.AddInputParameter("p_PERIOD_TYPE_DETAIL", 0);
            command.AddInputParameter("p_SESSION_DATA_TYPE", parameters.SessionType ?? string.Empty);
            command.AddInputParameter("p_VALIDATE_FILE_NAME", string.Empty);
            command.AddInputParameter("p_EXCHANGE_TASK_ID", parameters.ExchangeTaskId);
            command.AddInputParameter("p_STATUS_ID", parameters.StatusTypeId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
    }
}
