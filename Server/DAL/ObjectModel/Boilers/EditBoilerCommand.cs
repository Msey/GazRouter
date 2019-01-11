using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Boilers;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Boilers
{
	public class EditBoilerCommand : CommandNonQuery<EditBoilerParameterSet>
    {
		public EditBoilerCommand(ExecutionContext context)
			: base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, EditBoilerParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.Id);
			command.AddInputParameter("p_entity_name", parameters.Name);
			command.AddInputParameter("p_sort_order", parameters.SortOrder);
			command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
			command.AddInputParameter("p_boiler_type_id", parameters.BoilerTypeId);
            command.AddInputParameter("p_heat_loss_factor", parameters.HeatLossFactor);
            command.AddInputParameter("p_heat_supply_system_load", parameters.HeatSupplySystemLoad);
			command.AddInputParameter("p_kilometer", parameters.Kilometer);
			command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(EditBoilerParameterSet parameters)
        {
			return "rd.p_boiler.Edit";
        }

    }
}