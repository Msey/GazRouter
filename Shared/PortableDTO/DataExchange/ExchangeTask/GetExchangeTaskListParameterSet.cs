
using System;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using System.Collections.Generic;

namespace GazRouter.DTO.DataExchange.ExchangeTask
{
    public class GetExchangeTaskListParameterSet
    {
        public int? Id { get; set; }

        public List<int> Ids { get; set; }

        public int? SourceId { get; set; }

        public ExchangeType? ExchangeTypeId { get; set; }

        public PeriodType? PeriodTypeId { get; set; }
        public Guid? EnterpriseId { get; set; }
    }
}