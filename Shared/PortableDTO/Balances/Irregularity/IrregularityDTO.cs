using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Balances.Irregularity
{
    [DataContract]
    public class IrregularityDTO : BaseDto<int>
    {

        [DataMember]
        public int BalanceValueId { get; set; }

        [DataMember]
        public int StartDayNum { get; set; }

        [DataMember]
        public int EndDayNum { get; set; }

        [DataMember]
        public double Value { get; set; }
    }
}