using System;

namespace GazRouter.DTO.Balances.Values
{
    public class GetBalanceValueListParameterSet
    {
        public int ContractId { get; set; }
        
        public Guid? SiteId { get; set; }
    }
}