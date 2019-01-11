using System;

namespace GazRouter.DTO.Authorization.User
{
    public class AddUserParameterSet
    {
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public Guid SiteId { get; set; }
        public UserSettings SettingsUser { get; set; }
    }
}
