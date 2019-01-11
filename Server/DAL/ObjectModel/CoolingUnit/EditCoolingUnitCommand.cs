using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CoolingUnit
{
    public class EditCoolingUnitCommand : CommandNonQuery<EditCoolingUnitParameterSet>
    {
		public EditCoolingUnitCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, EditCoolingUnitParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
			command.AddInputParameter("P_COOLING_UNIT_TYPE_ID", parameters.CoolintUnitType);
        }

		protected override string GetCommandText(EditCoolingUnitParameterSet parameters)
        {
			return @"p_Cooling_Unit.Edit";
        }
    }
}