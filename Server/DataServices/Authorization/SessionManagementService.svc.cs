using System.Collections.Generic;
using System.Linq;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DTO.Authorization.User;

namespace GazRouter.DataServices.Authorization
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class SessionManagementService : ISessionManagementService
    {
        public List<UserSessionDTO> GetActiveSessions()
        {
            var sessions = SessionManager.GetActiveSessions();
            return sessions.
                Select(userSession =>
                new UserSessionDTO
                {
                    LastActionTime = userSession.LastActionTime,
                    User = userSession.User
                }).ToList();
        }
    }
}
