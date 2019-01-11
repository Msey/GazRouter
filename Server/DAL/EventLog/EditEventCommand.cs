using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog
{
	public class EditEventCommand : CommandNonQuery<EditEventParameterSet>
    {
        public EditEventCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, EditEventParameterSet parameters)
		{
            command.AddInputParameter("p_event_id", parameters.Id);
            command.AddInputParameter("P_EVENT_TYPE_ID", parameters.TypeId);
            command.AddInputParameter("p_event_date", parameters.EventDate);
            command.AddInputParameter("p_event_text", parameters.Text);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_kilometer", parameters.Kilometer);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
		}

		protected override string GetCommandText(EditEventParameterSet parameters)
		{
            return "eventlog.P_EVENT.Edit";
		}
	}
}