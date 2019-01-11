using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.InputStory;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.InputStory
{
    public class GetManualInputStoryQuery : QueryReader<GetManualInputStoryParameterSet, List<ManualInputStoryDTO>>
    {
        public GetManualInputStoryQuery(ExecutionContext context)
            : base(context)
        {
  
        }

        protected override string GetCommandText(GetManualInputStoryParameterSet parameters)
        {
            var q =
                @"  SELECT  s.entity_id,
                            s.user_name,
                            s.edit_date,
                            s.user_site_name,
                            site.site_id
                FROM        rd.v_input_story s 
                LEFT JOIN  v_entity_2_site site ON site.entity_id = s.entity_id
                WHERE       s.series_id = :serieid";

            var sb = new StringBuilder(q);
            if (parameters.EntityId.HasValue) sb.Append(" AND s.entity_id = :entityid");
            if (parameters.SiteId.HasValue) sb.Append(" AND site.site_id = :siteid");
            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetManualInputStoryParameterSet parameters)
        {
            command.AddInputParameter("serieid", parameters.SerieId);
            if (parameters.EntityId.HasValue) command.AddInputParameter("entityid", parameters.EntityId);
            if (parameters.SiteId.HasValue) command.AddInputParameter("siteid", parameters.SiteId);
        }

        protected override List<ManualInputStoryDTO> GetResult(OracleDataReader reader, GetManualInputStoryParameterSet parameters)
        {
            var result = new List<ManualInputStoryDTO>();
            while (reader.Read())
            {
                result.Add(new ManualInputStoryDTO
                {
                    EntityId = reader.GetValue<Guid>("entity_id"),
                    UserName = reader.GetValue<string>("user_name"),
                    SiteName = reader.GetValue<string>("user_site_name"),
                    EditDate = reader.GetValue<DateTime>("edit_date"),
                    SiteId = reader.GetValue<Guid>("site_id"),
                    IsChanged = true
                });
            }

            return result;
        }
    }
}