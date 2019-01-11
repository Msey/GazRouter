using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog.EventRecipient;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.EventRecipient
{
	public class AckEventCommand : CommandNonQuery<AckEventParameterSet>
    {
        public AckEventCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, AckEventParameterSet parameters)
		{
            command.AddInputParameter("p_event_id", parameters.EventId);
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
		}

		protected override string GetCommandText(AckEventParameterSet parameters)
		{
            return "eventlog.P_EVENT_RECIPIENT.Ack";
		}
    }
}