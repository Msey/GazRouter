using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DataServices.Infrastructure.Authorization;
using GazRouter.Log;
namespace GazRouter.DataServices.Infrastructure.Sessions
{
    public static class SessionManager
    {
        public static readonly string SystemUserLogin = "SystemUser";

        private static readonly ConcurrentDictionary<string, UserSession> Sessions = new ConcurrentDictionary<string, UserSession>();

        public static UserSession GetSession(string login, Func<string, UserSession> addFunc = null)
        {
            if (addFunc == null)
            {
                UserSession session;
                return Sessions.TryGetValue(login, out session) ? session : null;
            }

            return Sessions.GetOrAdd(login, addFunc);
        }

        public static void RemoveUserSessionByUserId(int userId)
        {
            var session = Sessions.Values.SingleOrDefault(rec => rec.User.Id == userId);
            if (session != null)
            {
                var login = session.User.Login;
                Sessions.TryRemove(login, out session);
            }
        }

        public static void UpdateRolesForActiveSessions(Func<int, IEnumerable<ServiceAction>> getActions)
        {

            var sessions = Sessions.Values.Where(s => s.User.Login != SystemUserLogin).ToList();
            foreach (var userSession in sessions)
            {
                var actions = getActions(userSession.User.Id);
                userSession.UpdateActions(actions);
            }
        }

        public static IEnumerable<UserSession> GetActiveSessions()
        {
            return Sessions.Values;
        }
    }
}