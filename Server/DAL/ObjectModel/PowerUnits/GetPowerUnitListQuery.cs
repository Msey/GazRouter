using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.PowerUnits;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.PowerUnits
{
    public class GetPowerUnitListQuery : QueryReader<GetPowerUnitListParameterSet, List<PowerUnitDTO>>
	{
		public GetPowerUnitListQuery(ExecutionContext context) : base(context)
		{
		}

		protected override string GetCommandText(GetPowerUnitListParameterSet parameters)
		{
			var result = new StringBuilder(@"   SELECT      pu.power_unit_id,
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
                                                            pu.description,
                                                            NVL(s.site_id, s1.site_id) AS site_id
                                                FROM        rd.v_power_units pu 
                                                LEFT JOIN   v_nm_all n ON n.entity_id = pu.power_unit_id
                                                LEFT JOIN   v_nm_short_all n1 ON pu.power_unit_id = n1.entity_id
                                                LEFT JOIN   v_segments_by_sites s 
                                                    ON      pu.pipeline_id = s.pipeline_id
                                                    AND     pu.kilometer >= s.kilometer_start 
                                                    AND     pu.kilometer < s.kilometer_end
                                                LEFT JOIN   v_entity_2_site s1 ON s1.entity_id = pu.power_unit_id
                                                WHERE       1 = 1 ");
            if(parameters != null) { 
			    if (parameters.SystemId.HasValue) 
                    result.Append(" AND SYSTEM_ID = :systemId");

                if (parameters.SiteId.HasValue)
                    result.Append(" AND NVL(s.site_id, s1.site_id) = :siteId");
            }
            result.Append(" ORDER BY sort_order");
			return result.ToString();
		}
		protected override void BindParameters(OracleCommand command, GetPowerUnitListParameterSet parameters)
		{
		    if (parameters != null)
		    {
		        if (parameters.SystemId.HasValue)
		            command.AddInputParameter("systemId", parameters.SystemId.Value);

		        if (parameters.SiteId.HasValue)
		            command.AddInputParameter("siteId", parameters.SiteId.Value);
		    }
		}

        protected override List<PowerUnitDTO> GetResult(OracleDataReader reader, GetPowerUnitListParameterSet parameters)
		{
			var result = new List<PowerUnitDTO>();
			while (reader.Read())
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
                    Description = reader.GetValue<string>("description"),
                    SiteId = reader.GetValue<Guid?>("site_id")
			    };
				result.Add(powerUnit);

                var pipelineId = reader.GetValue<Guid?>("pipeline_id");
                if (pipelineId.HasValue)
                {
                    powerUnit.ParentId = pipelineId.Value;
                    powerUnit.ParentEntityType = EntityType.Pipeline;
                }
                var powerPlantId = reader.GetValue<Guid?>("power_plant_id");
                if (powerPlantId.HasValue)
                {
                    powerUnit.ParentId = powerPlantId.Value;
                    powerUnit.ParentEntityType = EntityType.CompStation;
                }

			}
			return result;
		}
	}
}