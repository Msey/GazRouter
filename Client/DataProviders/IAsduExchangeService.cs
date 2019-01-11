using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.ExchangeServices  
{
    [ServiceContract]
    public interface IAsduExchangeService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetTask(string data, AsyncCallback callback, object state);
        string EndGetTask(IAsyncResult result);
    }

	public interface IAsduExchangeServiceProxy
	{

        Task<string> GetTaskAsync(string data);

    }

    public sealed class AsduExchangeServiceProxy : DataProviderBase<IAsduExchangeService>, IAsduExchangeServiceProxy
	{
        protected override string ServiceUri => "/ExchangeServices/AsduExchangeService.svc";
      


        public Task<string> GetTaskAsync(string data)
        {
            var channel = GetChannel();
            return ExecuteAsync<string,string>(channel, channel.BeginGetTask, channel.EndGetTask, data);
        }

    }
}
