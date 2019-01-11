using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;

namespace GazRouter.DTO.Balances.Contracts
{
    public class GetContractListParameterSet
	{
        public int? ContractId { get; set; }

        public PeriodType? PeriodTypeId { get; set; }

        public Target? TargetId { get; set; }

        public int? SystemId { get; set; }

        public bool? IsFinal { get; set; }

        public DateTime? ContractDate { get; set; }

        public int? Day { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }
	}
}
