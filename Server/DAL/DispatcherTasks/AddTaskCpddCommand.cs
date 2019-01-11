using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks
{
    public class AddTaskCpddCommand : CommandScalar<AddTaskCpddParameterSet, Guid>
    {
        public AddTaskCpddCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddTaskCpddParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("task_id");
            command.AddInputParameter("p_subject", parameters.Subject);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_global_task_id", parameters.GlobalTaskId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddTaskCpddParameterSet parameters)
        {
			return parameters.IsAproved ? "tasks.P_TASK.ADDF_CPDD_APPROVED" : "tasks.P_TASK.AddF_CPDD_ONSUBMIT";
        }
    }
}
