using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog.Attachments;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.Attachments
{
    public class GetEventAttachmentListQuery : QueryReader<int, List<EventAttachmentDTO>>
    {
        public GetEventAttachmentListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(int parameters)
        {
            return @"   SELECT      ea.event_attachment_id, 
                                    ea.event_id, 
                                    ea.file_name, 
                                    ea.blob_id,
                                    ea.data_length, 
                                    ea.create_date, 
                                    ea.create_user_id, 
                                    u.name AS create_user_name, 
                                    n.entity_name AS user_entity_name, 
                                    ea.description, 
                                    u.site_id

                        FROM        v_event_attachments ea 
                        JOIN        v_users u ON ea.create_user_id = u.user_id
                        LEFT JOIN   v_entities e ON u.site_id = e.entity_id
                        LEFT JOIN   v_nm_short_all n ON n.entity_id = u.site_id 
                        WHERE       event_id = :eventid
                        ORDER BY    ea.create_date DESC";
          
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("eventId", parameters);
        }

        protected override List<EventAttachmentDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new List<EventAttachmentDTO>();
            while (reader.Read())
            {
                result.Add(
                    new EventAttachmentDTO
                    {
                        Id = reader.GetValue<Guid>("event_attachment_id"),
                        ExternalId = reader.GetValue<int>("event_id"),
                        BlobId = reader.GetValue<Guid>("blob_id"),
                        FileName = reader.GetValue<string>("file_name"),
                        DataLength = reader.GetValue<int>("data_length"),
                        CreateDate = reader.GetValue<DateTime>("create_date"),
                        UserId = reader.GetValue<int>("create_user_id"),
                        UserName = reader.GetValue<string>("create_user_name"),
                        SiteId = reader.GetValue<Guid>("site_id"),
                        SiteName = reader.GetValue<string>("user_entity_name"),
                        Description = reader.GetValue<string>("description")
                    });
            }
            return result;
        }
    }
}