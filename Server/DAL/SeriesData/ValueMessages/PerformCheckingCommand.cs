using GazRouter.DAL.Core;
using GazRouter.DTO.SeriesData.ValueMessages;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.ValueMessages
{
    public class PerformCheckingCommand : CommandNonQuery<PerformCheckingParameterSet>
    {
        public PerformCheckingCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, PerformCheckingParameterSet parameters)
        {
            if (parameters.EntityId.HasValue)
            {
                command.AddInputParameter("p_entity_id", parameters.EntityId);
                command.AddInputParameter("p_is_edit_hist", parameters.SaveHistory);
            }

            command.AddInputParameter("p_series_id", parameters.SerieId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(PerformCheckingParameterSet parameters)
        {
            return "rd.P_COMPUTE1.cmp_series";
        }

    }

}