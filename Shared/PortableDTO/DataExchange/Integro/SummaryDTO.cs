using GazRouter.DTO.Dictionaries.Integro;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.DataExchange.Integro
{
    [DataContract]
    public class SummaryDTO : NamedDto<Guid>
    {
        [DataMember]
        public string Descriptor { get; set; }

        [DataMember]
        public string TransformFileName { get; set; }

        [DataMember]
        public PeriodType PeriodType { get; set; }

        [DataMember]
        public string SessionDataCode { get; set; }

        [DataMember]
        public SessionDataType SessionDataType { get; set; }

        [DataMember]
        public int SystemId { get; set; }

        [DataMember]
        public int ExchangeTaskId { get; set; }

        [DataMember]
        public int StatusId { get; set; }
    }
}
