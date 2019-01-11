using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.EventLog
{
    public class QueueExchangeEventParameterSet
    {
        public int EventId { get; set; }
        public DateTime EventDateTime { get; set; }
        public ExchangeEventStatus EventStatus { get; set; }
        public string ChangingUserName { get; set; }
    }
}
