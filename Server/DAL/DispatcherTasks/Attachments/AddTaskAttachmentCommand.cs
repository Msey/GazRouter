using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.Attachments;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.Attachments
{

    
	public class AddTaskAttachmentCommand : CommandScalar<AddTaskAttachmentParameterSet, Guid>
    {
        public AddTaskAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, AddTaskAttachmentParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("task_attachment_id");
            command.AddInputParameter("p_task_id", parameters.TaskId);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_attachment_data", parameters.Data);
            command.AddInputParameter("p_attachment_file_name", parameters.FileName);
            command.AddInputParameter("p_user_name_cpdd", parameters.UserName);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddTaskAttachmentParameterSet parameters)
		{
		    return "P_TASK_ATTACHMENT.AddF";
		}
    }
}
