using System;

namespace GazRouter.DTO.ObjectModel.Consumers
{
    public class GetConsumerListParameterSet
    {
        public Guid? DistrStationId { get; set; }
        public int? SystemId { get; set; }
    }
}
