using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog
{
    public class RegisterEventCommand: CommandScalar<RegisterEventParameterSet, int>
    {
        public RegisterEventCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, RegisterEventParameterSet parameters)
		{
            OutputParameter = command.AddReturnParameter<int>("event_id");
            command.AddInputParameter("p_event_date", parameters.EventDate);
            command.AddInputParameter("p_event_text", parameters.Text);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_kilometer", parameters.Kilometer);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
            command.AddInputParameter("p_event_type_id", parameters.TypeId);
        }

        protected override string GetCommandText(RegisterEventParameterSet parameters)
        {
            return "eventlog.P_EVENT.AddF";
        }
    }
}