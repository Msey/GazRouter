using GazRouter.DAL.Core;
using GazRouter.DTO.Calculations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Calculations
{
    public class RunCalcSqlCommand : CommandNonQuery<RunCalcParameterSet>
    {
        public RunCalcSqlCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, RunCalcParameterSet parameterSet)
        {
            command.AddInputParameter("p_series_id", parameterSet.SeriesId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(RunCalcParameterSet parameters)
        {
            return "rd.P_COMPUTE1.cmp_series";
        }
    }
}