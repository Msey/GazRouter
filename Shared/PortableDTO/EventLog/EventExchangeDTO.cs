using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.EventLog
{
    [DataContract]
    public class EventExchangeDTO : BaseDto<int>
    {
        [DataMember]
        public DateTime EventDateTime { get; set; }

        [DataMember]
        public ExchangeEventStatus EventStatus { get; set; }

        [DataMember]
        public string EventDescription { get; set; }

        [DataMember]
        public string ChangingUserName { get; set; }
    }
}
