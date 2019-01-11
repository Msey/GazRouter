using System;
using System.Runtime.Serialization;


namespace GazRouter.DTO.ManualInput.CompUnitStates
{
    [DataContract]
    public class FailureRelatedUnitStartDTO
    {

        [DataMember]
        public int FailureId { get; set; }

        [DataMember]
        public int StateChangeId { get; set; }

        [DataMember]
        public DateTime StateChangeDate { get; set; }

        [DataMember]
        public Guid CompUnitId { get; set; }

		[DataMember]
        public string CompUnitName { get; set; }
        
        [DataMember]
        public int CompUnitTypeId { get; set; }
    }
}
