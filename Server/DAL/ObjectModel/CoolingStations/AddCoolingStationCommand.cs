using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.CoolingStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CoolingStations
{
    public class AddCoolingStationCommand : CommandScalar<AddCoolingStationParameterSet, Guid>
    {
        public AddCoolingStationCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddCoolingStationParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_comp_station_id", parameters.ParentId);
			command.AddInputParameter("P_SORT_ORDER", parameters.SortOrder);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddCoolingStationParameterSet parameters)
        {
            return "P_COOLING_STATION.AddF";
        }
    }
}