using System;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.DataExchange.ExchangeProperty
{
    public class SetExchangePropertyParameterSet
    {
        public int ExchangeTaskId { get; set; }
        public Guid EntityId { get; set; }
        public PropertyType PropertyTypeId { get; set; }
        public string ExtId { get; set; }
        public double? Coeff { get; set; }
    }
}