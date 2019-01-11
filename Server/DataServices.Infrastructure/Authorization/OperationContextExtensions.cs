using System.ServiceModel;
using System.Text.RegularExpressions;
namespace GazRouter.DataServices.Infrastructure.Authorization
{
    public static class OperationContextExtensions
    {
        private static T GetHeader<T>(this OperationContext operationContext, string key)
        {
            int index = operationContext.IncomingMessageHeaders.FindHeader(key, string.Empty);
             return index >= 0 ? operationContext.IncomingMessageHeaders.GetHeader<T>(index) : default(T);
        }

        private const string RegexPattern = "/(?<serviceName>[^/]*)/(?<actionName>[^/]*)$";

        public static string GetActionPath(this OperationContext operationContext)
        {
            var action = operationContext.IncomingMessageHeaders.Action;

            var match = Regex.Match(action, RegexPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            return match.Success ? string.Format("{0}/{1}", match.Groups["serviceName"].Value, match.Groups["actionName"].Value) : null;
        }

        public static string GetLogin(this OperationContext operationContext)
        {
#if DEBUG
//           return "dev\\v.semenov";
//           return "dev\\a.sidorenko";
            //            return "dev\\m.shiryaev";
            //            return "dev\\l.balashov";
            //            return "dev\\a.kosov";
            //            return "dev\\l.yudnikova";
            //            return "dev\\t.smorodinova";
#endif
            return operationContext.ServiceSecurityContext.WindowsIdentity.Name.ToLower();
        }

        public static string GetClientVersion(this OperationContext operationContext)
        {
            return GetHeader<string>(operationContext, "ClientVersion");
        }

        public static AccessDeniedReason GetAccessDeniedReason(this OperationContext operationContext)
        {
            return GetHeader<AccessDeniedReason>(operationContext, "AccessDeniedReason");
        }

        public static void SetAccessDeniedReason(this OperationContext operationContext, AccessDeniedReason reason)
        {
            var mh = new MessageHeader<AccessDeniedReason>(reason);
            var untyped = mh.GetUntypedHeader("AccessDeniedReason", string.Empty);
            operationContext.IncomingMessageHeaders.Add(untyped);
        }
    }

    public enum AccessDeniedReason
    {
        Version,
        Rights,
        UserNotRegistered
    }
}