using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog;
using GazRouter.DTO.EventLog.EventRecipient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.EventLog
{
    public class AddQueueExchangeEventCommand : CommandNonQuery<QueueExchangeEventParameterSet>
    {
        public AddQueueExchangeEventCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, QueueExchangeEventParameterSet parameters)
        {
            command.AddInputParameter("p_event_id", parameters.EventId);
            command.AddInputParameter("p_event_date", parameters.EventDateTime);
            command.AddInputParameter("p_status", (int)(parameters.EventStatus));
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(QueueExchangeEventParameterSet parameters)
        {
            return "eventlog.P_EVENT_EXCHANGE_QUEUE.Set_IT";
        }
    }
}
             