using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Attachments;

namespace GazRouter.DTO.EventLog.Attachments
{
    [DataContract]
    public class EventAttachmentDTO : AttachmentDTO<Guid, int>
    {
        [DataMember]
        public DateTime CreateDate { get; set; }


        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string UserName { get; set; }


        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public string SiteName { get; set; }

    }
}