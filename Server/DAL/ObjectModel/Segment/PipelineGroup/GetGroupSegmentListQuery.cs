using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Segment;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Segment.PipelineGroup
{
    public class GetGroupSegmentListQuery : QueryReader<Guid?, List<GroupSegmentDTO>>
	{
        public GetGroupSegmentListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(Guid? parameters)
		{
            var q = @"  SELECT      segments_by_group_id, 
                                    pipeline_group_id, 
                                    pipeline_group_name, 
                                    pipeline_name, 
                                    pipeline_id, 
                                    kilometer_start, 
                                    kilometer_end 
                        FROM        rd.v_segments_by_groups
                        WHERE       1=1";

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

        protected override List<GroupSegmentDTO> GetResult(OracleDataReader reader, Guid? parameters)
		{
            var result = new List<GroupSegmentDTO>();
			while (reader.Read())
			{
                result.Add(new GroupSegmentDTO
				{
                    Id = reader.GetValue<int>("segments_by_group_id"),
                    PipelineGroupId = reader.GetValue<Guid>("pipeline_group_id"),
                    PipelineGroupName = reader.GetValue<string>("pipeline_group_name"),
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
