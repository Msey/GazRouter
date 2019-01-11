using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Routes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Routes
{
    public class GetSectionListQuery : QueryReader<List<int>, List<RouteSectionDTO>>
	{
        public GetSectionListQuery(ExecutionContext context)
			: base(context)
		{ }

        protected override List<RouteSectionDTO> GetResult(OracleDataReader reader, List<int> parameters)
		{
            var routes = new List<RouteSectionDTO>();
            while (reader.Read())
            {
                routes.Add(new RouteSectionDTO
                {
                    RouteId = reader.GetValue<int>("route_id"),
                    RouteSectionId = reader.GetValue<int>("route_section_id"),
                    PipelineId = reader.GetValue<Guid>("pipeline_id"),
                    PipelineName = reader.GetValue<string>("pipeline_name"),
                    KilometerStart = reader.GetValue<double>("kilometer_start"),
                    KilometerEnd = reader.GetValue<double>("kilometer_end")
                });
            }
            return routes;
		}

        protected override string GetCommandText(List<int> parameters)
        {
            var q = @"  SELECT      s.route_section_id, 
                                    s.route_id, 
                                    s.pipeline_id, 
                                    p.pipeline_name,
                                    s.kilometer_start, 
                                    s.kilometer_end
                        FROM        v_bl_route_sections s
                        INNER JOIN  v_pipelines p ON p.pipeline_id = s.pipeline_id
                        WHERE       route_id IN {0}
                        ORDER BY    s.sort_order ASC";


            q = string.Format(q, CreateInClause(parameters.Count));
            return q;
        }

        protected override void BindParameters(OracleCommand command, List<int> parameters)
        {
            for (var i = 0; i < parameters.Count; i++)
            {
                command.AddInputParameter(string.Format("p{0}", i), parameters[i]);
            }
            
        }
	}
}
