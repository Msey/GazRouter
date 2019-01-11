using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;

namespace GazRouter.DTO.Balances.Contracts
{
	public class AddContractParameterSet
    {
        public PeriodType PeriodTypeId { get; set; }
        public Target TargetId { get; set; }
        public int GasTransportSystemId { get; set; }
        public DateTime ContractDate { get; set; }
        public bool IsFinal { get; set; }		
    }
}