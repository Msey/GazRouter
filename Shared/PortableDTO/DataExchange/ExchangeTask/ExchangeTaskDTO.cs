using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.TransportTypes;

namespace GazRouter.DTO.DataExchange.ExchangeTask
{
    [DataContract]
    public class ExchangeTaskDTO : NamedDto<int>
    {
        public ExchangeTaskDTO()
        {
            IsSql = false;
            ExchangeStatus = ExchangeStatus.Event;
        }

        [DataMember]
        public int DataSourceId { get; set; }

        [DataMember]
        public string DataSourceName { get; set; }

        [DataMember]
        public ExchangeType ExchangeTypeId { get; set; }
        
        [DataMember]
        public PeriodType PeriodTypeId { get; set; }

        [DataMember]
        public bool IsCritical { get; set; }
        
        [DataMember]
        public string FileNameMask { get; set; }

        
        [DataMember]
        public bool IsTransform { get; set; }

        [DataMember]
        public string Transformation { get; set; }


        [DataMember]
        public TransportType? TransportTypeId { get; set; }
        
        [DataMember]
        public string TransportAddress { get; set; }
        
        [DataMember]
        public string TransportLogin { get; set; }

        [DataMember]
        public string TransportPassword { get; set; }

        [DataMember]
        public string HostKey { get; set; }

        [DataMember]
        public bool SendAsAttachment { get; set; }

        [DataMember]
        public Guid? EnterpriseId { get; set; }

        [DataMember]
        public string EnterpriseCode { get; set; }

        [DataMember]
        public bool IsSql { get; set; }

        [DataMember]
        public string SqlProcedureName { get; set; }

        [DataMember]
        public ExchangeStatus ExchangeStatus { get; set; }

        [DataMember]
        public int Lag { get; set; }
        [DataMember]
        public string ExcludeHours { get; set; }
        
    }
}