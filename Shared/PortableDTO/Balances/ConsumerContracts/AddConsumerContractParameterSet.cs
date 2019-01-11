using System;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.DTO.Balances.ConsumerContracts
{
    public class AddConsumerContractParameterSet
    {
        public Guid ConsumerId { get; set; }
        public int GasOwnerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }

        
    }
}
