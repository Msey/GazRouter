using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Balances.Corrections
{
    [DataContract]
    public class CorrectionDTO
    {
        [DataMember]
        public int BalanceValueId { get; set; }

        [DataMember]
        public int DocId { get; set; }

        [DataMember]
        public double Value { get; set; }
    }
}