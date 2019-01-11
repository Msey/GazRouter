using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Attachments;

namespace GazRouter.DTO.DispatcherTasks.Attachments
{
	public class TaskAttachmentDTO : AttachmentDTO<Guid, Guid>
    {
        [DataMember]
        public DateTime CreateDate { get; set; }


        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string UserDescription { get; set; }


        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public string SiteName { get; set; }

    }
}