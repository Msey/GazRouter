using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.CompStationCoolingRecomended;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CompStationCoolingRecomended
{
	public class SetCompStationCoolingRecomendedCommand : CommandNonQuery<SetCompStationCoolingRecomendedParameterSet>
    {
		public SetCompStationCoolingRecomendedCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, SetCompStationCoolingRecomendedParameterSet parameters)
        {
			command.AddInputParameter("p_comp_station_id", parameters.CompStationId);
			command.AddInputParameter("p_month", parameters.Month);
			command.AddInputParameter("p_temperature", parameters.Temperature);
			command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SetCompStationCoolingRecomendedParameterSet parameters)
        {
			return "rd.P_T_COOLING_RECOMENDED.Set_T";
        }
    }
}