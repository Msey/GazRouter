using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.CoolingStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CoolingStations
{
    public class EditCoolingStationCommand : CommandNonQuery<EditCoolingStationParameterSet>
    {
        public EditCoolingStationCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditCoolingStationParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditCoolingStationParameterSet parameters)
        {
            return @"P_COOLING_STATION.Edit";
        }
    }
}