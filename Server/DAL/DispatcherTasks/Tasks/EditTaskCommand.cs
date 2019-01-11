using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.Tasks
{
    public class EditTaskCommand : CommandNonQuery<EditTaskParameterSet>
    {
		public EditTaskCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, EditTaskParameterSet parameters)
		{
		    command.AddInputParameter("p_task_id", parameters.TaskId);
            command.AddInputParameter("p_subject", parameters.Subject);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(EditTaskParameterSet parameters)
		{
			return "tasks.P_TASK.Edit";
		}
    }
}

