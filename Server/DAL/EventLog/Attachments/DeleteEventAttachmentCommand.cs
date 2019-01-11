using System;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.Attachments
{
    public class DeleteEventAttachmentCommand : CommandNonQuery<Guid>
    {
        public DeleteEventAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("p_event_attachment_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(Guid parameters)
        {
            return "P_EVENT_ATTACHMENT.Remove";
        }
    }
}
