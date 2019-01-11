
namespace GazRouter.DTO.Attachments
{
    public class AddAttachmentParameterSet<T>
    {
        public T ExternalId { get; set; }
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }
}
