using GazRouter.DAL.Core;
using GazRouter.DTO.GasCosts;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.GasCosts
{
    public class SetDefaultParamValuesCommand : CommandNonQuery<DefaultParamValuesDTO>
    {
        public SetDefaultParamValuesCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override string GetCommandText(DefaultParamValuesDTO parameters)
        {
            return "rd.P_AUX_DEFAULT_DATA.Set_DATA";
        }

        protected override void BindParameters(OracleCommand command, DefaultParamValuesDTO parameters)
        {
            command.AddInputParameter("p_target_id", parameters.Target);
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_period", parameters.Period);

            command.AddInputParameter("p_p_air", parameters.PressureAir);
            command.AddInputParameter("p_density", parameters.Density);
            command.AddInputParameter("p_comb_heat", parameters.CombHeat);
            command.AddInputParameter("p_n_content", parameters.NitrogenContent);
            command.AddInputParameter("p_cd_content", parameters.CarbonDioxideContent);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
    }
}