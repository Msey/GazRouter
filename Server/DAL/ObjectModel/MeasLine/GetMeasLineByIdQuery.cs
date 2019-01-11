using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.MeasLine;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.MeasLine
{

    public class GetMeasLineByIdQuery : QueryReader<Guid, MeasLineDTO>
    {
        public GetMeasLineByIdQuery(ExecutionContext context)
            : base(context)
        {}

		protected override string GetCommandText(Guid parameters)
		{
            return @"   SELECT      ml.meas_line_id, 
                                    ml.entity_type_id, 
                                    ml.status, 
                                    ml.sort_order, 
                                    ml.meas_line_name, 
                                    ml.is_virtual, 
                                    ml.meas_station_id, 
                                    ml.pipeline_id, 
                                    ml.pipelinename, 
                                    ml.kilometer_conn, 
                                    n.entity_name full_name, 
                                    n1.entity_name short_name, 
                                    ml.system_id,
                                    ml.description,
                                    ml.bal_name
                                    
                        FROM        rd.v_meas_lines ml
                        LEFT JOIN   v_nm_all n on ml.meas_line_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 on ml.meas_line_id = n1.entity_id
                        WHERE       ml.meas_line_id = :id";
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
		{
            command.AddInputParameter("id", parameters);
		}

        protected override MeasLineDTO GetResult(OracleDataReader reader, Guid parameters)
        {
            MeasLineDTO measLines = null;
            while (reader.Read())
            {
                measLines = new MeasLineDTO
                {
                    Id = reader.GetValue<Guid>("meas_line_id"),
                    Name = reader.GetValue<string>("meas_line_name"),
                    IsVirtual = reader.GetValue<bool>("is_virtual"),
                    ParentId = reader.GetValue<Guid>("meas_station_id"),
				    PipelineName = reader.GetValue<string>("pipelinename"),
				    PipelineId = reader.GetValue<Guid>("pipeline_id"),
                    KmOfConn = reader.GetValue<double>("kilometer_conn"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name")  ,
				    SystemId = reader.GetValue<int>("system_id"),
                    Description = reader.GetValue<string>("description"),
                    BalanceName = reader.GetValue<string>("bal_name")
                };
            }
            return measLines;
        }
    }
}
