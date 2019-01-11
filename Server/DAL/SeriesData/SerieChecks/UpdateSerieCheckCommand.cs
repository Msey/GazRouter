using GazRouter.DAL.Core;
using GazRouter.DTO.SeriesData.SerieChecks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.SerieChecks
{
    public class UpdateSerieCheckCommand : CommandNonQuery<UpdateSerieCheckParameterSet>
    {
        public UpdateSerieCheckCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, UpdateSerieCheckParameterSet parameters)
        {
            command.AddInputParameter("p_check_id", parameters.CheckId);
            command.AddInputParameter("p_status", !parameters.IsEnable);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(UpdateSerieCheckParameterSet parameters)
        {
            return "rd.P_CHECK.Set_STATUS";
        }
    }
}



