
namespace GazRouter.DTO.Stoppages.Attachments
{
    public class AddStoppageAttachmentParameterSet
    {
        public int StoppageId { get; set; }
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }
}
