using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Segment;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Segment.Diameter
{
    public class GetDiameterSegmentListQuery : QueryReader<Guid?, List<DiameterSegmentDTO>>
	{
		public GetDiameterSegmentListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(Guid? parameters)
		{
			var q = @" SELECT       seg.segments_by_diametr_id, 
                                    seg.pipeline_id, 
                                    seg.pipeline_name, 
                                    seg.kilometer_start, 
                                    seg.kilometer_end, 
                                    seg.diametr_id,
                                    seg.diametrs_external_id, 
                                    dia.diametr_name, 
                                    dia.diameter_real, 
                                    dia.diameter_conv,
                                    exd.diametr_external,
                                    exd.wall_thickness
	                    FROM        rd.v_segments_by_diametr seg 
                        INNER JOIN  v_diametrs dia ON seg.diametr_id = dia.diametr_id  
                        LEFT JOIN   v_diametrs_external exd on seg.diametrs_external_id = exd.diametrs_external_id
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters.HasValue) sb.Append(" AND pipeline_id = :pipelineId");

            sb.Append(" ORDER BY seg.kilometer_start");

            return sb.ToString();

		}

        protected override void BindParameters(OracleCommand command, Guid? parameters)
        {
            if (parameters.HasValue)
                command.AddInputParameter("pipelineId", parameters);
        }

        protected override List<DiameterSegmentDTO> GetResult(OracleDataReader reader, Guid? parameters)
		{
			var result = new List<DiameterSegmentDTO>();
			while (reader.Read())
			{
				result.Add(new DiameterSegmentDTO
				{
					Id = reader.GetValue<int>("segments_by_diametr_id"),
                    PipelineId = reader.GetValue<Guid>("pipeline_id"),
                    PipelineName = reader.GetValue<string>("pipeline_name"),
                    KilometerOfStartPoint = reader.GetValue<double>("kilometer_start"),
                    KilometerOfEndPoint = reader.GetValue<double>("kilometer_end"),
					DiameterId = reader.GetValue<int>("diametr_id"),
                    ExternalDiameterId = reader.GetValue<int>("diametrs_external_id"),
					DiameterName = reader.GetValue<string>("diametr_name"),
                    DiameterReal = reader.GetValue<int>("diameter_real"),
                    DiameterConv = reader.GetValue<int>("diameter_conv"),
                    ExternalDiameter = reader.GetValue<double>("diametr_external"),
                    WallThickness = reader.GetValue<double>("wall_thickness"),
                });

			}
			return result;
		}
	}
}
