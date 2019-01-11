using System;

namespace GazRouter.DTO.Balances.MonthAlgorithms
{
    public class DivideVolumeAlgorithmParameterSet 
    {
        public int ContractId { get; set; }
        
        public int DefaultOwnerId { get; set; }
        
        public Guid? SiteId { get; set; }

        public bool IntakeTransitFilter { get; set; }
        public bool ConsumersFilter { get; set; }
        public bool OperConsumersFilter { get; set; }
    }

   
}
