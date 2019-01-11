using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Attachments;

namespace GazRouter.DTO.Repairs.RepairWorks
{
    [DataContract]
    public class RepairWorkAttachmentDTO: AttachmentDTO<int,int>
    {
        [DataMember]
        public DateTime CreationDate { get; set; }
        
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string UserDescription { get; set; }
    }
}
