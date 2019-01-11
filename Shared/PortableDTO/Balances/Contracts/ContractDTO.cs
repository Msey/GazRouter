using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;

namespace GazRouter.DTO.Balances.Contracts
{
	[DataContract]
	public class ContractDTO : BaseDto<int>
	{
		[DataMember]
		public PeriodType PeriodTypeId { get; set; }

		[DataMember]
		public Target TargetId { get; set; }

        [DataMember]
        public int SystemId { get; set; }

        [DataMember]
        public bool IsFinal { get; set; }

        [DataMember]
		public DateTime ContractDate { get; set; }
    }
}