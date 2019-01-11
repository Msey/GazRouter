using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.Pipelines;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Pipelines
{

    public class GetPipelineByIdQuery : QueryReader<Guid, PipelineDTO>
    {
        public GetPipelineByIdQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(Guid parameters)
        {
            return @"   SELECT      p.pipeline_id id, 
                                    p.entity_type_id, 
                                    status, 
                                    p.sort_order, 
                                    pipeline_name name, 
                                    is_virtual, 
                                    p.pipeline_type_id, 
                                    pt.pipeline_type_name, 
                                    c1.dest_entity_id begin_entity_id, 
                                    c1.kilometer begin_conn_kilometer, 
                                    c2.dest_entity_id end_entity_id, 
                                    c2.kilometer end_conn_kilometer,
                                    kilometer_start, 
                                    KILOMETER_END,
                                    p.system_id,
                                    p.sort_order, 
                                    n.entity_name full_name, 
                                    n1.entity_name short_name,
                                    p.description
                        FROM        rd.v_pipelines p 
                        JOIN        rd.v_pipeline_types pt ON pt.pipeline_type_id = p.pipeline_type_id
                        LEFT JOIN   v_nm_all n ON p.pipeline_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 ON p.pipeline_id = n1.entity_id
                        LEFT JOIN   v_pipeline_conns c1 ON c1.pipeline_id = p.pipeline_id AND c1.end_type_id = 21
                        LEFT JOIN   v_pipeline_conns c2 ON c2.pipeline_id = p.pipeline_id AND c2.end_type_id = 22
                        WHERE       p.pipeline_id = :id";
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override PipelineDTO GetResult(OracleDataReader reader, Guid parameters)
        {
            PipelineDTO pipeline = null;
            while (reader.Read())
            {
                pipeline = new PipelineDTO
                                  {
                                      Id = reader.GetValue<Guid>("ID"),
                                      Type = reader.GetValue<PipelineType>("PIPELINE_TYPE_ID"),
                                      TypeName = reader.GetValue<string>("PIPELINE_TYPE_NAME"),
                                      Name = reader.GetValue<string>("NAME"),
                                      Path = reader.GetValue<string>("full_name"),
                                      ShortPath = reader.GetValue<string>("short_name"),
                                      IsVirtual = reader.GetValue<bool>("IS_VIRTUAL"),
                                      BeginEntityId = reader.GetValue<Guid?>("begin_entity_id"),
                                      EndEntityId = reader.GetValue<Guid?>("end_entity_id"),
                                      KilometerOfBeginConn = reader.GetValue<double?>("begin_conn_kilometer"),
                                      KilometerOfEndConn = reader.GetValue<double?>("end_conn_kilometer"),
                                      KilometerOfStartPoint = reader.GetValue<double>("KILOMETER_START"),
                                      KilometerOfEndPoint = reader.GetValue<double>("KILOMETER_END"),
									  SystemId = reader.GetValue<int>("SYSTEM_ID"),
									  SortOrder = reader.GetValue<int>("Sort_order"),
                                      Description = reader.GetValue<string>("description")
                                  };
            }
            return pipeline;
        }
    }
}
