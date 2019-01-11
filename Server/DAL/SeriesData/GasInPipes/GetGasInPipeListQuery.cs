using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.SeriesData.GasInPipes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.GasInPipes
{
    public class GetGasInPipeListQuery : QueryReader<GetGasInPipeListParameterSet, List<GasInPipeDTO>>
    {
        public GetGasInPipeListQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(GetGasInPipeListParameterSet parameter)
        {
            if (parameter == null) 
                throw new Exception("parameters должен быть задан");

            var q = @"   SELECT     s.key_date,
                                    v.pipeline_id,
                                    v.kilometer_start,
                                    v.kilometer_end,
                                    v.gaz_volume,
                                    v.gaz_volume_change
                        FROM        v_gas_supply  v
                        INNER JOIN  v_pipelines p ON p.pipeline_id = v.pipeline_id 
                        INNER JOIN  v_value_series s ON s.series_id = v.series_id
                        WHERE       1 = 1
                            AND     s.period_type_id  = 5
                            AND     s.key_date  BETWEEN :begin AND :end";

            var sb = new StringBuilder(q);

            if (parameter.SystemId.HasValue)
                sb.Append(" AND p.system_id = :sys");

            if (parameter.PipelineId.HasValue)
                sb.Append(" AND v.pipeline_id = :pipe");


            if (parameter.KmBegin.HasValue)
                sb.Append(" AND v.kilometer_start >= :kmb");

            if (parameter.KmBegin.HasValue)
                sb.Append(" AND v.kilometer_end <= :kme");


            sb.Append(" ORDER BY v.kilometer_start ASC");
            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetGasInPipeListParameterSet parameters)
        {
            command.AddInputParameter("begin", parameters.BeginDate);
            command.AddInputParameter("end", parameters.EndDate);

            if (parameters.SystemId.HasValue)
                command.AddInputParameter("sys", parameters.SystemId);

            if (parameters.PipelineId.HasValue)
                command.AddInputParameter("pipe", parameters.PipelineId);

            if (parameters.KmBegin.HasValue)
                command.AddInputParameter("kmb", parameters.KmBegin);

            if (parameters.KmEnd.HasValue)
                command.AddInputParameter("kme", parameters.KmEnd);
        }

        protected override List<GasInPipeDTO> GetResult(OracleDataReader reader, GetGasInPipeListParameterSet parameters)
        {
            var result = new List<GasInPipeDTO>();
            while (reader.Read())
            {
                var dto = new GasInPipeDTO
                {
                    Timestamp = reader.GetValue<DateTime>("key_date"),
                    PipelineId = reader.GetValue<Guid>("pipeline_id"),
                    KmBegin = reader.GetValue<double>("kilometer_start"),
                    KmEnd = reader.GetValue<double>("kilometer_end"),
                    Volume = reader.GetValue<double?>("gaz_volume"),
                    Delta = reader.GetValue<double?>("gaz_volume_change")
                };
                result.Add(dto);
            }
            return result;

        }

    }
}
