using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.BoilerPlants
{
    public class AddBoilerPlantCommand : CommandScalar<AddBoilerPlantParameterSet, Guid>
    {
        public AddBoilerPlantCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddBoilerPlantParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_comp_station_id", parameters.ParentId);
            command.AddInputParameter("p_entity_name", parameters.Name);
			command.AddInputParameter("P_SORT_ORDER", parameters.SortOrder);
            
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddBoilerPlantParameterSet parameters)
        {
            return "P_BOILER_PLANT.AddF";
        }
    }
}