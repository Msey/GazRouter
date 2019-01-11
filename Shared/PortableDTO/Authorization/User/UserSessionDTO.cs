using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Authorization.User
{
    [DataContract]
    public class UserSessionDTO
    {
        [DataMember]
        public UserDTO User { get; set; }

        [DataMember]
        public DateTime LastActionTime { get; set; }
    }
}