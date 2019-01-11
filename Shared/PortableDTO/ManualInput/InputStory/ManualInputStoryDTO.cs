using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.ManualInput.InputStory
{
    [DataContract]
    public class ManualInputStoryDTO
    {
        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public Guid SiteId { get; set; }
        
        [DataMember]
        public DateTime EditDate { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string SiteName { get; set; }

        [DataMember]
        public bool IsChanged { get; set; }
    }
}