using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.PowerUnits;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.PowerUnits
{
    public class EditPowerUnitCommand : CommandNonQuery<EditPowerUnitParameterSet>
    {
        public EditPowerUnitCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, EditPowerUnitParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.Id);
			command.AddInputParameter("p_entity_name", parameters.Name);
			command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("p_power_unit_type_id", parameters.PowerUnitTypeId);
            command.AddInputParameter("p_operating_time_factor", parameters.OperatingTimeFactor);
            command.AddInputParameter("p_turbine_consumption", parameters.TurbineConsumption);
            command.AddInputParameter("p_turbine_runtime", parameters.TurbineRuntime);
            command.AddInputParameter("p_kilometer", parameters.Kilometer);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(EditPowerUnitParameterSet parameters)
        {
			return "rd.p_Power_Unit.Edit";
        }

    }
}