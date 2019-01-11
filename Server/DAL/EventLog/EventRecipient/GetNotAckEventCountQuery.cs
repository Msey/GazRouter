using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog.EventRecipient;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.EventRecipient
{
    public sealed class GetNotAckEventCountQuery : QueryReader<Guid, NonAckEventCountDTO>
    {
        public GetNotAckEventCountQuery(ExecutionContext context) : base(context)
        {
        }

        protected override string GetCommandText(Guid parameters)
        {
            return @"   SELECT      COUNT(*) AS cnt, 
                                    MAX(er.assignment_date) AS max_date 

                        FROM        v_event_recipients er 
                        WHERE       er.ack_date IS NULL 
                            AND     er.site_id = :siteId";
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("siteId", parameters);
        }

        protected override NonAckEventCountDTO GetResult(OracleDataReader reader, Guid parameters)
        {
            if (reader.Read())
            {
                return new NonAckEventCountDTO
                {
                    Count = reader.GetValue<int>("cnt"),
                    LastEventDate = reader.GetValue<DateTime>("max_date")
                };
            }
            return null;
        }
    }
}