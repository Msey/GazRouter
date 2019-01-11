using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.CoolingStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CoolingStations
{
    public class GetCoolingStationByIdQuery : QueryReader<Guid, CoolingStationDTO>
    {
        public GetCoolingStationByIdQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(Guid parameters)
        {
            return @"   SELECT      cs.cooling_station_id, 
                                    cs.cooling_station_name, 
                                    cs.comp_station_id,
                                    cs.sort_order,
                                    cs.is_virtual,
                                    cs.system_id,
                                    cs.description
                        FROM        v_cooling_stations cs
                        LEFT JOIN   v_nm_all n ON cs.cooling_station_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 ON cs.cooling_station_id = n1.entity_id
                        WHERE       cs.cooling_station_id = :id";
        }


        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }


        protected override CoolingStationDTO GetResult(OracleDataReader reader, Guid parameters)
        {
			
            if (reader.Read())
            {
                return new CoolingStationDTO
                {
                    Id = reader.GetValue<Guid>("cooling_station_id"),
                    Name = reader.GetValue<string>("cooling_station_name"),
                    ParentId = reader.GetValue<Guid>("comp_station_id"),
                    SortOrder = reader.GetValue<int>("sort_order"),
                    IsVirtual = reader.GetValue<bool>("is_virtual"),
                    SystemId = reader.GetValue<int>("system_id"),
                    Description = reader.GetValue<string>("description")
                };
            }
            return null;
        }
    }
}