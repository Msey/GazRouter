using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog.EventRecipient;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.EventRecipient
{
	public class RestoreFromTrashEventCommand : CommandNonQuery<RestoreFromTrashEventParameterSet>
    {
		public RestoreFromTrashEventCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, RestoreFromTrashEventParameterSet parameters)
        {
            command.AddInputParameter("p_event_id", parameters.EventId);
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(RestoreFromTrashEventParameterSet parameters)
        {
            return "eventlog.P_EVENT_RECIPIENT.RestoreFromTrash";
        }
    }
}
