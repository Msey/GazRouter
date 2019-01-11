using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Segment;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Segment.Site
{
    public class GetSiteSegmentListQuery : QueryReader<Guid?, List<SiteSegmentDTO>>
	{
        public GetSiteSegmentListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(Guid? parameters)
		{
            var q = @"  SELECT      segments_by_sites_id, 
                                    site_id, 
                                    site_name, 
                                    pipeline_id, 
                                    kilometer_start, 
                                    kilometer_end 
                        FROM        rd.v_segments_by_sites 
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

        protected override List<SiteSegmentDTO> GetResult(OracleDataReader reader, Guid? parameters)
		{
			var result = new List<SiteSegmentDTO>();
			while (reader.Read())
			{
				result.Add(new SiteSegmentDTO
				{
                    Id = reader.GetValue<int>("segments_by_sites_id"),
                    SiteId = reader.GetValue<Guid>("site_id"),
                    SiteName = reader.GetValue<string>("site_name"),
                    PipelineId = reader.GetValue<Guid>("pipeline_id"),
                    KilometerOfStartPoint = reader.GetValue<double>("kilometer_start"),
                    KilometerOfEndPoint = reader.GetValue<double>("kilometer_end")
				});

			}
			return result;
		}
	}
}
