using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.DataLoadMonitoring;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.DataLoadMonitoring  
{
    [ServiceContract]
    public interface IDataLoadService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDataLoadSiteStatisticsTechData(DateTime dt, AsyncCallback callback, object state);
        List<SiteDataLoadStatistics> EndGetDataLoadSiteStatisticsTechData(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(EntityPropertyValueStringDTO))] 
        [ServiceKnownType(typeof(EntityPropertyValueDateDTO))] 
        [ServiceKnownType(typeof(EntityPropertyValueDoubleDTO))] 
        IAsyncResult BeginGetSiteTechData(EntityPropertyValueParameterSet parameters, AsyncCallback callback, object state);
        List<BaseEntityProperty> EndGetSiteTechData(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(CompressorShopValuesChangeDTO))] 
        [ServiceKnownType(typeof(MeasureLineGasFlowChangeDTO))] 
        [ServiceKnownType(typeof(ConsumerGasFlowChangeDTO))] 
        [ServiceKnownType(typeof(ChangeModeValueDouble))] 
        [ServiceKnownType(typeof(ChangeModeValue<double?>))] 
        IAsyncResult BeginGetGasModeChangeData(GasModeChangeParameterSet parameters, AsyncCallback callback, object state);
        GasModeChangeData EndGetGasModeChangeData(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(CompressorShopValuesChangeDTO))] 
        [ServiceKnownType(typeof(MeasureLineGasFlowChangeDTO))] 
        [ServiceKnownType(typeof(ConsumerGasFlowChangeDTO))] 
        [ServiceKnownType(typeof(ChangeModeValueDouble))] 
        [ServiceKnownType(typeof(ChangeModeValue<double?>))] 
        IAsyncResult BeginGetGasModeChangeDataLastSerie(object parameters, AsyncCallback callback, object state);
        GasModeChangeData EndGetGasModeChangeDataLastSerie(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetGasSupplyDataSet(int systemId, AsyncCallback callback, object state);
        GasSupplyDataSetDTO EndGetGasSupplyDataSet(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetGasSupplyValues(DateTime dt, AsyncCallback callback, object state);
        List<GasSupplyValue> EndGetGasSupplyValues(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSumGasSupplyValuesByEnterprise(GasSupplySumParameterSet paramSet, AsyncCallback callback, object state);
        List<GasSupplySumValueDTO> EndGetSumGasSupplyValuesByEnterprise(IAsyncResult result);
    }


    public class DataLoadServiceProxy : DataProviderBase<IDataLoadService>
	{
        protected override string ServiceUri
        {
            get { return "/DataLoadMonitoring/DataLoadService.svc"; }
        }

        public Task<List<SiteDataLoadStatistics>> GetDataLoadSiteStatisticsTechDataAsync(DateTime dt)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<SiteDataLoadStatistics>,DateTime>(channel, channel.BeginGetDataLoadSiteStatisticsTechData, channel.EndGetDataLoadSiteStatisticsTechData, dt);
        }

        public Task<List<BaseEntityProperty>> GetSiteTechDataAsync(EntityPropertyValueParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<BaseEntityProperty>,EntityPropertyValueParameterSet>(channel, channel.BeginGetSiteTechData, channel.EndGetSiteTechData, parameters);
        }

        public Task<GasModeChangeData> GetGasModeChangeDataAsync(GasModeChangeParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<GasModeChangeData,GasModeChangeParameterSet>(channel, channel.BeginGetGasModeChangeData, channel.EndGetGasModeChangeData, parameters);
        }

        public Task<GasModeChangeData> GetGasModeChangeDataLastSerieAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<GasModeChangeData>(channel, channel.BeginGetGasModeChangeDataLastSerie, channel.EndGetGasModeChangeDataLastSerie);
        }

        public Task<GasSupplyDataSetDTO> GetGasSupplyDataSetAsync(int systemId)
        {
            var channel = GetChannel();
            return ExecuteAsync<GasSupplyDataSetDTO,int>(channel, channel.BeginGetGasSupplyDataSet, channel.EndGetGasSupplyDataSet, systemId);
        }

        public Task<List<GasSupplyValue>> GetGasSupplyValuesAsync(DateTime dt)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<GasSupplyValue>,DateTime>(channel, channel.BeginGetGasSupplyValues, channel.EndGetGasSupplyValues, dt);
        }

        public Task<List<GasSupplySumValueDTO>> GetSumGasSupplyValuesByEnterpriseAsync(GasSupplySumParameterSet paramSet)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<GasSupplySumValueDTO>,GasSupplySumParameterSet>(channel, channel.BeginGetSumGasSupplyValuesByEnterprise, channel.EndGetSumGasSupplyValuesByEnterprise, paramSet);
        }

    }
}
