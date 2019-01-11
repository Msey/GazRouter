using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.PipelineLimits;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Units;

namespace GazRouter.DAL.ManualInput.PipelineLimits
{
    public class GetPipelineLimitsListQuery : QueryReader<GetPipelineLimitListParameterSet, List<PipelineLimitDTO>>
    {
        public GetPipelineLimitsListQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(GetPipelineLimitListParameterSet parameter)
        {
            var q =
                @"select   t.limit_id,
                           t.pipeline_id,
                           t.kilometer_start,
                           t.kilometer_end,
                           t.p_max,
                           t.description
                from rd.V_PIPELINE_LIMITS t
                WHERE 1=1";


            var sb = new StringBuilder(q);

            if (parameter != null)
            {
                if (parameter.PipelineId != null)
                    sb.Append(" AND t.pipeline_id = : pipelineid");
                if (parameter.LimitId != null)
                    sb.Append(" AND t.limit_id = : limitid");
            }
            
            return sb.ToString();
        }


        protected override void BindParameters(OracleCommand command, GetPipelineLimitListParameterSet parameters)
        {
            if (parameters == null) return;
            if (parameters.PipelineId != null) command.AddInputParameter("pipelineid", parameters.PipelineId);
            if (parameters.LimitId != null)    command.AddInputParameter("limitid", parameters.LimitId);
        }

        protected override List<PipelineLimitDTO> GetResult(OracleDataReader reader, GetPipelineLimitListParameterSet parameter)
        {
            var tests = new List<PipelineLimitDTO>();
            while (reader.Read())
            {
                var test =
                    new PipelineLimitDTO
                    {
                        Id = reader.GetValue<int>("limit_id"),
                        PipelineId = reader.GetValue<Guid>("pipeline_id"),
                        Begin = reader.GetValue<double>("kilometer_start"),
                        End = reader.GetValue<double?>("kilometer_end"),
                        MaxPressure =  Pressure.FromKgh(reader.GetValue<double>("p_max")),
                        Description = reader.GetValue<string>("description")
                    };
                tests.Add(test);
            }
            return tests;
        }
    }
}
