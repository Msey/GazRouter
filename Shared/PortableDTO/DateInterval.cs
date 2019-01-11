using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO
{
    [DataContract]
    public class DateIntervalDTO
    {
        [DataMember]
        public DateTime BeginDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }
    }
}
