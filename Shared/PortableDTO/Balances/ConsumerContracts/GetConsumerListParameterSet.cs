using System;

namespace GazRouter.DTO.Balances.ConsumerContracts
{
    public class GetConsumerContractListParameterSet
    {
        public Guid? ConsumerId { get; set; }
        public Guid? DistrStationId { get; set; }
        public int? SystemId { get; set; }

        public bool? IsActive { get; set; }

        // Для получения действующих договоров на указанну дату
        public DateTime? TheDate { get; set; }
        
    }
}
