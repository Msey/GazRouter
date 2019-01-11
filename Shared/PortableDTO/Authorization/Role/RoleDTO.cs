
using System.Runtime.Serialization;

namespace GazRouter.DTO.Authorization.Role
{
    [DataContract]
    public class RoleDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}
