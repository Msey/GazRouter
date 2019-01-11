using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData;
using GazRouter.DTO.SeriesData.CompUnitPropertyValues;
using GazRouter.DTO.SeriesData.GasInPipes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SeriesData.Trends;
using GazRouter.DTO.SeriesData.ValueMessages;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.SeriesData  
{
    [ServiceContract]
    public interface ISeriesDataService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSeriesList(GetSeriesListParameterSet parameters, AsyncCallback callback, object state);
        List<SeriesDTO> EndGetSeriesList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSeries(GetSeriesParameterSet parameters, AsyncCallback callback, object state);
        SeriesDTO EndGetSeries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddSerie(AddSeriesParameterSet parameters, AsyncCallback callback, object state);
        SeriesDTO EndAddSerie(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPropertyValueMessageList(GetPropertyValueMessageListParameterSet parameters, AsyncCallback callback, object state);
        Dictionary<Guid, Dictionary<PropertyType, List<PropertyValueMessageDTO>>> EndGetPropertyValueMessageList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginPerformChecking(List<PerformCheckingParameterSet> parameters, AsyncCallback callback, object state);
        void EndPerformChecking(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAcceptMessage(Guid parameters, AsyncCallback callback, object state);
        void EndAcceptMessage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(PropertyValueDoubleDTO))] 
        [ServiceKnownType(typeof(PropertyValueStringDTO))] 
        [ServiceKnownType(typeof(PropertyValueDateDTO))] 
        [ServiceKnownType(typeof(PropertyValueEmptyDTO))] 
        IAsyncResult BeginGetPropertyValue(GetPropertyValueParameterSet parameters, AsyncCallback callback, object state);
        BasePropertyValueDTO EndGetPropertyValue(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(PropertyValueDoubleDTO))] 
        [ServiceKnownType(typeof(PropertyValueStringDTO))] 
        [ServiceKnownType(typeof(PropertyValueDateDTO))] 
        [ServiceKnownType(typeof(PropertyValueEmptyDTO))] 
        IAsyncResult BeginGetTrend(GetTrendParameterSet parameters, AsyncCallback callback, object state);
        TrendDTO EndGetTrend(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(PropertyValueDoubleDTO))] 
        [ServiceKnownType(typeof(PropertyValueStringDTO))] 
        [ServiceKnownType(typeof(PropertyValueDateDTO))] 
        [ServiceKnownType(typeof(PropertyValueEmptyDTO))] 
        IAsyncResult BeginGetPropertyValueList(GetPropertyValueListParameterSet parameters, AsyncCallback callback, object state);
        List<BasePropertyValueDTO> EndGetPropertyValueList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(PropertyValueDoubleDTO))] 
        [ServiceKnownType(typeof(PropertyValueStringDTO))] 
        [ServiceKnownType(typeof(PropertyValueDateDTO))] 
        [ServiceKnownType(typeof(PropertyValueEmptyDTO))] 
        IAsyncResult BeginGetEntityPropertyValueList(GetEntityPropertyValueListParameterSet parameters, AsyncCallback callback, object state);
        Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> EndGetEntityPropertyValueList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetPropertyValue(List<SetPropertyValueParameterSet> parameters, AsyncCallback callback, object state);
        void EndSetPropertyValue(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCompUnitPropertyValueList(GetCompUnitPropertyValuesParameterSet parameters, AsyncCallback callback, object state);
        Dictionary<PropertyType, PropertyValueDoubleDTO> EndGetCompUnitPropertyValueList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(SiteDTO))] 
        [ServiceKnownType(typeof(CompStationDTO))] 
        [ServiceKnownType(typeof(CompShopDTO))] 
        [ServiceKnownType(typeof(CompUnitDTO))] 
        [ServiceKnownType(typeof(DateIntervalDTO))] 
        IAsyncResult BeginGetOperatingTimeCompUnitList(DateIntervalParameterSet parameters, AsyncCallback callback, object state);
        CompUnitsOperatingTimeDto EndGetOperatingTimeCompUnitList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetGasInPipeList(GetGasInPipeListParameterSet parameters, AsyncCallback callback, object state);
        List<GasInPipeDTO> EndGetGasInPipeList(IAsyncResult result);
    }


    public class SeriesDataServiceProxy : DataProviderBase<ISeriesDataService>
	{
        protected override string ServiceUri
        {
            get { return "/SeriesData/SeriesDataService.svc"; }
        }

        public Task<List<SeriesDTO>> GetSeriesListAsync(GetSeriesListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<SeriesDTO>,GetSeriesListParameterSet>(channel, channel.BeginGetSeriesList, channel.EndGetSeriesList, parameters);
        }

        public Task<SeriesDTO> GetSeriesAsync(GetSeriesParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<SeriesDTO,GetSeriesParameterSet>(channel, channel.BeginGetSeries, channel.EndGetSeries, parameters);
        }

        public Task<SeriesDTO> AddSerieAsync(AddSeriesParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<SeriesDTO,AddSeriesParameterSet>(channel, channel.BeginAddSerie, channel.EndAddSerie, parameters);
        }

        public Task<Dictionary<Guid, Dictionary<PropertyType, List<PropertyValueMessageDTO>>>> GetPropertyValueMessageListAsync(GetPropertyValueMessageListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Dictionary<Guid, Dictionary<PropertyType, List<PropertyValueMessageDTO>>>,GetPropertyValueMessageListParameterSet>(channel, channel.BeginGetPropertyValueMessageList, channel.EndGetPropertyValueMessageList, parameters);
        }

        public Task PerformCheckingAsync(List<PerformCheckingParameterSet> parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginPerformChecking, channel.EndPerformChecking, parameters);
        }

        public Task AcceptMessageAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAcceptMessage, channel.EndAcceptMessage, parameters);
        }

        public Task<BasePropertyValueDTO> GetPropertyValueAsync(GetPropertyValueParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<BasePropertyValueDTO,GetPropertyValueParameterSet>(channel, channel.BeginGetPropertyValue, channel.EndGetPropertyValue, parameters);
        }

        public Task<TrendDTO> GetTrendAsync(GetTrendParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<TrendDTO,GetTrendParameterSet>(channel, channel.BeginGetTrend, channel.EndGetTrend, parameters);
        }

        public Task<List<BasePropertyValueDTO>> GetPropertyValueListAsync(GetPropertyValueListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<BasePropertyValueDTO>,GetPropertyValueListParameterSet>(channel, channel.BeginGetPropertyValueList, channel.EndGetPropertyValueList, parameters);
        }

        public Task<Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>>> GetEntityPropertyValueListAsync(GetEntityPropertyValueListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>>,GetEntityPropertyValueListParameterSet>(channel, channel.BeginGetEntityPropertyValueList, channel.EndGetEntityPropertyValueList, parameters);
        }

        public Task SetPropertyValueAsync(List<SetPropertyValueParameterSet> parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetPropertyValue, channel.EndSetPropertyValue, parameters);
        }

        public Task<Dictionary<PropertyType, PropertyValueDoubleDTO>> GetCompUnitPropertyValueListAsync(GetCompUnitPropertyValuesParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Dictionary<PropertyType, PropertyValueDoubleDTO>,GetCompUnitPropertyValuesParameterSet>(channel, channel.BeginGetCompUnitPropertyValueList, channel.EndGetCompUnitPropertyValueList, parameters);
        }

        public Task<CompUnitsOperatingTimeDto> GetOperatingTimeCompUnitListAsync(DateIntervalParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<CompUnitsOperatingTimeDto,DateIntervalParameterSet>(channel, channel.BeginGetOperatingTimeCompUnitList, channel.EndGetOperatingTimeCompUnitList, parameters);
        }

        public Task<List<GasInPipeDTO>> GetGasInPipeListAsync(GetGasInPipeListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<GasInPipeDTO>,GetGasInPipeListParameterSet>(channel, channel.BeginGetGasInPipeList, channel.EndGetGasInPipeList, parameters);
        }

    }
}
