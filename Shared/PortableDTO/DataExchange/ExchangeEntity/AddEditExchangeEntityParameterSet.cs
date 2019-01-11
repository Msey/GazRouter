using System;

namespace GazRouter.DTO.DataExchange.ExchangeEntity
{
    public class AddEditExchangeEntityParameterSet
    {
        public int ExchangeTaskId { get; set; }
        public Guid EntityId { get; set; }
        public string ExtId { get; set; }
        public bool IsActive { get; set; }
    }
}