using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using GazRouter.DTO;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.Time  
{
    [ServiceContract]
    public interface ITimeService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetTimeServer(object parameters, AsyncCallback callback, object state);
        DateTime EndGetTimeServer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetServerState(Module parameters, AsyncCallback callback, object state);
        Guid EndGetServerState(IAsyncResult result);
    }

	public interface ITimeServiceProxy
	{

        Task<DateTime> GetTimeServerAsync();

        Task<Guid> GetServerStateAsync(Module parameters);

    }

    public sealed class TimeServiceProxy : DataProviderBase<ITimeService>, ITimeServiceProxy
	{
        protected override string ServiceUri => "/Time/TimeService.svc";
      


        public Task<DateTime> GetTimeServerAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<DateTime>(channel, channel.BeginGetTimeServer, channel.EndGetTimeServer);
        }

        public Task<Guid> GetServerStateAsync(Module parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,Module>(channel, channel.BeginGetServerState, channel.EndGetServerState, parameters);
        }

    }
}
