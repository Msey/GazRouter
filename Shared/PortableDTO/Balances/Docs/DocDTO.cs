using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Attachments;

namespace GazRouter.DTO.Balances.Docs
{
    [DataContract]
    public class DocDTO : AttachmentDTO<int, int>
    {
        
        [DataMember]
        public DateTime CreateDate { get; set; }
    }
}
