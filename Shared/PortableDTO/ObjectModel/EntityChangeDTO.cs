using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.ObjectModel
{
    [DataContract]
	public class EntityChangeDTO
    {
        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Action { get; set; }
    }
}
