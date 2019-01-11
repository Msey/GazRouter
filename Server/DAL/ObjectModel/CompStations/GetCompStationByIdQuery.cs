using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.CompStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CompStations
{
    public class GetCompStationByIdQuery : QueryReader<Guid, CompStationDTO>
	{
        public GetCompStationByIdQuery(ExecutionContext context)
            : base(context)
		{
		}
                
        protected override string GetCommandText(Guid parameters)
        {
            return @"   SELECT      cs.comp_station_id, 
                                    cs.comp_station_name, 
                                    cs.status, cs.site_id, 
                                    cs.region_id, 
                                    cs.use_in_balance,
                                    n.entity_name full_name, 
                                    n1.entity_name short_name,
                                    cs.system_id,
                                    cs.sort_order,
                                    cs.description
                        FROM        rd.v_comp_stations cs
                        LEFT JOIN   v_nm_all n ON cs.comp_station_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 on cs.comp_station_id = n1.entity_id
                        WHERE       cs.comp_station_id = :id";
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override CompStationDTO GetResult(OracleDataReader reader, Guid parameters)
		{
            CompStationDTO result = null;
			while (reader.Read())
			{
				result = new CompStationDTO
                {
                    Id = reader.GetValue<Guid>("COMP_STATION_ID"),
                    Name = reader.GetValue<string>("COMP_STATION_NAME"),
                    ParentId = reader.GetValue<Guid>("SITE_ID"),
                    RegionId = reader.GetValue<int>("REGION_ID"),
                    UseInBalance = reader.GetValue<bool>("use_in_balance"),
                    Status = reader.GetValue<EntityStatus?>("STATUS"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name"),
					SystemId = reader.GetValue<int>("SYSTEM_ID"),
                    Description = reader.GetValue<string>("description")
				};
			}
			return result;
		}
	}
}