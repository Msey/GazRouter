
namespace GazRouter.DTO.ASDU
{
    public class XmlFileForImport
    {
        public bool LoadFromDisk { get; set; }
        public bool IsMetadataFile { get; set; } = false;
        public string Filename { get; set; }
        public string Xml { get; set; }
    }
}
