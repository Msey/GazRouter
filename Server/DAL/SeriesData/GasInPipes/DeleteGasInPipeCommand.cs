using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.GasInPipes
{
    public class DeleteGasInPipeCommand : CommandNonQuery<int>
    {
        public DeleteGasInPipeCommand(ExecutionContext context)
                : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_series_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int parameters)
        {
            return "rd.P_GAS_SUPPLY.Remove";
        }
        
    }
}
