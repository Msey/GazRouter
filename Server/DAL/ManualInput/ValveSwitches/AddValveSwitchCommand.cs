using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.ValveSwitches;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.ValveSwitches
{
    public class AddValveSwitchCommand: CommandNonQuery<AddValveSwitchParameterSet>
    {
        public AddValveSwitchCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddValveSwitchParameterSet parameters)
		{
            command.AddInputParameter("p_switching_date", parameters.SwitchingDate);
            command.AddInputParameter("p_switching_type", (int)parameters.ValveSwitchType);
            command.AddInputParameter("p_entity_id", parameters.ValveId);
            command.AddInputParameter("p_state", parameters.State);
            command.AddInputParameter("p_user_name ", Context.UserIdentifier);
		}

        protected override string GetCommandText(AddValveSwitchParameterSet parameters)
        {
            return "P_VALVE_SWITCHING.Add";
        }
    }
}