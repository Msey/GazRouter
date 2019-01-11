using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.PowerUnits;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.PowerUnits
{
    public class GetPowerUnitByIdQuery : QueryReader<Guid,PowerUnitDTO>
	{
        public GetPowerUnitByIdQuery(ExecutionContext context)
            : base(context)
		{
		}

        protected override string GetCommandText(Guid parameters)
		{
			return @"   SELECT      pu.power_unit_id,
                                    pu.power_unit_name,
                                    pu.power_unit_type_id,
                                    pu.operating_time_factor,
                                    pu.turbine_consumption,
                                    pu.turbine_runtime,  
                                    pu.kilometer,
                                    pu.system_id,
                                    pu.sort_order,
                                    pu.pipeline_id,
                                    pu.power_plant_id,
                                    pu.pipelinename,
                                    n.entity_name as path,
                                    n1.entity_name as short_path,
                                    pu.description
                        FROM        rd.v_power_units pu 
                        LEFT JOIN   v_nm_all n ON n.entity_id = pu.power_unit_id
                        LEFT JOIN   v_nm_short_all n1 ON pu.power_unit_id = n1.entity_id 
                        WHERE       pu.power_unit_id = :puid";
			
			
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
		{
            command.AddInputParameter("puid", parameters);
		}


        protected override PowerUnitDTO GetResult(OracleDataReader reader, Guid parameters)
		{
			
			if (reader.Read())
			{
			    var powerUnit = new PowerUnitDTO
			        {
                        Id = reader.GetValue<Guid>("power_unit_id"),
                        Name = reader.GetValue<string>("power_unit_name"),
                        PowerUnitTypeId = reader.GetValue<int>("power_unit_type_Id"),
                        OperatingTimeFactor = reader.GetValue<double>("operating_time_factor"),
                        TurbineConsumption = reader.GetValue<double>("turbine_consumption"),
                        TurbineRuntime = reader.GetValue<int>("turbine_runtime"),
                        Kilometr = reader.GetValue<double>("kilometer"),
                        SystemId = reader.GetValue<int>("system_id"),
			            SortOrder = reader.GetValue<int>("sort_order"),
                        Path = reader.GetValue<string>("path"),
                        ShortPath = reader.GetValue<string>("short_path"),
                        Description = reader.GetValue<string>("description")
			        };
				
                var pipelineId = reader.GetValue<Guid?>("pipeline_id");
                if (pipelineId.HasValue)
                {
                    powerUnit.ParentId = pipelineId.Value;
                    powerUnit.ParentEntityType = EntityType.Pipeline;
                }
                var compStationId = reader.GetValue<Guid?>("power_plant_id");
                if (compStationId.HasValue)
                {
                    powerUnit.ParentId = compStationId.Value;
                    powerUnit.ParentEntityType = EntityType.CompStation;
                }

                return powerUnit;
			}

		    return null;

		}
	}
}