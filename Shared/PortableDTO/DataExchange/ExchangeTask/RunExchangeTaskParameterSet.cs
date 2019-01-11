using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.DataExchange.ExchangeTask
{
    public class RunExchangeTaskParameterSet
    {
        public int? Id { get; set; }
        
        public DateTime TimeStamp { get; set; }
        public PeriodType? PeriodTypeId { get; set; }
    }
}