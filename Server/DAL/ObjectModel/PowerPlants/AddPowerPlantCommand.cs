using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.PowerPlants;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.PowerPlants
{
    public class AddPowerPlantCommand : CommandScalar<AddPowerPlantParameterSet, Guid>
    {
        public AddPowerPlantCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddPowerPlantParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_comp_station_id", parameters.ParentId);
            command.AddInputParameter("p_entity_name", parameters.Name);
			command.AddInputParameter("P_SORT_ORDER", parameters.SortOrder);
            
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddPowerPlantParameterSet parameters)
        {
            return "P_POWER_PLANT.AddF";
        }
    }
}