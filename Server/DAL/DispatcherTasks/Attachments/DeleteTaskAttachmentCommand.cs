using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using System;


namespace GazRouter.DAL.DispatcherTasks.Attachments
{
    public class DeleteTaskAttachmentCommand : CommandNonQuery<Guid>
    {
        public DeleteTaskAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, Guid guid)
        {
            command.AddInputParameter("p_task_attachment_id ", guid);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(Guid guid)
        {
            return "P_TASK_ATTACHMENT.REMOVE";
        }

    }
}
