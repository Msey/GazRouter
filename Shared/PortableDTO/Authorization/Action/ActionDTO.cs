using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Authorization.Action
{
    [DataContract]
    public class ActionDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string ServiceDescription { get; set; }

        [DataMember]
        public bool IsAllowedByDefault { get; set; }

        [DataMember]
        public List<RolePermit> RolePermits { get; set; }
    }
}