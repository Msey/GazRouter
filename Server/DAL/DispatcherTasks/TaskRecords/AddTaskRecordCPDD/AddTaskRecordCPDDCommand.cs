using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.TaskRecords.AddTaskRecordCPDD
{
    public class AddTaskRecordCPDDCommand : CommandScalar<AddTaskRecordCpddParameterSet, Guid>
    {
		public AddTaskRecordCPDDCommand(ExecutionContext context)
            : base(context)
		{
            IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, AddTaskRecordCpddParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("record_id");
            command.AddInputParameter("p_task_id", parameters.TaskId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_property_type_id", parameters.PropertyTypeId);
            command.AddInputParameter("p_period_type_id", parameters.PeriodTypeId);
            command.AddInputParameter("p_target_value", parameters.TargetValue);
            command.AddInputParameter("p_completion_date", parameters.CompletionDate);
            command.AddInputParameter("p_orderno", parameters.OrderNo);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name_cpdd", parameters.UserNameCpdd);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(AddTaskRecordCpddParameterSet parameters)
        {
            return "tasks.P_TASK_RECORD.AddF_CPDD";
        }
    }
}
