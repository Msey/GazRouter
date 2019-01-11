using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog.EventAnalytical;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.EventAnalytical
{
    public class GetEventAnalyticalAckListQuery : QueryReader<EventAnalyticalParameterSet, List<EventAnalyticalDTO>>
    {
        public GetEventAnalyticalAckListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(EventAnalyticalParameterSet parameters)
        {
            return
                @"
                SELECT en.entity_name,
                round(MAX(er.ACK_DATE - e.CREATE_DATE)*24*60) as MAX_DATE, 
                round(AVG(er.ACK_DATE - e.CREATE_DATE)*24*60) as AVG_DATE                    
                from V_EVENT_RECIPIENTS er JOIN V_EVENTS e on e.EVENT_ID = er.EVENT_ID
                join V_ENTITIES en on er.site_id = en.entity_id
                where e.CREATE_DATE BETWEEN :dateBegin and :dateEnd
                and er.ACK_DATE is not null
                and  er.ACK_DATE  <> e.CREATE_DATE
                and  er.site_id = :siteId
                GROUP BY en.entity_name";
        }

        protected override void BindParameters(OracleCommand command, EventAnalyticalParameterSet parameters)
        {
            command.AddInputParameter("siteId", parameters.SiteId);
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
                        Name = reader.GetValue<string>("entity_name"),
                        Max = reader.GetValue<int>("MAX_DATE"),
                        Avg = reader.GetValue<int>("AVG_DATE"),
                    });
            }
            return result;
        }
    }
}