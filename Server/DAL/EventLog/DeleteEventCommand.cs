using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog
{
    public class DeleteEventCommand : CommandNonQuery<int>
    {
        public DeleteEventCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, int parameters)
		{
            command.AddInputParameter("p_event_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
		}

        protected override string GetCommandText(int parameters)
        {
            return "eventlog.P_EVENT.Remove";
        }
    }
}