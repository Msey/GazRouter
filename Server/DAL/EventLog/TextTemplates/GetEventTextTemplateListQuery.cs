using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog.TextTemplates;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.TextTemplates
{
    public class GetEventTextTemplateListQuery : QueryReader<Guid, List<EventTextTemplateDTO>>
    {
        public GetEventTextTemplateListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(Guid parameters)
        {
            return @" SELECT    event_text_template_id,
                                template_name,
                                template_text,
                                description,
                                upd_user_id,
                                upd_user_name,
                                upd_user_site_id,
                                upd_user_site_name,
                                upd_date
                      FROM eventlog.v_event_text_templates
                      WHERE upd_user_site_id = :siteid";

        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("siteid", parameters);
        }


        protected override List<EventTextTemplateDTO> GetResult(OracleDataReader reader, Guid parameters)
        {
            var result = new List<EventTextTemplateDTO>();
            while (reader.Read())
            {
                result.Add(new EventTextTemplateDTO
                {
                    Id = reader.GetValue<int>("event_text_template_id"),
                    Name = reader.GetValue<string>("template_name"),
                    Text = reader.GetValue<string>("template_text"),
                    UserId = reader.GetValue<int>("upd_user_id"),
                    UserName = reader.GetValue<string>("upd_user_name"),
                    UpdateDate = reader.GetValue<DateTime>("upd_date"),
                    SiteId = reader.GetValue<Guid>("upd_user_site_id"),
                    SiteName = reader.GetValue<string>("upd_user_site_name"),
                    
                });
            }
            return result;
        }
    }
}
