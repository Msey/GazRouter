using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.EventPriorities;
using GazRouter.DTO.EventLog;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog
{
    public class GetEventListQuery : QueryReader<GetEventListParameterSet, List<EventDTO>>
    {
        public GetEventListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetEventListParameterSet parameters)
        {
            //EventPriority
            var builder = new StringBuilder();
            builder.Append(
                @"  SELECT          e.event_id, 
                                    e.event_type_id, 
                                    e.event_num,
                                    e.event_date, 
                                    e.event_text,  
                                    e.create_user_id, 
                                    e.create_date, 
                                    ent.entity_id, 
                                    ent.entity_name,
                                    ent.entity_type_id, 
                                    e.user_entity_id, 
                                    e.user_entity_name, 
                                    e.comments_count, 
                                    e.attachments_count, 
                                    quotrec.priority_id AS priority,
                                    quotrec.upd_date AS checkdate, 
                                    quotrec.name AS checkuser, 
                                    quotrec.ack_date AS isquetirovaly,
                                    n.entity_name AS path,
                                    u.name AS create_user_name, 
                                    e.kilometer,                   
                    CASE    WHEN e.event_type_id = 3
                            THEN 1
                            ELSE 0
                    END AS emergency,
                    
                    CASE    WHEN rd.p_entity.GetSiteID(n.entity_id) <> NULL
                            THEN 
                               (SELECT s.site_name FROM v_sites s WHERE s.site_id = rd.p_entity.GetSiteID(n.entity_id))
                            ELSE 
                               (SELECT s.site_name FROM v_sites s WHERE s.site_id = rd.p_entity.GetSiteID(n.entity_id, e.kilometer))
                    END AS site_name

                    FROM    v_events e
                    JOIN    v_entities ent ON e.entity_id = ent.entity_id
                    LEFT JOIN    v_users u ON e.create_user_id = u.user_id
					JOIN    (   SELECT      er.event_id, 
                                            er.site_id, 
                                            er.priority_id, 
                                            er.ack_date, 
                                            er.upd_date, 
                                            u.name
                                FROM        v_event_recipients er
                                LEFT JOIN   v_users u ON er.upd_user_id = u.user_id) quotrec ON quotrec.event_id = e.event_id AND quotrec.site_id = :siteId
					LEFT JOIN   v_nm_short_all n on e.entity_id = n.entity_id 
                    WHERE 1=1 ");

            switch (parameters.QueryType)
            {
                case EventListType.Archive:
                    if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
                    {
                        builder.Append(@"AND (e.event_date >= :startDate AND e.event_date <= :endDate)");
                    }
                    break;

                case EventListType.Trash:
                    if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
                    {
                        builder.AppendFormat(@"AND (quotrec.priority_id = {0} and e.event_date >= :startDate AND e.event_date <= :endDate)", (int)EventPriority.Trash);
                    }

                    break;

                case EventListType.List:
                    if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
                    {
                        builder.AppendFormat(@"AND (quotrec.priority_id <> {0} and e.event_date >= :startDate AND e.EVENT_DATE <= :endDate)", (int)EventPriority.Trash);
                        //builder.AppendFormat(@"where (quotrec.ACK_Date is null and quotrec.priority_id <> {0} and e.EVENT_DATE >= :startDate AND e.EVENT_DATE <= :endDate)", (int)EventPriority.Trash);
                    }
                    else
                    {
                        builder.AppendFormat(@"AND ((quotrec.priority_id = {0} and e.event_date >= :dateparam) or quotrec.priority_id = {1} or quotrec.ACK_Date is null)", (int)EventPriority.Normal, (int)EventPriority.Control);
                    }
                    break;
            }

            if (parameters.EntityId.HasValue)
            {
                builder.Append(@" AND e.entity_id = :entityid");
            }

            builder.Append(" ORDER BY  e.event_date DESC");
            return builder.ToString();
        }

        //1) ((priority_id = 1 and EVENT_DATE <= 5 day) or priority_id = 2) list
        //2) EVENT_DATE > 5 day and priority_id<>2 archive
        //3) priority_id = 3 and EVENT_DATE < 5 day trash

        protected override void BindParameters(OracleCommand command, GetEventListParameterSet parameters)
        {
            command.AddInputParameter("siteId", parameters.SiteId);

            if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
            {
                switch (parameters.QueryType)
                {
                    case EventListType.List:
                        command.AddInputParameter("startDate", parameters.StartDate);
                        command.AddInputParameter("endDate", parameters.EndDate);
                        break;

                    case EventListType.Archive:
                        command.AddInputParameter("startDate", parameters.StartDate);
                        command.AddInputParameter("endDate", parameters.EndDate);
                        break;
                    case EventListType.Trash:
                        command.AddInputParameter("startDate", parameters.StartDate);
                        command.AddInputParameter("endDate", parameters.EndDate);
                        break;
                }
            }
            else
            {
                if (parameters.QueryType != EventListType.Archive)
                    command.AddInputParameter("dateparam", DateTime.Now.Date.AddDays(-parameters.ArchivingDelay));
            }

            if (parameters.EntityId.HasValue)
            {
                command.AddInputParameter("entityid", parameters.EntityId);
            }
        }


        protected override List<EventDTO> GetResult(OracleDataReader reader, GetEventListParameterSet parameters)
        {
            var result = new List<EventDTO>();
            while (reader.Read())
            {
                var checkDate = reader.GetValue<DateTime?>("checkdate");
                result.Add(new EventDTO
                {
                    Id = reader.GetValue<int>("event_id"),
                    TypeId = reader.GetValue<int>("event_type_id"),
                    EventDate = reader.GetValue<DateTime?>("event_date"),
                    Description = reader.GetValue<string>("event_text"),
                    CreateUserId = reader.GetValue<int>("create_user_id"),
                    CreateUserName = reader.GetValue<string>("create_user_name"),
                    CreateDate = reader.GetValue<DateTime?>("create_date"),
                    CommentsCount = reader.GetValue<int>("comments_count"),
                    AttachmentsCount = reader.GetValue<int>("attachments_count"),
                    UserEntityId = reader.GetValue<Guid>("user_entity_id"),
                    UserEntityName = reader.GetValue<string>("user_entity_name"),
                    IsQuote = reader.GetValue<DateTime?>("isquetirovaly").HasValue,
                    Priority = reader.GetValue<EventPriority>("priority"),
                    SiteName = reader.GetValue<string>("site_name"),
                    Kilometer = reader.GetValue<double?>("kilometer"),
                    CheckUser = checkDate.HasValue ?
                        string.Format("{0}\n{1}", reader.GetValue<string>("checkuser"), checkDate) : null,
                    IsEmergency = reader.GetValue<bool>("emergency"),
                    SerialNumber = reader.GetValue<string>("event_num"),
                    Entity = new CommonEntityDTO
                    {
                        Id = reader.GetValue<Guid>("entity_id"),
                        Name = reader.GetValue<string>("entity_name"),
                        ShortPath = reader.GetValue<string>("path"),
                        EntityType = reader.GetValue<EntityType>("entity_type_id")
                    }
                });
            }
            return result;
        }
    }
}
