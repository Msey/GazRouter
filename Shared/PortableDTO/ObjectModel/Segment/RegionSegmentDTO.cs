using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.ObjectModel.Segment
{
    [DataContract]
    public class RegionSegmentDTO : BaseSegmentDTO
    {
        [DataMember]
        public int RegionID { get; set; }

        public string RegionName { get; set; }
    }
}
