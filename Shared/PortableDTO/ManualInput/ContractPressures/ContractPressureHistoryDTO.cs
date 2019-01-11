using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.ManualInput.ContractPressures
{
    public class ContractPressureHistoryDTO
    {       
        [DataMember]
        public double? Pressure { get; set; }

        [DataMember]
        public DateTime ChangeDate { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string UserSiteName { get; set; }
    }
}
