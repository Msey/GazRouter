using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.DataExchange.ExchangeLog
{
    [DataContract]
    public class ExchangeLogDTO
    {
        [DataMember]
        public int ExchangeTaskId { get; set; }

        [DataMember]
        public string ExchangeTaskName { get; set; }

        [DataMember]
        public int SourceId { get; set; }

        [DataMember]
        public string SourceName { get; set; }

        [DataMember]
        public int SerieId { get; set; }
        
        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public PeriodType PeriodType { get; set; }

        [DataMember]
        public DateTime StartTime { get; set; }

        [DataMember]
        public ExchangeType ExchangeType { get; set; }

        [DataMember]
        public bool IsOk { get; set; }

        [DataMember]
        public string DataContent { get; set; }

        [DataMember]
        public string ProcessingError { get; set; }
    }

    
}