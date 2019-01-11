using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.Boilers;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Boilers
{
	public class GetBoilerByIdQuery : QueryReader<Guid,BoilerDTO>
	{
        public GetBoilerByIdQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(Guid parameters)
		{
			return @"   SELECT      v.boiler_id,
                                    v.boiler_name,
                                    v.boiler_type_id,
                                    v.kilometer,
                                    v.system_id,
                                    v.sort_order,
                                    v.pipeline_id,
                                    v.boiler_plant_id,
                                    v.distr_station_id,
                                    v.meas_station_id,
                                    v.heat_loss_factor,
                                    v.heat_supply_system_load,
                                    n1.entity_name AS short_name,
                                    n.entity_name as full_name,
                                    v.description
                        FROM        rd.V_boilers v
                        LEFT JOIN   v_nm_short_all n1 ON v.boiler_id = n1.entity_id
                        left join   V_NM_ALL n ON n.entity_id = v.boiler_id
                        WHERE       v.boiler_id = :boilerid";
}

        protected override void BindParameters(OracleCommand command, Guid parameters)
		{
			command.AddInputParameter("boilerid", parameters);
		}


        protected override BoilerDTO GetResult(OracleDataReader reader, Guid parameters)
		{
		    BoilerDTO boiler = null;
			if (reader.Read())
			{
                boiler = new BoilerDTO
			    {
			        Id = reader.GetValue<Guid>("BOILER_ID"),
			        Name = reader.GetValue<string>("BOILER_NAME"),
			        BoilerTypeId = reader.GetValue<int>("BOILER_TYPE_ID"),
			        Kilometr = reader.GetValue<double>("KILOMETER"),
			        SystemId = reader.GetValue<int>("SYSTEM_ID"),
			        SortOrder = reader.GetValue<int>("Sort_Order"),
                    ShortPath = reader.GetValue<string>("short_name"),
                    Path = reader.GetValue<string>("full_name"),
                    HeatLossFactor = reader.GetValue<double>("heat_loss_factor"),
                    HeatSupplySystemLoad = reader.GetValue<double>("heat_supply_system_load"),
                    Description = reader.GetValue<string>("description")
			    };

                var pipelineId = reader.GetValue<Guid?>("PIPELINE_ID");
                if (pipelineId.HasValue)
                {
                    boiler.ParentId = pipelineId.Value;
                    boiler.ParentEntityType = EntityType.Pipeline;
            }
                var boilerPlantId = reader.GetValue<Guid?>("boiler_plant_id");
                if (boilerPlantId.HasValue)
                {
                    boiler.ParentId = boilerPlantId.Value;
                    boiler.ParentEntityType = EntityType.BoilerPlant;
                }
                var distrStationId = reader.GetValue<Guid?>("DISTR_STATION_ID");
                if (distrStationId.HasValue)
                {
                    boiler.ParentId = distrStationId.Value;
                    boiler.ParentEntityType = EntityType.DistrStation;
                }
                var measStationId = reader.GetValue<Guid?>("MEAS_STATION_ID");
                if (measStationId.HasValue)
                {
                    boiler.ParentId = measStationId.Value;
                    boiler.ParentEntityType = EntityType.MeasStation;
                }
            }
            return boiler;
		}
	}
}