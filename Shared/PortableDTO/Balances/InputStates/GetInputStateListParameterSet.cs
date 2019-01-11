using System;

namespace GazRouter.DTO.Balances.InputStates
{
    public class GetInputStateListParameterSet
    {
        public int ContractId { get; set; }
        public Guid? SiteId { get; set; }
    }
}