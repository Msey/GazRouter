using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.ReducingStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.ReducingStations
{
    public class GetReducingStationListQuery : QueryReader<GetReducingStationListParameterSet, List<ReducingStationDTO>>
	{
		public GetReducingStationListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(GetReducingStationListParameterSet parameters)
        {
            var sql = new StringBuilder(@"  SELECT  rs.reducing_station_id, 
                                                    rs.reducing_station_name, 
                                                    rs.site_id, 
                                                    rs.pipeline_id,
                                                    rs.pipeline_name,
                                                    rs.kilometer, 
                                                    n.entity_name AS full_name, 
                                                    n1.entity_name AS short_name,
                                                    rs.system_id,
                                                    rs.sort_order,
                                                    rs.description
                                            FROM rd.v_reducing_stations rs
                                            LEFT JOIN v_nm_all n ON rs.reducing_station_id = n.entity_id
                                            LEFT JOIN v_nm_short_all n1 ON rs.reducing_station_id = n1.entity_id
                                            WHERE 1=1");
            if (parameters != null)
            {
                if (parameters.Id.HasValue)
                    sql.Append(" AND rs.reducing_station_id = :id");

                if (parameters.SiteId.HasValue)
                    sql.Append(" AND rs.site_id = :siteId");

                if (parameters.SystemId.HasValue)
                    sql.Append(" AND rs.system_id = :systemId");

                if (parameters.PipelineId.HasValue)
                    sql.Append(" AND rs.pipeline_id = :pipeId");
            }
            sql.Append(@" ORDER BY rs.sort_order, rs.reducing_station_name");

            return sql.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetReducingStationListParameterSet parameters)
        {
            if (parameters != null)
            {
                if (parameters.Id.HasValue)
                    command.AddInputParameter("Id", parameters.Id);

                if (parameters.SiteId.HasValue)
                    command.AddInputParameter("siteId", parameters.SiteId);
                
                if (parameters.SystemId.HasValue)
                    command.AddInputParameter("systemId", parameters.SystemId);

                if (parameters.PipelineId.HasValue)
                    command.AddInputParameter("pipeId", parameters.PipelineId);
            }
        }

        protected override List<ReducingStationDTO> GetResult(OracleDataReader reader, GetReducingStationListParameterSet parameters)
		{
			var result = new List<ReducingStationDTO>();
			while (reader.Read())
			{
				result.Add(new ReducingStationDTO
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
                    SortOrder = reader.GetValue<int>("sort_order"),
                    Description = reader.GetValue<string>("description")
				});

			}
			return result;
		}
	}
}
