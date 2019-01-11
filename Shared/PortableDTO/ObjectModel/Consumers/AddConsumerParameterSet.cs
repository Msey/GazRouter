using System;

namespace GazRouter.DTO.ObjectModel.Consumers
{
    public class AddConsumerParameterSet : AddEntityParameterSet
    {
        public int ConsumerType { get; set; }
        public int RegionId { get; set; }
        public int? DistrNetworkId { get; set; }
        public Guid? Id { get; set; }
    }
}
