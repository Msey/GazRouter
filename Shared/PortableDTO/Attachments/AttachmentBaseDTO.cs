using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Attachments
{
	[DataContract]
	public abstract class AttachmentBaseDTO
	{
        [DataMember]
		public string Description { get; set; }

        [DataMember]
        public Guid BlobId { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public int DataLength { get; set; }
     
    }
    
}