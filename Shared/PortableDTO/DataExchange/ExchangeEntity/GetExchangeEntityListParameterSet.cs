
using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.DataExchange.ExchangeEntity
{
    public class GetExchangeEntityListParameterSet
    {
        public GetExchangeEntityListParameterSet()
        {
            ExchangeTaskIdList = new List<int>();
            IsActive = false;
        }

        public List<int> ExchangeTaskIdList { get; set; }

        public bool IsActive { get; set; }

        public EntityType? EntityType { get; set; }

        public Guid? EntityId { get; set; }
    }
}