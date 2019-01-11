using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.Diameters
{
    [DataContract]
    public class ExternalDiameterDTO : BaseDictionaryDTO
    {
        [DataMember]
        public int InternalDiameterId { get; set; }

        [DataMember]
        public double ExternalDiameter { get; set; }

        [DataMember]
        public double WallThickness { get; set; }

    }
}
