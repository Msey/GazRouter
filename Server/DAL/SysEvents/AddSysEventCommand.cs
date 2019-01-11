using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.SysEvents;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SysEvents
{
    public class AddSysEventCommand : CommandScalar<AddSysEventParameters, Guid>
    {
        public AddSysEventCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddSysEventParameters parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("event_id");
            if (parameters.EventTypeId != null) command.AddInputParameter("p_event_type_id", (int)parameters.EventTypeId);
            if (parameters.EventStatusId != null) command.AddInputParameter("p_status_id", (int)parameters.EventStatusId);
            if (parameters.EventStatusIdMii != null) command.AddInputParameter("p_status_id_mii", (int)parameters.EventStatusIdMii);
            command.AddInputParameter("p_series_id", parameters.SeriesId);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddSysEventParameters parameters)
        {
            return "P_SYS_EV_EVENT.AddF";
        }
    }
}