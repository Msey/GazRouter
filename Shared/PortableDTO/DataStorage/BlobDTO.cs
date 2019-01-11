using System.Runtime.Serialization;

namespace GazRouter.DTO.DataStorage
{
    [DataContract]
    public class BlobDTO
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public byte[] Data { get; set; }
    }
}
