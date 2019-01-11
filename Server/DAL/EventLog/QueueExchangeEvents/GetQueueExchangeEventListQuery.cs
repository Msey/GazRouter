using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Text;

namespace GazRouter.DAL.EventLog.QueueExchangeEventCommands
{
    public class GetQueueExchangeEventListQuery : QueryReader<GetQueueExchangeEventListParameterSet, List<EventExchangeDTO>>
    {
        public GetQueueExchangeEventListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetQueueExchangeEventListParameterSet parameters)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"
                SELECT eeq.event_id,
                       eeq.event_date, 
                       eeq.status,
                       ev.event_text,
                       us.name
                 
                from V_EVENT_EXCHANGE_QUEUE eeq ");

            sb.Append(@"INNER JOIN V_EVENTS ev ON ev.event_id = eeq.event_id
                        INNER JOIN V_USERS us ON us.user_id = ev.create_user_id 
                        where 1 = 1");

            if (parameters.EventId != null)
                return sb.Append(@" AND (eeq.event_id = :eventId)").ToString();

            if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
                sb.Append(@" AND (eeq.event_date BETWEEN :startDate and :endDate)");

            if (parameters.Status != null)
                sb.Append(@" AND (eeq.status = :status)");

            if (string.IsNullOrEmpty(parameters.ChangeUserName)==false)
                sb.Append(@" AND (us.name = :changeUserName)");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetQueueExchangeEventListParameterSet parameters)
        {
            command.AddInputParameter("eventId", parameters.EventId);
            command.AddInputParameter("startDate", parameters.StartDate);
            command.AddInputParameter("endDate", parameters.EndDate);
            command.AddInputParameter("status", parameters.Status);
            command.AddInputParameter("changeUserName", parameters.ChangeUserName);
        }

        protected override List<EventExchangeDTO> GetResult(OracleDataReader reader, GetQueueExchangeEventListParameterSet parameters)
        {
            var result = new List<EventExchangeDTO>();
            while (reader.Read())
            {
                result.Add(
                    new EventExchangeDTO
                    {
                          Id = reader.GetValue<int>("event_id"),
                          EventDateTime = reader.GetValue<DateTime>("event_date"),
                          EventStatus = reader.GetValue<ExchangeEventStatus>("status"),
                          EventDescription = reader.GetValue<string>("event_text"),
                           ChangingUserName = reader.GetValue<string>("name")
                    });
            }
            return result;
        }
    }
}
