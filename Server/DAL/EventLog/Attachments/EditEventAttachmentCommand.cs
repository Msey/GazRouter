using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog.Attachments;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.Attachments
{
    public class EditEventAttachmentCommand : CommandNonQuery<EditEventAttachmentParameterSet>
    {
        public EditEventAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditEventAttachmentParameterSet parameters)
        {
            command.AddInputParameter("p_event_attachment_id ", parameters.EventAttachmentId);
            command.AddInputParameter("p_attachment_file_name", parameters.FileName);
            if(parameters.Data!= null) command.AddInputParameter("p_attachment_data", parameters.Data);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditEventAttachmentParameterSet parameters)
        {
            return "P_EVENT_ATTACHMENT.Edit";
        }
	}
}