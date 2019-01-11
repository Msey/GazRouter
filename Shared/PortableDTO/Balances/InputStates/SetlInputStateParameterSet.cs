using System;
using GazRouter.DTO.ManualInput.InputStates;

namespace GazRouter.DTO.Balances.InputStates
{
    public class SetInputStateParameterSet
    {
        public int ContractId { get; set; }
        public Guid SiteId { get; set; }
        public ManualInputState State { get; set; }
    }
}