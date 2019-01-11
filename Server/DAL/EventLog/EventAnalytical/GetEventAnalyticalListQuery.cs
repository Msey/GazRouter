using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog.EventAnalytical;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.EventAnalytical
{
    public class GetEventAnalyticalListQuery : QueryReader<EventAnalyticalParameterSet, List<EventAnalyticalDTO>>
    {
        public GetEventAnalyticalListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(EventAnalyticalParameterSet parameters)
        {
         
            return
                @"select n.ENTITY_NAME SITE_NAME
                           ,count(distinct e.event_id) as count_event
                       from V_EVENTS e
                     INNER JOIN  V_EVENT_RECIPIENTS  er on e.event_id = er.event_id and er.ack_date = e.create_date
                     left JOIN V_NM_SHORT_ALL n
ON er.SITE_ID              = n.ENTITY_ID

                      where e.create_date between :dateBegin and :dateEnd
                    group by n.ENTITY_NAME
";
        }

        protected override void BindParameters(OracleCommand command, EventAnalyticalParameterSet parameters)
        {
            command.AddInputParameter("dateBegin", parameters.DateBegin);
            command.AddInputParameter("dateEnd", parameters.DateEnd);
        }

        protected override List<EventAnalyticalDTO> GetResult(OracleDataReader reader, EventAnalyticalParameterSet parameters)
        {
            var result = new List<EventAnalyticalDTO>();
            while (reader.Read())
            {
                result.Add(
                    new EventAnalyticalDTO
                    {
                        Name = reader.GetValue<string>("SITE_NAME"),
                        Total = reader.GetValue<int>("count_event"),
                    });
            }
            return result;
        }
    }
}