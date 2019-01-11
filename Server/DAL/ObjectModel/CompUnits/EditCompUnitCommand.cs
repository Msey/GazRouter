using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.CompUnits;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CompUnits
{
    public class EditCompUnitCommand : CommandNonQuery<EditCompUnitParameterSet>
    {
        public EditCompUnitCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditCompUnitParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_comp_unit_num", parameters.CompUnitNum);
            command.AddInputParameter("p_comp_shop_id", parameters.ParentId);
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_hidden", parameters.IsHidden);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("p_has_recovery_boiler", parameters.HasRecoveryBoiler);
            command.AddInputParameter("p_comp_unit_type_id", parameters.CompUnitTypeId);
            command.AddInputParameter("p_supercharger_type_id", parameters.SuperchargerTypeId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
            command.AddInputParameter("p_injection_profile_volume", parameters.InjectionProfileVolume);
            command.AddInputParameter("p_turbine_starter_consumption", parameters.TurbineStarterConsumption);
            command.AddInputParameter("p_bleeding_rate", parameters.BleedingRate);
            command.AddInputParameter("p_comp_unit_sealing_type_id", parameters.SealingType);
            command.AddInputParameter("p_comp_unit_sealing_count", parameters.SealingCount);
            command.AddInputParameter("p_dry_motoring_consumption", parameters.DryMotoringConsumption);
            command.AddInputParameter("p_start_valve_consumption", parameters.StartValveConsumption);
            command.AddInputParameter("p_stop_valve_consumption", parameters.StopValveConsumption);
            command.AddInputParameter("p_valve_consumption_details", parameters.ValveConsumptionDetails);
            command.AddInputParameter("p_injection_profile_piping", parameters.InjectionProfilePiping);
        }

        protected override string GetCommandText(EditCompUnitParameterSet parameters)
        {
            return "P_COMP_UNIT.Edit";
        }

    }
}