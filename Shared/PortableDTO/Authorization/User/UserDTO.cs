using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.Authorization.User
{
    [DataContract]
    public class UserDTO
    {
        private UserSettings _userSettings;

        public UserDTO()
        {
            UserSettings = new UserSettings();
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public Guid? SiteId { get; set; }

        [DataMember]
		public string SiteName { get; set; }

        [DataMember]
        public UserSettings UserSettings
        {
            get { return _userSettings; }
            set
            {

                _userSettings = value ?? new UserSettings();
            }
        }

        [DataMember]
        public int SiteLevel { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public EntityType? EntityType { get; set; }
    }
}
