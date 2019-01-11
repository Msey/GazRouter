using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.TaskRecords
{
    public class AddTaskRecordPDSCommand : CommandScalar<AddTaskRecordPdsParameterSet, Guid>
    {
		public AddTaskRecordPDSCommand(ExecutionContext context)
            : base(context)
		{
            IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, AddTaskRecordPdsParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("record_id");
            command.AddInputParameter("p_task_id", parameters.TaskId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            if(parameters.PropertyTypeId != DTO.Dictionaries.PropertyTypes.PropertyType.None)
                command.AddInputParameter("p_property_type_id", parameters.PropertyTypeId);
            //else
            //    command.AddInputParameter("p_property_type_id", (object)null);
            command.AddInputParameter("p_period_type_id", parameters.PeriodTypeId);
            command.AddInputParameter("p_target_value", parameters.TargetValue);
            command.AddInputParameter("p_completion_date", parameters.CompletionDate);
            command.AddInputParameter("p_orderno", parameters.OrderNo);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("P_SITE_ID", parameters.SiteId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddTaskRecordPdsParameterSet parameters)
        {
            return "tasks.P_TASK_RECORD.AddF_PDS";
        }
    }
}
