using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.GasCosts
{
    public class DeleteGasCostCommand : CommandNonQuery<int>
    {
        public DeleteGasCostCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override string GetCommandText(int parameters)
        {
            return "P_AUX_COST.Remove";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_aux_cost_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
    }
}