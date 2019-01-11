using System;

namespace GazRouter.DTO.Balances.ConsumerContracts
{
    public class GetConsumerContractListParameterSet
    {
        public Guid? ConsumerId { get; set; }
        public Guid? DistrStationId { get; set; }
        public int? SystemId { get; set; }

        public bool? IsActive { get; set; }

        // ��� ��������� ����������� ��������� �� �������� ����
        public DateTime? TheDate { get; set; }
        
    }
}
