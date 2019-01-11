using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog.EventRecipient;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.EventRecipient
{
	public class GetEventRecepientListQuery : QueryReader<int, List<EventRecepientDTO>>
    {
        public GetEventRecepientListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(int parameters)
        {
            return @"select er.EVENT_RECIPIENT_ID, er.EVENT_ID, er.ASSIGNMENT_DATE, er.SITE_ID, er.ACK_USER_ID, er.Ack_DATE, er.PRIORITY_ID, u.NAME user_name, e.entity_name, e.name    
                        from V_EVENT_RECIPIENTS er LEFT JOIN V_USERS u on er.ACK_USER_ID = u.user_id
                        left join v_entities e on er.site_id = e.entity_id
                        where EVENT_ID = :eventId";
        }

		protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("eventId", parameters);
        }

        protected override List<EventRecepientDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new List<EventRecepientDTO>();
            while (reader.Read())
            {
                result.Add(
                    new EventRecepientDTO
                    {
                        Id = reader.GetValue<Guid>("EVENT_RECIPIENT_ID"),
                        EventId = reader.GetValue<int>("EVENT_ID"),
                        PriorityId = reader.GetValue<int>("PRIORITY_ID"),
                        SiteId = reader.GetValue<Guid>("SITE_ID"),
                        AssignementDate = reader.GetValue<DateTime>("ASSIGNMENT_DATE"),
                        AckDate = reader.GetValue<DateTime?>("Ack_DATE"),
                        Recepient = reader.GetValue<string>("entity_name"),
                        AckRecepient = reader.GetValue<string>("user_name"),
                        EntityName = reader.GetValue<string>("name"),
                    });
            }
            return result;
        }
    }
}
