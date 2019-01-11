using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.TaskStatuses
{
	public class TaskApprovedCPDDCommand : CommandScalar<SetTaskStatusParameterSet, Guid>
    {
		public TaskApprovedCPDDCommand(ExecutionContext context)
            : base(context)
		{
            IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, SetTaskStatusParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("task_version_id");
            command.AddInputParameter("p_task_id", parameters.TaskId);
            command.AddInputParameter("p_user_name_cpdd", parameters.UserNameCpdd);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(SetTaskStatusParameterSet parameters)
        {
            return "tasks.P_TASK.Set_APPROVEDBYCPDD";
        }
    }
}
