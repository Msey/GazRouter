using System.Runtime.Serialization;

namespace GazRouter.DTO.ObjectModel.Segment
{
    [DataContract]
    public class DiameterSegmentDTO : BaseSegmentDTO
    {
        [DataMember]
        public int DiameterId { get; set; }

        [DataMember]
        public int ExternalDiameterId { get; set; }

        [DataMember]
        public string DiameterName { get; set; }

        [DataMember]
        public int DiameterReal { get; set; }

        [DataMember]
        public int DiameterConv { get; set; }

        [DataMember]
        public double ExternalDiameter { get; set; }

        [DataMember]
        public double WallThickness { get; set; }
    }
}