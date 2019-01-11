using GazRouter.DAL.Core;
using GazRouter.DTO.SysEvents;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SysEvents
{
    public class SetStatusSysEventCommand : CommandNonQuery<SetStatusSysEventParameters>
    {
        public SetStatusSysEventCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, SetStatusSysEventParameters parameters)
        {
            command.AddInputParameter("p_event_id", parameters.EventId);
            if (parameters.ResultId != null) command.AddInputParameter("p_result_id", (int)parameters.ResultId);
            if (parameters.EventStatusId != null) command.AddInputParameter("p_status_id", (int)parameters.EventStatusId);
            command.AddInputParameter("p_handler_id", 10);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SetStatusSysEventParameters parameters)
        {
            return "P_SYS_EV_EVENT.Set_STATUS";
        }
    }
}