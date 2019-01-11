using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.EventLog.EventRecipient
{
    [DataContract]
    public class NonAckEventCountDTO
    {
	    [DataMember]
		public int Count { get; set; }

        [DataMember]
		public DateTime LastEventDate { get; set; }
			
    }
}