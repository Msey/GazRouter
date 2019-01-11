using System;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.TaskRecords
{
    public class TaskRecordsExecutedCommand : CommandNonQuery<Guid>
    {
		public TaskRecordsExecutedCommand(ExecutionContext context)
            : base(context)
		{
            IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("P_RECORD_ID", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(Guid parameters)
        {
            return "tasks.P_TASK_record.SETEXECUTED";
        }
    }
}
