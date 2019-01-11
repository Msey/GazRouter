using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.BoilerPlants
{
    public class EditBoilerPlantCommand : CommandNonQuery<EditBoilerPlantParameterSet>
    {
        public EditBoilerPlantCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditBoilerPlantParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_entity_name", parameters.Name);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditBoilerPlantParameterSet parameters)
        {
            return @"P_BOILER_PLANT.Edit";
        }
    }
}