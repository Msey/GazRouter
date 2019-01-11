using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Repairs.Plan
{
    [DataContract]
    public class RepairUpdateDTO : BaseDto<int>
    {
        [DataMember]
        public DateTime UpdateDate { get; set; }

        [DataMember]
        public string Action { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string SiteName { get; set; }
    }
}
