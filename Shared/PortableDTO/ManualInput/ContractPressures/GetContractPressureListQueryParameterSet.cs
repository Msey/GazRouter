using System;

namespace GazRouter.DTO.ManualInput.ContractPressures
{
    public class GetContractPressureListQueryParameterSet
    {
        public Guid? SiteId { get; set; }

        public DateTime? TargetMonth {get;set;}
    }
}
