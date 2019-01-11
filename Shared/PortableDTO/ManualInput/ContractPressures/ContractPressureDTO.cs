using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.ManualInput.ContractPressures
{
    public class ContractPressureDTO
    {
        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public Guid DistrStationId { get; set; }

        [DataMember]
        public string DistrStationName { get; set; }

        [DataMember]
        public Guid DistrStationOutletId { get; set; }

        [DataMember]
        public string DistrStationOutletName { get; set; }

        [DataMember]
        public double? Pressure { get; set; }

        [DataMember]
        public DateTime? ChangeDate { get; set; }

        //[DataMember]
        //public string UserName { get; set; }
    }
}
