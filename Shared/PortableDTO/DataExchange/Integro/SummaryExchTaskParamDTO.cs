using GazRouter.DTO.DataExchange.ExchangeTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.DataExchange.Integro
{
    [DataContract]
    [KnownType(typeof(SummaryDTO))]
    [KnownType(typeof(ExchangeTaskDTO))]
    public class SummaryExchTaskDTO
    {
        [DataMember]
        public SummaryDTO Summary { get; set; }
        [DataMember]
        public ExchangeTaskDTO ExchangeTask { get; set; }
    }
}
