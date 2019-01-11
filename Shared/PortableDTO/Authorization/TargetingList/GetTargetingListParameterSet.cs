using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.Authorization.TargetingList
{
    [DataContract]
    public class GetTargetingListParameterSet
    {
        [DataMember]
        public int? EntityTypeId { get; set; }
        [DataMember]
        public Guid? SiteId { get; set; }
        [DataMember]
        public bool IsCpdd { get; set; } = false;
    }
}
