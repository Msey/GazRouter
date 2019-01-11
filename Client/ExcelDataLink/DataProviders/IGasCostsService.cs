using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.GasCosts.Import;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.GasCosts  
{
    [ServiceContract]
    public interface IGasCostsService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetGasCostList(GetGasCostListParameterSet parameter, AsyncCallback callback, object state);
        List<GasCostDTO> EndGetGasCostList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDefaultParamValues(GetGasCostListParameterSet parameter, AsyncCallback callback, object state);
        List<DefaultParamValuesDTO> EndGetDefaultParamValues(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetDefaultParamValues(List<DefaultParamValuesDTO> parameter, AsyncCallback callback, object state);
        void EndSetDefaultParamValues(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCostTypeList(object parameters, AsyncCallback callback, object state);
        List<GasCostTypeDTO> EndGetCostTypeList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddGasCost(AddGasCostParameterSet parameters, AsyncCallback callback, object state);
        int EndAddGasCost(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteGasCost(int parameters, AsyncCallback callback, object state);
        void EndDeleteGasCost(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditGasCost(EditGasCostParameterSet parameters, AsyncCallback callback, object state);
        void EndEditGasCost(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetGasCostAccessList(GetGasCostAccessListParameterSet parameter, AsyncCallback callback, object state);
        List<GasCostAccessDTO> EndGetGasCostAccessList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginUpdateGasCostAccessList(List<GasCostAccessDTO> parameters, AsyncCallback callback, object state);
        void EndUpdateGasCostAccessList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddGasCostImportInfo(AddGasCostImportInfoParameterSet parameters, AsyncCallback callback, object state);
        int EndAddGasCostImportInfo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteGasCostImportInfo(int parameters, AsyncCallback callback, object state);
        void EndDeleteGasCostImportInfo(IAsyncResult result);
    }


    public class GasCostsServiceProxy : DataProviderBase<IGasCostsService>
	{
        protected override string ServiceUri
        {
            get { return "/GasCosts/GasCostsService.svc"; }
        }

        public Task<List<GasCostDTO>> GetGasCostListAsync(GetGasCostListParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<GasCostDTO>,GetGasCostListParameterSet>(channel, channel.BeginGetGasCostList, channel.EndGetGasCostList, parameter);
        }

        public Task<List<DefaultParamValuesDTO>> GetDefaultParamValuesAsync(GetGasCostListParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DefaultParamValuesDTO>,GetGasCostListParameterSet>(channel, channel.BeginGetDefaultParamValues, channel.EndGetDefaultParamValues, parameter);
        }

        public Task SetDefaultParamValuesAsync(List<DefaultParamValuesDTO> parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetDefaultParamValues, channel.EndSetDefaultParamValues, parameter);
        }

        public Task<List<GasCostTypeDTO>> GetCostTypeListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<GasCostTypeDTO>>(channel, channel.BeginGetCostTypeList, channel.EndGetCostTypeList);
        }

        public Task<int> AddGasCostAsync(AddGasCostParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddGasCostParameterSet>(channel, channel.BeginAddGasCost, channel.EndAddGasCost, parameters);
        }

        public Task DeleteGasCostAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteGasCost, channel.EndDeleteGasCost, parameters);
        }

        public Task EditGasCostAsync(EditGasCostParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditGasCost, channel.EndEditGasCost, parameters);
        }

        public Task<List<GasCostAccessDTO>> GetGasCostAccessListAsync(GetGasCostAccessListParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<GasCostAccessDTO>,GetGasCostAccessListParameterSet>(channel, channel.BeginGetGasCostAccessList, channel.EndGetGasCostAccessList, parameter);
        }

        public Task UpdateGasCostAccessListAsync(List<GasCostAccessDTO> parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginUpdateGasCostAccessList, channel.EndUpdateGasCostAccessList, parameters);
        }

        public Task<int> AddGasCostImportInfoAsync(AddGasCostImportInfoParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddGasCostImportInfoParameterSet>(channel, channel.BeginAddGasCostImportInfo, channel.EndAddGasCostImportInfo, parameters);
        }

        public Task DeleteGasCostImportInfoAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteGasCostImportInfo, channel.EndDeleteGasCostImportInfo, parameters);
        }

    }
}
