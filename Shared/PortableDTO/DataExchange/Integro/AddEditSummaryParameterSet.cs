using GazRouter.DTO.Dictionaries.PeriodTypes;
using System;

namespace GazRouter.DTO.DataExchange.Integro
{
    public class AddEditSummaryParameterSet
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Descriptor { get; set; }

        public string TransformFileName { get; set; }

        public PeriodType PeriodType { get; set; }

        public int SystemId { get; set; }

        public int ExchangeTaskId { get; set; }

        public string SessionType { get; set; }

        public int StatusTypeId { get; set; }
    }
}
