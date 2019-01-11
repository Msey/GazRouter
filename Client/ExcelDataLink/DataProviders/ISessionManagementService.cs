using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Authorization.User;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.Authorization  
{
    [ServiceContract]
    public interface ISessionManagementService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetActiveSessions(object parameters, AsyncCallback callback, object state);
        List<UserSessionDTO> EndGetActiveSessions(IAsyncResult result);
    }


    public class SessionManagementServiceProxy : DataProviderBase<ISessionManagementService>
	{
        protected override string ServiceUri
        {
            get { return "/Authorization/SessionManagementService.svc"; }
        }

        public Task<List<UserSessionDTO>> GetActiveSessionsAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<UserSessionDTO>>(channel, channel.BeginGetActiveSessions, channel.EndGetActiveSessions);
        }

    }
}
