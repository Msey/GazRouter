using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Calculations.Parameter
{
    public class DeleteCalculationParameterCommand : CommandNonQuery<int>
    {
        public DeleteCalculationParameterCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int id)
        {
            command.AddInputParameter("p_parameter_id", id);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int id)
        {
            return "P_PARAMETER.Remove";
        }
    }
}