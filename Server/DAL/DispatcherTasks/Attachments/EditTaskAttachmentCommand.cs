using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.Attachments;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.DispatcherTasks.Attachments
{

    public class EditTaskAttachmentCommand : CommandNonQuery<EditTaskAttachmentParameterSet>
    {
        public EditTaskAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditTaskAttachmentParameterSet parameters)
        {
            command.AddInputParameter("p_task_attachment_id", parameters.AttachmentId);
            command.AddInputParameter("p_description", parameters.Description);
            if (parameters.Data != null) command.AddInputParameter("p_attachment_data", parameters.Data);
            command.AddInputParameter("p_attachment_file_name", parameters.FileName);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditTaskAttachmentParameterSet parameters)
        {
            return "P_TASK_ATTACHMENT.EDIT";
        }
    }
}
