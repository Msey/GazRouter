using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Calculations.Calculation
{
    public class DeleteCalculationCommand : CommandNonQuery<int>
    {
        public DeleteCalculationCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int id)
        {
            command.AddInputParameter("p_calculation_id", id);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int id)
        {
            return "P_CALCULATION.Remove";
        }
    }
}