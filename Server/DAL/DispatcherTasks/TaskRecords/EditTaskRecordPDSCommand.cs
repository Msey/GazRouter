using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.TaskRecords
{
	public class EditTaskRecordPDSCommand : CommandNonQuery<EditTaskRecordPdsParameterSet>
    {
        public EditTaskRecordPDSCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, EditTaskRecordPdsParameterSet parameters)
        {
            command.AddInputParameter("P_RECORD_ID", parameters.RowId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_property_type_id", parameters.PropertyTypeId);
            command.AddInputParameter("p_period_type_id", parameters.PeriodTypeId);
            command.AddInputParameter("p_target_value", parameters.TargetValue);
            command.AddInputParameter("p_completion_date", parameters.CompletionDate);
            command.AddInputParameter("p_orderno", parameters.OrderNo);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("P_IS_CPDD_ROW", false);
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(EditTaskRecordPdsParameterSet parameters)
        {
            return "tasks.P_TASK_RECORD.EDIT";
        }
	}
}
