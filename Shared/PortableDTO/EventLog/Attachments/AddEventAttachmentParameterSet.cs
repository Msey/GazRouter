
namespace GazRouter.DTO.EventLog.Attachments
{
    public class AddEventAttachmentParameterSet
    {
        public int EventId { get; set; }
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }
}
