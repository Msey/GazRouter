using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.CompUnits;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CompUnits
{
    public class AddCompUnitCommand : CommandScalar<AddCompUnitParameterSet, Guid>
    {
        public AddCompUnitCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddCompUnitParameterSet parameters)
        {
            if (parameters.Id.HasValue)
            {
                command.AddInputParameter("p_entity_id", parameters.Id.Value);
            }

            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_comp_unit_num", parameters.CompUnitNum);
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_comp_shop_id", parameters.ParentId);
            command.AddInputParameter("p_hidden", parameters.IsHidden);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("p_has_recovery_boiler", parameters.HasRecoveryBoiler);
            command.AddInputParameter("p_comp_unit_type_id", parameters.CompUnitTypeId);
            command.AddInputParameter("p_supercharger_type_id", parameters.SuperchargerTypeId);
            command.AddInputParameter("p_injection_profile_volume", parameters.InjectionProfileVolume);
            command.AddInputParameter("p_turbine_starter_consumption", parameters.TurbineStarterConsumption);
            command.AddInputParameter("p_dry_motoring_consumption", parameters.DryMotoringConsumption);
            command.AddInputParameter("p_bleeding_rate", parameters.BleedingRate);
            command.AddInputParameter("p_comp_unit_sealing_type_id", parameters.SealingType);
            command.AddInputParameter("p_comp_unit_sealing_count", parameters.SealingCount);
            command.AddInputParameter("p_start_valve_consumption", parameters.StartValveConsumption);
            command.AddInputParameter("p_stop_valve_consumption", parameters.StopValveConsumption);
            command.AddInputParameter("p_valve_consumption_details", parameters.ValveConsumptionDetails);
            command.AddInputParameter("p_injection_profile_piping", parameters.InjectionProfilePiping);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddCompUnitParameterSet parameters)
        {
            return "P_COMP_UNIT.AddF";
        }

    }
    
}