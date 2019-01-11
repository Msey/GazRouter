using System;
using System.Collections.Generic;
using GazRouter.DataServices.Infrastructure.Authorization;
using GazRouter.DTO.Authorization.User;

namespace GazRouter.DataServices.Infrastructure.Sessions
{
    public class UserSession
    {
        public UserDTO User { get; set; }
        public DateTime LastActionTime { get; set; }
        public List<ServiceAction> AllowedActions { get; private set; }

        public UserSession(UserDTO user)
        {
            User = user;
            AllowedActions = new List<ServiceAction>();
        }

        public void UpdateActions(IEnumerable<ServiceAction> actions)
        {
            AllowedActions = new List<ServiceAction>(actions);
        }

    }
}