using System.Runtime.Serialization;
namespace GazRouter.DTO.Authorization.Role
{
    [DataContract]
    public class PermissionDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int RoleId { get; set; }
        [DataMember]
        public int ItemId { get; set; }
        [DataMember]
        public int? ParentId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Permission  { get; set; }
    }
}
