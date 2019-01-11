using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Segment;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.ObjectModel.Segment.Regions
{
    public class GetRegionSegmentListQuery : QueryReader<Guid?, List<RegionSegmentDTO>>
    {
        public GetRegionSegmentListQuery(ExecutionContext context)
			: base(context)
		{
        }

        protected override string GetCommandText(Guid? parameters)
        {
            var q = 
@"
SELECT 
    segments_by_region_id,
    pipeline_id, pipeline_name, pipeline_description,
    region_id,
    kilometer_start, kilometer_end 
FROM
    rd.v_segments_by_region 
WHERE 1=1";

            var sb = new StringBuilder(q);

            if (parameters.HasValue) sb.Append(" AND pipeline_id = :pipelineId");

            sb.Append(" ORDER BY kilometer_start");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, Guid? parameters)
        {
            if (parameters.HasValue)
                command.AddInputParameter("pipelineId", parameters);
        }

        protected override List<RegionSegmentDTO> GetResult(OracleDataReader reader, Guid? parameters)
        {
            var result = new List<RegionSegmentDTO>();
            while (reader.Read())
            {
                result.Add(new RegionSegmentDTO
                {
                    Id = reader.GetValue<int>("segments_by_region_id"),
                    RegionID = reader.GetValue<int>("region_id"),
                    PipelineId = reader.GetValue<Guid>("pipeline_id"),
                    PipelineName = reader.GetValue<string>("pipeline_name"),
                    KilometerOfStartPoint = reader.GetValue<double>("kilometer_start"),
                    KilometerOfEndPoint = reader.GetValue<double>("kilometer_end")                    
                });

            }
            return result;
        }
    }
}
