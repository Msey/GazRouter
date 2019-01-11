
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.TransportTypes;

namespace GazRouter.DTO.DataExchange.ExchangeTask
{
    public class AddExchangeTaskParameterSet
    {
        public AddExchangeTaskParameterSet()
        {
            IsSql = false;
            ExchangeStatus = ExchangeStatus.Off;
        }

        public string Name { get; set; }

        public int DataSourceId { get; set; }

        public ExchangeType ExchangeTypeId { get; set; }

        public PeriodType PeriodTypeId { get; set; }

        public bool IsCritical { get; set; }

        public string FileNameMask { get; set; }
        
        public int Lag { get; set; }
        public bool IsTransform { get; set; }

        public string Transformation { get; set; }

        public TransportType? TransportTypeId { get; set; }
        public ExchangeStatus ExchangeStatus { get; set; }

        public string TransportAddress { get; set; }

        public string TransportLogin { get; set; }

        public string TransportPassword { get; set; }

        public string HostKey { get; set; }

        public bool SendAsAttachment { get; set; }

        public bool IsSql { get; set; }

        public string SqlProcedureName { get; set; }

        public string ExcludeHours { get; set; }

    }
}