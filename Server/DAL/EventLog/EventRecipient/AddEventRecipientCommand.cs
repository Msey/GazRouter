using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog.EventRecipient;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.EventRecipient
{
    public class AddEventRecipientCommand : CommandScalar<AddEventRecipientParameterSet, Guid>
    {
        public AddEventRecipientCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, AddEventRecipientParameterSet parameters)
		{
            OutputParameter = command.AddReturnParameter<Guid>("event_recipient_id");
            command.AddInputParameter("p_event_id", parameters.EventId);
            command.AddInputParameter("p_priority_id", parameters.Priority);
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
		}

		protected override string GetCommandText(AddEventRecipientParameterSet parameters)
		{
            return "eventlog.P_EVENT_RECIPIENT.AddF";
		}
    }
}