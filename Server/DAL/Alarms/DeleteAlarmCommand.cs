using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Alarms
{
    public class DeleteAlarmCommand : CommandNonQuery<int>
    {
        public DeleteAlarmCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, int parameters)
		{
            command.AddInputParameter("p_alarm_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
		}

        protected override string GetCommandText(int parameters)
        {
            return "rd.P_ALARM.Remove";
        }
    }
}