using System.Runtime.Serialization;

namespace GazRouter.DTO.Appearance
{
    [DataContract]
    public class SchemeDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int TypeId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int SystemId { get; set; }
    }
}