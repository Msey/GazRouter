using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.Boilers;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Boilers
{
	public class GetBoilerListQuery : QueryReader<GetBoilerListParameterSet, List<BoilerDTO>>
	{
		public GetBoilerListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText(GetBoilerListParameterSet parameters)
		{
			var result = new StringBuilder(@"   SELECT      v.boiler_id,
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
                                                            v.description,
                                                            NVL(s.site_id, s1.site_id) AS site_id

                                                FROM        rd.V_boilers v
                                                LEFT JOIN   v_nm_short_all n1 ON v.boiler_id = n1.entity_id                                                
                                                LEFT JOIN   V_NM_ALL n on n.entity_id = v.boiler_id
                                                LEFT JOIN   v_segments_by_sites s 
                                                    ON      v.pipeline_id = s.pipeline_id
                                                    AND     v.kilometer >= s.kilometer_start 
                                                    AND     v.kilometer < s.kilometer_end
                                                LEFT JOIN   v_entity_2_site s1 ON s1.entity_id = v.boiler_id
                                                WHERE       1 = 1");
            if(parameters != null) { 
			    if (parameters.SystemId.HasValue) 
                    result.Append(" AND v.SYSTEM_ID = :systemId");

		        if (parameters.SiteId.HasValue)
                    result.Append(" AND NVL(s.site_id, s1.site_id) = :siteId");
            }

            result.Append(" order by v.sort_order");
			return result.ToString();
		}

		protected override void BindParameters(OracleCommand command, GetBoilerListParameterSet parameters)
		{
		    if (parameters != null)
		    {
		        if (parameters.SystemId.HasValue)
		            command.AddInputParameter("systemId", parameters.SystemId.Value);
		        if (parameters.SiteId.HasValue)
		            command.AddInputParameter("siteId", parameters.SiteId.Value);
		    }
		}
        protected override List<BoilerDTO> GetResult(OracleDataReader reader, GetBoilerListParameterSet parameters)
		{
			var result = new List<BoilerDTO>();
			while (reader.Read())
			{
			    var boiler = new BoilerDTO
			    {
			        Id = reader.GetValue<Guid>("boiler_id"),
			        Name = reader.GetValue<string>("boiler_name"),
			        BoilerTypeId = reader.GetValue<int>("boiler_type_id"),
			        Kilometr = reader.GetValue<double>("kilometer"),
			        SystemId = reader.GetValue<int>("system_id"),
			        SortOrder = reader.GetValue<int>("sort_order"),
			        HeatLossFactor = reader.GetValue<double>("heat_loss_factor"),
                    HeatSupplySystemLoad = reader.GetValue<double>("heat_supply_system_load"),
			        ShortPath = reader.GetValue<string>("short_name"),
                    Path = reader.GetValue<string>("full_name"),
                    Description = reader.GetValue<string>("description"),
                    SiteId = reader.GetValue<Guid?>("site_id")
			    };
                result.Add(boiler);

                var pipelineId = reader.GetValue<Guid?>("pipeline_id");
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
                var distrStationId = reader.GetValue<Guid?>("distr_station_id");
                if (distrStationId.HasValue)
                {
                    boiler.ParentId = distrStationId.Value;
                    boiler.ParentEntityType = EntityType.DistrStation;
                }
                var measStationId = reader.GetValue<Guid?>("meas_station_id");
                if (measStationId.HasValue)
                {
                    boiler.ParentId = measStationId.Value;
                    boiler.ParentEntityType = EntityType.MeasStation;
                }

			}
			return result;
		}
	}
}