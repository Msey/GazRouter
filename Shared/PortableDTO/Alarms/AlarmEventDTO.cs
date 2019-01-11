using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Alarms
{
    [DataContract]
    public class AlarmEventDTO : BaseDto<int>
    {
        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public double PropertyValue { get; set; }
        
        [DataMember]
        public bool Status { get; set; }
        
    }
}
