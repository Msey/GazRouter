using System.Runtime.Serialization;

namespace GazRouter.DTO.Authorization.Action
{
    [DataContract]
    public class RolePermit
    {
        [DataMember]
        public int RoleId { get; set; }

        [DataMember]
        public string RoleName { get; set; }

        [DataMember]
        public bool IsAllowed { get; set; }
    }
}
