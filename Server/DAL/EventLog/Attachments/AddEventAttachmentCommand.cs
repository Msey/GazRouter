using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog.Attachments;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.Attachments
{
    public class AddEventAttachmentCommand : CommandScalar<AddEventAttachmentParameterSet, Guid>
    {
        public AddEventAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddEventAttachmentParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("event_attachment_id");
            command.AddInputParameter("p_event_id", parameters.EventId);
            command.AddInputParameter("p_attachment_data", parameters.Data);
            command.AddInputParameter("p_attachment_file_name", parameters.FileName);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddEventAttachmentParameterSet parameters)
        {
            return "P_EVENT_ATTACHMENT.AddF";
        }
    }
}
