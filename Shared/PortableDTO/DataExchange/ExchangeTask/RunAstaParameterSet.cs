using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.DataExchange.ExchangeTask
{
    public class RunAstaParameterSet
    {
        public PeriodType PeriodType { get; set; }
        
        public DateTime TimeStamp { get; set; }
    }
}