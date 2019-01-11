using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.Tasks
{
    public class AddTaskCommand : CommandScalar<AddTaskParameterSet, Guid>
    {
        public AddTaskCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddTaskParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("task_id");
            command.AddInputParameter("p_subject", parameters.Subject);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddTaskParameterSet parameters)
        {
            return "tasks.P_TASK.AddF_PDS";
        }
    }
}

