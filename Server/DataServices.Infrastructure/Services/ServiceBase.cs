using System.ServiceModel;
using GazRouter.DAL.Core;
using GazRouter.DataServices.Infrastructure.Authorization;
using GazRouter.DataServices.Infrastructure.Sessions;

namespace GazRouter.DataServices.Infrastructure.Services
{
    public abstract class ServiceBase : AnonymousServiceBase
    {
        protected static UserSession Session
        {
            get { return SessionManager.GetSession(Login); }
        }

        protected override ExecutionContextReal OpenDbContext()
        {
            return DbContextHelper.OpenDbContext(Login, _logger);
        }

        protected static string Login
        {
            get { return OperationContext.Current.GetLogin(); }
        }


    }
}