using System.Runtime.Serialization;

namespace GazRouter.DTO.Authorization.User
{
    [DataContract]
    public class AdUserDTO
    {
        public AdUserDTO()
        {
        }

        public AdUserDTO(string login, string displayName, string description = null)
        {
            Login = login;
            DisplayName = displayName;
            Description = description;
        }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}