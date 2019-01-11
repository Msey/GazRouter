using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.Pipelines;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Pipelines
{

    public class GetPipelineListQuery : QueryReader<GetPipelineListParameterSet, List<PipelineDTO>>
    {
        public GetPipelineListQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(GetPipelineListParameterSet parameters)
        {
            var sb = new StringBuilder(@"   SELECT      p.pipeline_id id, 
                                                        p.entity_type_id, 
                                                        status, 
                                                        p.sort_order, 
                                                        pipeline_name AS name, 
                                                        is_virtual, 
                                                        p.pipeline_type_id, 
                                                        pt.pipeline_type_name, 
                                                        c1.dest_entity_id AS begin_entity_id, 
                                                        c1.kilometer AS begin_conn_kilometer, 
                                                        c2.dest_entity_id AS end_entity_id, 
                                                        c2.kilometer AS end_conn_kilometer,
                                                        kilometer_start, 
                                                        kilometer_end,
                                                        p.system_id,
                                                        p.sort_order, 
                                                        n.entity_name AS full_name, 
                                                        n1.entity_name AS short_name,
                                                        p.description
                                            FROM        rd.v_pipelines p 
                                            JOIN        rd.v_pipeline_types pt on pt.pipeline_type_id = p.pipeline_type_id
                                            LEFT JOIN   v_nm_all n on p.pipeline_id = n.entity_id
                                            LEFT JOIN   v_nm_short_all n1 on p.pipeline_id = n1.entity_id
                                            LEFT JOIN   v_pipeline_conns c1 on c1.pipeline_id = p.pipeline_id and c1.end_type_id = 21
                                            LEFT JOIN   v_pipeline_conns c2 on c2.pipeline_id = p.pipeline_id and c2.end_type_id = 22
                                            WHERE       1=1");
            if (parameters != null)
            {
                if (parameters.SystemId.HasValue)
                    sb.Append(" AND p.SYSTEM_ID  = :systemId");

                if (parameters.SiteId.HasValue)
                    sb.Append(" AND p.Pipeline_Id IN (SELECT sbs.pipeline_id FROM v_segments_by_sites sbs WHERE sbs.site_id = :siteId)");

                if (parameters.PipelineTypes != null && parameters.PipelineTypes.Any())
                {
                    sb.Append(" AND p.pipeline_type_id IN ");
                    sb.Append(CreateInClause(parameters.PipelineTypes.Count));
                }
                    
            }
            sb.Append(" ORDER BY p.sort_order, p.pipeline_name");                                                                 
                                                                                                 
            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetPipelineListParameterSet parameters)
        {
            if (parameters == null) return;
            
            if (parameters.SystemId.HasValue)
                command.AddInputParameter("systemId", parameters.SystemId);

            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteId", parameters.SiteId);

            if (parameters.PipelineTypes != null && parameters.PipelineTypes.Any())
                for(var i = 0; i < parameters.PipelineTypes.Count; i++)
                    command.AddInputParameter(string.Format("p{0}", i), parameters.PipelineTypes[i]);
            
        }

        protected override List<PipelineDTO> GetResult(OracleDataReader reader, GetPipelineListParameterSet parameters)
        {
            var pipelines   = new List<PipelineDTO>();
            while (reader.Read())
            {
                pipelines.Add(
                    new PipelineDTO
                    {
                        Id = reader.GetValue<Guid>("id"),
                        Type = reader.GetValue<PipelineType>("pipeline_type_id"),
                        TypeName = reader.GetValue<string>("pipeline_type_name"),
                        Name = reader.GetValue<string>("name"),
                        Path = reader.GetValue<string>("full_name"),
                        ShortPath = reader.GetValue<string>("short_name"),
                        IsVirtual = reader.GetValue<bool>("is_virtual"),
                        BeginEntityId = reader.GetValue<Guid?>("begin_entity_id"),
                        EndEntityId = reader.GetValue<Guid?>("end_entity_id"),
                        KilometerOfBeginConn = reader.GetValue<double?>("begin_conn_kilometer"),
                        KilometerOfEndConn = reader.GetValue<double?>("end_conn_kilometer"),
                        KilometerOfStartPoint = reader.GetValue<double>("kilometer_start"),
                        KilometerOfEndPoint = reader.GetValue<double>("KILOMETER_END"),
						SystemId = reader.GetValue<int>("system_id"),
						SortOrder = reader.GetValue<int?>("sort_order")??Int32.MaxValue,
                        Description = reader.GetValue<string>("description")
                    });
            }
            return pipelines;
        }
    }
}
