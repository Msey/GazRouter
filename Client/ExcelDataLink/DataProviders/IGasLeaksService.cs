using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.GasLeaks;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.GasLeaks  
{
    [ServiceContract]
    public interface IGasLeaksService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetLeaks(GetLeaksParameterSet parameters, AsyncCallback callback, object state);
        List<LeakDTO> EndGetLeaks(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteLeak(int parameters, AsyncCallback callback, object state);
        void EndDeleteLeak(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddLeak(AddLeakParameterSet parameters, AsyncCallback callback, object state);
        int EndAddLeak(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditLeak(EditLeakParameterSet parameters, AsyncCallback callback, object state);
        void EndEditLeak(IAsyncResult result);
    }


    public class GasLeaksServiceProxy : DataProviderBase<IGasLeaksService>
	{
        protected override string ServiceUri
        {
            get { return "/GasLeaks/GasLeaksService.svc"; }
        }

        public Task<List<LeakDTO>> GetLeaksAsync(GetLeaksParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<LeakDTO>,GetLeaksParameterSet>(channel, channel.BeginGetLeaks, channel.EndGetLeaks, parameters);
        }

        public Task DeleteLeakAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteLeak, channel.EndDeleteLeak, parameters);
        }

        public Task<int> AddLeakAsync(AddLeakParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddLeakParameterSet>(channel, channel.BeginAddLeak, channel.EndAddLeak, parameters);
        }

        public Task EditLeakAsync(EditLeakParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditLeak, channel.EndEditLeak, parameters);
        }

    }
}
