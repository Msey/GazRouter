using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.PowerPlants;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.PowerPlants
{
    public class EditPowerPlantCommand : CommandNonQuery<EditPowerPlantParameterSet>
    {
        public EditPowerPlantCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditPowerPlantParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_entity_name", parameters.Name);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditPowerPlantParameterSet parameters)
        {
            return @"P_POWER_PLANT.Edit";
        }
    }
}