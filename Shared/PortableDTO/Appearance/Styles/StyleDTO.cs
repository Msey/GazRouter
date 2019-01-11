using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Appearance.Styles
{
    [DataContract]
    public class StyleDTO
    {
        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public int SchemeVersionId { get; set; }

        [DataMember]
        public int Color { get; set; }

        [DataMember]
        public int Size { get; set; }
    }
}
