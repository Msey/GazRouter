using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.ValveSwitches;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.ValveSwitches
{
    public class DeleteValveSwitchCommand: CommandNonQuery<DeleteValveSwitchParameterSet>
    {
        public DeleteValveSwitchCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, DeleteValveSwitchParameterSet parameters)
		{
            command.AddInputParameter("p_switching_date", parameters.SwitchingDate);
            command.AddInputParameter("p_switching_type", parameters.ValveSwitchType);
            command.AddInputParameter("p_entity_id", parameters.ValveId);
        }

        protected override string GetCommandText(DeleteValveSwitchParameterSet parameters)
        {
            return "P_VALVE_SWITCHING.Remove";
        }
    }
}