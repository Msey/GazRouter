using System.Reflection;
using System.ServiceModel;

namespace GazRouter.DataServices.Infrastructure.Authorization
{
    public class AuthorizationManagerVersion : ServiceAuthorizationManager
    {
        private static readonly string _serverVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            if (!AppSettingsManager.CheckClientVersion) return true;

            var clientVersion = operationContext.GetClientVersion();
            if (clientVersion == _serverVersion)
            {
                return true;
            }

            operationContext.SetAccessDeniedReason(AccessDeniedReason.Version);

            return false;
        }
    }
}