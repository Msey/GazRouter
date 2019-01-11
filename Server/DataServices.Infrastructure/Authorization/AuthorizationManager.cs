using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using GazRouter.DAL.Authorization.Action;
using GazRouter.DAL.Authorization.User;
using GazRouter.DAL.Core;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DTO.Authorization.User;
using GazRouter.Log;

namespace GazRouter.DataServices.Infrastructure.Authorization
{
    public class AuthorizationManager : AuthorizationManagerVersion
    {
        private readonly MyLogger _logger;

        public AuthorizationManager()
        {
            _logger = new MyLogger("mainLogger");
        }

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            var login = operationContext.GetLogin();
            if (!base.CheckAccessCore(operationContext)) return false;
            var session = SessionManager.GetSession(login, TryCreateSession);
            if (session == null)
            {
                operationContext.SetAccessDeniedReason(AccessDeniedReason.UserNotRegistered);
                return false;
            }
            //var action = ServiceActions.AllActions.First(item => item.ActionPath == actionPath);
            //if (!action.IsAllowedByDefault && session.AllowedActions.All(item => item.ActionPath != actionPath))
            //{
            //    operationContext.SetAccessDeniedReason(AccessDeniedReason.Rights);
            //    return false;
            //}
            var dateTime = DateTime.Now;
            session.LastActionTime = dateTime;
            return true;
        }

        private UserSession TryCreateSession(string login)
        {
            UserSession userSession = null;
            var isAdmin =
                AppSettingsManager.AdminLogins.Any(
                    l => String.Equals(l, login, StringComparison.CurrentCultureIgnoreCase));
            using (var context = OpenDbContext())
            {
                var user = new GetUserByLoginQuery(context).Execute(login);
                if (isAdmin)
                {
                    if (user == null)
                    {
                        var adUser =
                            AdManager.GetAdUsers()
                                .Single(u => String.Equals(u.Login, login, StringComparison.CurrentCultureIgnoreCase));
                        var userId = new AddUserCommand(context).Execute(new AddUserParameterSet
                        {
                            Login = login,
                            Description = "Администратор системы",
                            FullName = adUser.DisplayName,
                            SiteId = AppSettingsManager.CurrentEnterpriseId,
                            SettingsUser = new UserSettings()
                        });
                        user = new GetUserByIdQuery(context).Execute(userId);
                    }
                }

                if (user != null)
                {
                    IEnumerable<ServiceAction> allowedActions;
                    if (isAdmin)
                    {
                        allowedActions = ServiceActions.AllActions;
                    }
                    else
                    {
                        var actionDtos = new GetActionsByUserIdQuery(context).Execute(user.Id);
                        allowedActions = ServiceActions.AllActions.Join(actionDtos, a1 => a1.ActionPath, a2 => a2.Path,
                            (a1, a2) => a1);
                    }

                    userSession = new UserSession(user);
                    userSession.AllowedActions.AddRange(allowedActions);
                }
            }
            return userSession;
        }

        private ExecutionContextReal OpenDbContext()
        {
            return DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, _logger);
        }
    }
}