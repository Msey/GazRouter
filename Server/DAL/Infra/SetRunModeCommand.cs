using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Infra
{
    public class SetRunModeCommand : CommandNonQuery<RunMode>
    {
        public SetRunModeCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, RunMode parameters)
        {
            command.AddInputParameter("p_debug_regim", parameters);
        }

        protected override string GetCommandText(RunMode parameters)
        {
            return "rd.COMM.set_debug_regim";
        }
    }
    public enum RunMode
    {
        Standard = 0,
        Debug = 1
    }
}