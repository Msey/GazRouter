using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Balances.ConsumerContracts
{
    [DataContract]
    public class ConsumerContractDTO
	{
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Guid ConsumerId { get; set; }

        [DataMember]
        public int GasOwnerId { get; set; }

        [DataMember]
        public string GasOwnerName { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        
        public bool IsOpenEnd => !EndDate.HasValue;

    }
}