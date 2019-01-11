using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.ObjectModel.Segment
{
    [DataContract]
    public class SiteSegmentDTO : BaseSegmentDTO
    {
        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public string SiteName { get; set; }
    }
}
