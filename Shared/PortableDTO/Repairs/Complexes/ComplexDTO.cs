using System;
using System.Runtime.Serialization;

// Типы комплексов 
namespace GazRouter.DTO.Repairs.Complexes
{
    [DataContract]
    public class ComplexDTO : BaseDto<int>
    {
        [DataMember]
        public string ComplexName { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }
        
        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public bool IsLocal { get; set; }

        [DataMember]
        public int SystemId { get; set; }
        
    }
}
