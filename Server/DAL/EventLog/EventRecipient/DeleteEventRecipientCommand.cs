using System;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.EventRecipient
{
	public class DeleteEventRecipientCommand : CommandNonQuery<Guid>
    {
        public DeleteEventRecipientCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, Guid parameters)
		{
            command.AddInputParameter("p_event_recipient_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
		}

		protected override string GetCommandText(Guid parameters)
		{
            return "eventlog.P_EVENT_RECIPIENT.Remove";
		}
    }
}