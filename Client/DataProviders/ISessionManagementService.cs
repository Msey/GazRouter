using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Authorization.User;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.Authorization  
{
    [ServiceContract]
    public interface ISessionManagementService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetActiveSessions(object parameters, AsyncCallback callback, object state);
        List<UserSessionDTO> EndGetActiveSessions(IAsyncResult result);
    }

	public interface ISessionManagementServiceProxy
	{

        Task<List<UserSessionDTO>> GetActiveSessionsAsync();

    }

    public sealed class SessionManagementServiceProxy : DataProviderBase<ISessionManagementService>, ISessionManagementServiceProxy
	{
        protected override string ServiceUri => "/Authorization/SessionManagementService.svc";
      


        public Task<List<UserSessionDTO>> GetActiveSessionsAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<UserSessionDTO>>(channel, channel.BeginGetActiveSessions, channel.EndGetActiveSessions);
        }

    }
}
