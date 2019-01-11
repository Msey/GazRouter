using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.CoolingStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CoolingStations
{
    public class GetCoolingStationsListQuery : QueryReader<GetCoolingStationListParameterSet, List<CoolingStationDTO>>
    {
        public GetCoolingStationsListQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(GetCoolingStationListParameterSet parameters)
        {
            var q = @"  SELECT      cs.cooling_station_id, 
                                    cs.cooling_station_name, 
                                    cs.comp_station_id,
                                    cs.sort_order,
                                    cs.entity_type_id,
                                    cs.is_virtual,
                                    cs.system_ID,
                                    cs.description                                                
                        FROM        v_cooling_stations cs
                        INNER JOIN  v_comp_stations comp ON comp.comp_station_id = cs.comp_station_id 
                        LEFT JOIN   v_nm_all n ON cs.cooling_station_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 ON cs.cooling_station_id = n1.entity_id
                        WHERE       1=1";

            var sb = new StringBuilder(q);
            if (parameters.SystemId.HasValue) sb.Append(" AND cs.system_id = :systemId");
            if (parameters.SiteId.HasValue) sb.Append(" AND comp.site_id = :siteId");
            sb.Append(" ORDER BY cs.sort_order, cs.cooling_station_name");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetCoolingStationListParameterSet parameters)
		{
            if (parameters.SiteId.HasValue)
               command.AddInputParameter("siteId", parameters.SiteId.Value);
            if (parameters.SystemId.HasValue)
                command.AddInputParameter("systemId", parameters.SystemId.Value);
            
		}

        protected override List<CoolingStationDTO> GetResult(OracleDataReader reader, GetCoolingStationListParameterSet parameters)
        {
            var result = new List<CoolingStationDTO>();
            while (reader.Read())
            {
                result.Add(new CoolingStationDTO
                {
                    Id = reader.GetValue<Guid>("cooling_station_id"),
                    Name = reader.GetValue<string>("cooling_station_name"),
                    EntityType = reader.GetValue<EntityType>("entity_type_id"),
                    ParentId = reader.GetValue<Guid>("comp_station_id"),
                    SortOrder = reader.GetValue<int>("sort_order"),
                    IsVirtual = reader.GetValue<bool>("is_virtual"),
                    SystemId = reader.GetValue<int>("SYSTEM_ID"),
                    Description = reader.GetValue<string>("description")
                });
            }
            return result;
        }
    }
}