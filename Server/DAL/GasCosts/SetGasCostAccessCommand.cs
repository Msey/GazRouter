using GazRouter.DAL.Core;
using GazRouter.DTO.GasCosts;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.GasCosts
{
    public class SetGasCostAccessCommand : CommandNonQuery<SetGasCostAccessParameterSet>
    {
        public SetGasCostAccessCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override string GetCommandText(SetGasCostAccessParameterSet parameters)
        {
            return "rd.P_AUX_ACCESS.Set_ACCESS";
        }

        protected override void BindParameters(OracleCommand command, SetGasCostAccessParameterSet parameters)
        {
            command.AddInputParameter("p_key_date", parameters.Date);
            command.AddInputParameter("p_target_id", parameters.Target);
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_is_restrict", parameters.IsRestricted);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
            command.AddInputParameter("p_period_type_id", parameters.PeriodType);

        }
    }
}