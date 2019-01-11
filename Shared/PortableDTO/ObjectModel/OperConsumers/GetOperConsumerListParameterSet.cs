using System;
using GazRouter.DTO.ObjectModel.Entities;

namespace GazRouter.DTO.ObjectModel.OperConsumers
{
    public class GetOperConsumerListParameterSet
    {
        public int? SystemId { get; set; }

        public Guid? SiteId { get; set; }
    }
}