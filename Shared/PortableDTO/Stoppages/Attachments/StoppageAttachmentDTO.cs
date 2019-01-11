using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Stoppages.Attachments
{
    [DataContract]
    public class StoppageAttachmentDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int StoppageId { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int DataLength { get; set; }

        [DataMember]
        public Guid BlobId { get; set; }

    }
}
