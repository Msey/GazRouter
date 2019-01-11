using System;

namespace GazRouter.DTO.Balances.MiscTab
{
    public class AddRemoveMiscTabEntityParameterSet 
    {
        public Guid EntityId { get; set; }

        public int? BalGroupId { get; set; } 

        public int SystemId { get; set; }
        
    }
}
