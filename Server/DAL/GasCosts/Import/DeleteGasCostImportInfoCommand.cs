using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.GasCosts.Import
{
    public class DeleteGasCostImportInfoCommand : CommandNonQuery<int>
    {
        public DeleteGasCostImportInfoCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override string GetCommandText(int parameters)
        {
            return "P_AUX_COST_IMPORT.Remove";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_aux_cost_import_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
    }
}