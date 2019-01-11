using System.Runtime.Serialization;

namespace GazRouter.DTO.Authorization.User
{
    [DataContract]
    public class ProfileInfoDTO
    {
        [DataMember]
        public string Name { get; set; }
    }
}