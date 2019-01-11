using System;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.Tasks
{
    public class DeleteTaskCommand : CommandNonQuery<Guid>
    {
		public DeleteTaskCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, Guid parameters)
        {
			command.AddInputParameter("P_TASK_ID", parameters);
			command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
        }

		protected override string GetCommandText(Guid parameters)
		{
			return "tasks.P_TASK.REMOVE";
		}
    }
}

