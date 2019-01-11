using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.ReducingStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.ReducingStations
{
    public class GetReducingStationByIdQuery : QueryReader<Guid, ReducingStationDTO>
	{
        public GetReducingStationByIdQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(Guid parameters)
		{
            return @"   SELECT  rs.reducing_station_id, 
                                rs.reducing_station_name, 
                                rs.site_id, 
                                rs.pipeline_id,
                                rs.pipeline_name, 
                                rs.kilometer, 
                                n.entity_name AS full_name, 
                                n1.entity_name AS short_name,
                                rs.system_id,
                                rs.description
                        FROM rd.v_reducing_stations rs
                        LEFT JOIN v_nm_all n ON rs.reducing_station_id = n.entity_id
                        LEFT JOIN v_nm_short_all n1 ON rs.reducing_station_id = n1.entity_id 
                        WHERE rs.reducing_station_id = :id";
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override ReducingStationDTO GetResult(OracleDataReader reader, Guid parameters)
		{
            ReducingStationDTO result = null;
			while (reader.Read())
			{
				result = new ReducingStationDTO
				{
                    Id = reader.GetValue<Guid>("reducing_station_id"),
                    Name = reader.GetValue<string>("reducing_station_name"),
                    ParentId = reader.GetValue<Guid>("site_id"),
                    PipelineId = reader.GetValue<Guid>("pipeline_id"),
                    PipelineName = reader.GetValue<string>("pipeline_name"),
                    Kilometer = reader.GetValue<double>("kilometer"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name"),
                    SystemId = reader.GetValue<int>("system_id"),
                    Description = reader.GetValue<string>("description")
				};

			}
			return result;
		}
	}
}
