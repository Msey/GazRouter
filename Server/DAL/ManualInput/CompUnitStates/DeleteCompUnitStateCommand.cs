using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitStates
{
    public class DeleteCompUnitStateCommand: CommandNonQuery<int>
    {
        public DeleteCompUnitStateCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
		{
            command.AddInputParameter("p_comp_unit_state_change_id", parameters);
        }

        protected override string GetCommandText(int parameters)
        {
            return "rd.P_COMP_UNIT_STATE_CHANGE.Remove";
        }
    }
}