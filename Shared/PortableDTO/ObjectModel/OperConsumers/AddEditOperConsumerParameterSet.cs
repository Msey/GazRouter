using System;

namespace GazRouter.DTO.ObjectModel.OperConsumers
{
    public class AddEditOperConsumerParameterSet 
    {
        public Guid Id { get; set; }
        public Guid? SiteId{ get; set; }
        public int ConsumerType { get; set; }
        public bool IsDirectConnection { get; set; }
        public string ConsumerName { get; set; }
		public int RegionId { get; set; }

        public Guid? DistrStationId { get; set; }

        public int? BalanceGroupId { get; set; }
    }
}
