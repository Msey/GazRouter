using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.EventLog
{
    public class GetQueueExchangeEventListParameterSet
    {

        public int? EventId { get; set; }

        public ExchangeEventStatus? Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string ChangeUserName { get; set; }
    }
}
