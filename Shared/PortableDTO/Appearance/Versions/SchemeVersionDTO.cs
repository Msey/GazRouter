using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Appearance.Positions;
using GazRouter.DTO.Appearance.Styles;

namespace GazRouter.DTO.Appearance.Versions
{
    [DataContract]
    public class SchemeVersionDTO
    {
        [DataMember]
        public List<PositionDTO> Positions { get; set; }

        [DataMember]
        public List<StyleDTO> Styles { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int SchemeId { get; set; }

        [DataMember]
        public bool IsPublished { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public int CreatorId { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string SchemeName { get; set; }

        [DataMember]
        public string CreatorName { get; set; }

        [DataMember]
        public int SystemId { get; set; }

        [DataMember]
        public string Content { get; set; }
    }
}