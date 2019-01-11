using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.InputStates;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.InputStates
{
    public class SetManualInputStateCommand : CommandNonQuery<SetManualInputStateParameterSet>
    {
        public SetManualInputStateCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, SetManualInputStateParameterSet parameters)
        {
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_series_id", parameters.SerieId);
            command.AddInputParameter("p_state_id", (int)parameters.State);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SetManualInputStateParameterSet parameters)
        {
            return "rd.P_INPUT_STATE.Set_State";
        }
    }
}