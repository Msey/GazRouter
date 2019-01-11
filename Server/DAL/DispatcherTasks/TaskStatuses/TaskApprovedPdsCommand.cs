using System;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.TaskStatuses
{
    public class TaskApprovedPdsCommand : CommandScalar<Guid, Guid>
    {
        public TaskApprovedPdsCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("task_version_id");
            command.AddInputParameter("p_task_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(Guid parameters)
        {
            return "tasks.P_TASK.Set_APPROVEDFORSITE";
        }
    }
}
