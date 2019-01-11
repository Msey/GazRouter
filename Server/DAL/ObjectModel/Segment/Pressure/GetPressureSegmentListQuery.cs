using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Segment;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Segment.Pressure
{
    public class GetPressureSegmentListQuery : QueryReader<Guid?, List<PressureSegmentDTO>>
	{
        public GetPressureSegmentListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(Guid? parameters)
		{
            var q = @"  SELECT      segments_by_pressure_id, 
                                    pipeline_id, 
                                    pipeline_name, 
                                    kilometer_start, 
                                    kilometer_end, 
                                    pressure 
                        FROM        rd.v_segments_by_pressure 
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters.HasValue)
                sb.Append(" AND pipeline_id = :pipelineId");

            sb.Append(" ORDER BY kilometer_start");

            return sb.ToString();
		}

        protected override void BindParameters(OracleCommand command, Guid? parameters)
        {
            if (parameters.HasValue)
                command.AddInputParameter("pipelineId", parameters);
        }

        protected override List<PressureSegmentDTO> GetResult(OracleDataReader reader, Guid? parameters)
		{
            var result = new List<PressureSegmentDTO>();
			while (reader.Read())
			{
                result.Add(new PressureSegmentDTO
				{
                    Id = reader.GetValue<int>("segments_by_pressure_id"),
                    PipelineId = reader.GetValue<Guid>("pipeline_id"),
                    PipelineName = reader.GetValue<string>("pipeline_name"),
                    KilometerOfStartPoint = reader.GetValue<double>("kilometer_start"),
                    KilometerOfEndPoint = reader.GetValue<double>("kilometer_end"),
                    Pressure = reader.GetValue<double>("pressure")
				});

			}
			return result;
		}
	}
}
