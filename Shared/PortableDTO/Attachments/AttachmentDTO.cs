using System.Runtime.Serialization;

namespace GazRouter.DTO.Attachments
{
	[DataContract]
	public class AttachmentDTO<TId, TExtId> : AttachmentBaseDTO
	{
        [DataMember]
        public TId Id { get; set; }

        [DataMember]
        public TExtId ExternalId { get; set; }
    }
    
}