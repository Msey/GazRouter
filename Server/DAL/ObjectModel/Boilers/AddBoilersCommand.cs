using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Boilers;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Boilers
{
	public class AddBoilerCommand : CommandScalar<AddBoilerParameterSet, Guid>
    {
        public AddBoilerCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, AddBoilerParameterSet parameters)
        {
			OutputParameter = command.AddReturnParameter<Guid>("p_entity_id");
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("p_boiler_type_id", parameters.BoilerTypeId);
            command.AddInputParameter("p_heat_loss_factor", parameters.HeatLossFactor);
            command.AddInputParameter("p_heat_supply_system_load", parameters.HeatSupplySystemLoad);
            if (parameters.PipelineId.HasValue)
                command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            if (parameters.BoilerPlantId.HasValue)
                command.AddInputParameter("p_boiler_plant_id", parameters.BoilerPlantId);
            if (parameters.DistStationId.HasValue)
                command.AddInputParameter("p_distr_station_id", parameters.DistStationId);
            if (parameters.MeasStationId.HasValue)
                command.AddInputParameter("p_meas_station_id", parameters.MeasStationId);
            command.AddInputParameter("p_kilometer", parameters.Kilometer);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(AddBoilerParameterSet parameters)
        {
			return "rd.p_boiler.AddF";
        }

    }
    
}