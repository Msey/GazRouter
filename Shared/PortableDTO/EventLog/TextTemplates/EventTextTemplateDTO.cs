using System;
using System.Runtime.Serialization;


namespace GazRouter.DTO.EventLog.TextTemplates
{
    [DataContract]
    public class EventTextTemplateDTO : BaseDto<int>
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public DateTime UpdateDate { get; set; }

        [DataMember]
        public Guid SiteId { get; set; }


        [DataMember]
        public string SiteName { get; set; }
        
        
    }
}
