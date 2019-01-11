using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Balances.BalanceGroups;
using GazRouter.DTO.Balances.BalanceMeasurings;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.DayBalance;
using GazRouter.DTO.Balances.DistrNetworks;
using GazRouter.DTO.Balances.Docs;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.InputStates;
using GazRouter.DTO.Balances.MiscTab;
using GazRouter.DTO.Balances.MonthAlgorithms;
using GazRouter.DTO.Balances.Routes;
using GazRouter.DTO.Balances.Routes.Exceptions;
using GazRouter.DTO.Balances.SortOrder;
using GazRouter.DTO.Balances.Swaps;
using GazRouter.DTO.Balances.Transport;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.PropertyValues;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.Balances  
{
    [ServiceContract]
    public interface IBalancesService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetGasOwnerList(int? parameters, AsyncCallback callback, object state);
        List<GasOwnerDTO> EndGetGasOwnerList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddGasOwner(AddGasOwnerParameterSet parameters, AsyncCallback callback, object state);
        int EndAddGasOwner(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditGasOwner(EditGasOwnerParameterSet parameters, AsyncCallback callback, object state);
        void EndEditGasOwner(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteGasOwner(int parameters, AsyncCallback callback, object state);
        void EndDeleteGasOwner(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetGasOwnerSortOrder(SetGasOwnerSortOrderParameterSet parameters, AsyncCallback callback, object state);
        void EndSetGasOwnerSortOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetGasOwnerDisableList(object parameters, AsyncCallback callback, object state);
        List<GasOwnerDisableDTO> EndGetGasOwnerDisableList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetGasOwnerDisable(SetGasOwnerDisableParameterSet parameters, AsyncCallback callback, object state);
        void EndSetGasOwnerDisable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetGasOwnerSystem(SetGasOwnerSystemParameterSet parameters, AsyncCallback callback, object state);
        void EndSetGasOwnerSystem(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetContract(GetContractListParameterSet parameters, AsyncCallback callback, object state);
        ContractDTO EndGetContract(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetBalanceValues(int parameters, AsyncCallback callback, object state);
        List<BalanceValueDTO> EndGetBalanceValues(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSaveBalanceValues(SaveBalanceValuesParameterSet parameters, AsyncCallback callback, object state);
        void EndSaveBalanceValues(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetBalanceValue(SetBalanceValueParameterSet parameters, AsyncCallback callback, object state);
        void EndSetBalanceValue(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginClearBalanceValues(ClearBalanceValuesParameterSet parameters, AsyncCallback callback, object state);
        void EndClearBalanceValues(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetValueSwaps(int parameters, AsyncCallback callback, object state);
        List<SwapDTO> EndGetValueSwaps(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetRoute(SetRouteParameterSet parameters, AsyncCallback callback, object state);
        int EndSetRoute(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetRoutesList(GetRouteListParameterSet parameters, AsyncCallback callback, object state);
        List<RouteDTO> EndGetRoutesList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddRouteSection(AddRouteSectionParameterSet parameters, AsyncCallback callback, object state);
        int EndAddRouteSection(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddAllRoutes(GetRouteListParameterSet parameters, AsyncCallback callback, object state);
        void EndAddAllRoutes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddRouteException(AddRouteExceptionParameterSet parameters, AsyncCallback callback, object state);
        int EndAddRouteException(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditRouteException(EditRouteExceptionParameterSet parameters, AsyncCallback callback, object state);
        void EndEditRouteException(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteRouteException(int parameters, AsyncCallback callback, object state);
        void EndDeleteRouteException(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDocList(int? parameters, AsyncCallback callback, object state);
        List<DocDTO> EndGetDocList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddDoc(AddDocParameterSet parameters, AsyncCallback callback, object state);
        int EndAddDoc(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditDoc(EditDocParameterSet parameters, AsyncCallback callback, object state);
        void EndEditDoc(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteDoc(int parameters, AsyncCallback callback, object state);
        void EndDeleteDoc(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetTransportList(HandleTransportListParameterSet parameters, AsyncCallback callback, object state);
        List<TransportDTO> EndGetTransportList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginCalculateTransportList(HandleTransportListParameterSet parameters, AsyncCallback callback, object state);
        void EndCalculateTransportList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginCLearTransportResults(HandleTransportListParameterSet parameters, AsyncCallback callback, object state);
        void EndCLearTransportResults(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSortOrderList(object parameters, AsyncCallback callback, object state);
        List<BalSortOrderDTO> EndGetSortOrderList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetSortOrder(SetBalSortOrderParameterSet parameters, AsyncCallback callback, object state);
        void EndSetSortOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetMiscTabEntityList(int parameters, AsyncCallback callback, object state);
        List<CommonEntityDTO> EndGetMiscTabEntityList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddMiscTabEntity(AddRemoveMiscTabEntityParameterSet parameters, AsyncCallback callback, object state);
        void EndAddMiscTabEntity(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveMiscTabEntity(AddRemoveMiscTabEntityParameterSet parameters, AsyncCallback callback, object state);
        void EndRemoveMiscTabEntity(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetBalanceGroupList(int parameters, AsyncCallback callback, object state);
        List<BalanceGroupDTO> EndGetBalanceGroupList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddBalanceGroup(AddBalanceGroupParameterSet parameters, AsyncCallback callback, object state);
        int EndAddBalanceGroup(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditBalanceGroup(EditBalanceGroupParameterSet parameters, AsyncCallback callback, object state);
        void EndEditBalanceGroup(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveBalanceGroup(int parameters, AsyncCallback callback, object state);
        void EndRemoveBalanceGroup(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDistrNetworkList(object parameters, AsyncCallback callback, object state);
        List<DistrNetworkDTO> EndGetDistrNetworkList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddDistrNetwork(AddDistrNetworkParameterSet parameters, AsyncCallback callback, object state);
        int EndAddDistrNetwork(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditDistrNetwork(EditDistrNetworkParameterSet parameters, AsyncCallback callback, object state);
        void EndEditDistrNetwork(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveDistrNetwork(int parameters, AsyncCallback callback, object state);
        void EndRemoveDistrNetwork(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetInputStateList(GetInputStateListParameterSet parameters, AsyncCallback callback, object state);
        List<InputStateDTO> EndGetInputStateList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetInputState(SetInputStateParameterSet parameters, AsyncCallback callback, object state);
        void EndSetInputState(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(PropertyValueDoubleDTO))] 
        [ServiceKnownType(typeof(PropertyValueEmptyDTO))] 
        IAsyncResult BeginGetDayBalanceData(GetDayBalanceDataParameterSet parameters, AsyncCallback callback, object state);
        DayBalanceDataDTO EndGetDayBalanceData(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRunDivideVolumeAlgorithm(DivideVolumeAlgorithmParameterSet parameters, AsyncCallback callback, object state);
        void EndRunDivideVolumeAlgorithm(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRunOwnersDaySumAlgorithm(OwnersDaySumAlgorithmParameterSet parameters, AsyncCallback callback, object state);
        void EndRunOwnersDaySumAlgorithm(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRunMoveGasSupplyAlgorithm(int parameters, AsyncCallback callback, object state);
        void EndRunMoveGasSupplyAlgorithm(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginMoveAuxCosts(MoveAuxCostsParameterSet parameters, AsyncCallback callback, object state);
        void EndMoveAuxCosts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginMoveValuesToOtherVersion(MoveValuesToOtherVersionParameterSet parameters, AsyncCallback callback, object state);
        void EndMoveValuesToOtherVersion(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(PropertyValueDoubleDTO))] 
        [ServiceKnownType(typeof(PropertyValueStringDTO))] 
        [ServiceKnownType(typeof(PropertyValueDateDTO))] 
        [ServiceKnownType(typeof(PropertyValueEmptyDTO))] 
        IAsyncResult BeginGetBalanceMeasurings(GetBalanceMeasuringsParameterSet parameters, AsyncCallback callback, object state);
        Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> EndGetBalanceMeasurings(IAsyncResult result);
    }

	public interface IBalancesServiceProxy
	{

        Task<List<GasOwnerDTO>> GetGasOwnerListAsync(int? parameters);

        Task<int> AddGasOwnerAsync(AddGasOwnerParameterSet parameters);

        Task EditGasOwnerAsync(EditGasOwnerParameterSet parameters);

        Task DeleteGasOwnerAsync(int parameters);

        Task SetGasOwnerSortOrderAsync(SetGasOwnerSortOrderParameterSet parameters);

        Task<List<GasOwnerDisableDTO>> GetGasOwnerDisableListAsync();

        Task SetGasOwnerDisableAsync(SetGasOwnerDisableParameterSet parameters);

        Task SetGasOwnerSystemAsync(SetGasOwnerSystemParameterSet parameters);

        Task<ContractDTO> GetContractAsync(GetContractListParameterSet parameters);

        Task<List<BalanceValueDTO>> GetBalanceValuesAsync(int parameters);

        Task SaveBalanceValuesAsync(SaveBalanceValuesParameterSet parameters);

        Task SetBalanceValueAsync(SetBalanceValueParameterSet parameters);

        Task ClearBalanceValuesAsync(ClearBalanceValuesParameterSet parameters);

        Task<List<SwapDTO>> GetValueSwapsAsync(int parameters);

        Task<int> SetRouteAsync(SetRouteParameterSet parameters);

        Task<List<RouteDTO>> GetRoutesListAsync(GetRouteListParameterSet parameters);

        Task<int> AddRouteSectionAsync(AddRouteSectionParameterSet parameters);

        Task AddAllRoutesAsync(GetRouteListParameterSet parameters);

        Task<int> AddRouteExceptionAsync(AddRouteExceptionParameterSet parameters);

        Task EditRouteExceptionAsync(EditRouteExceptionParameterSet parameters);

        Task DeleteRouteExceptionAsync(int parameters);

        Task<List<DocDTO>> GetDocListAsync(int? parameters);

        Task<int> AddDocAsync(AddDocParameterSet parameters);

        Task EditDocAsync(EditDocParameterSet parameters);

        Task DeleteDocAsync(int parameters);

        Task<List<TransportDTO>> GetTransportListAsync(HandleTransportListParameterSet parameters);

        Task CalculateTransportListAsync(HandleTransportListParameterSet parameters);

        Task CLearTransportResultsAsync(HandleTransportListParameterSet parameters);

        Task<List<BalSortOrderDTO>> GetSortOrderListAsync();

        Task SetSortOrderAsync(SetBalSortOrderParameterSet parameters);

        Task<List<CommonEntityDTO>> GetMiscTabEntityListAsync(int parameters);

        Task AddMiscTabEntityAsync(AddRemoveMiscTabEntityParameterSet parameters);

        Task RemoveMiscTabEntityAsync(AddRemoveMiscTabEntityParameterSet parameters);

        Task<List<BalanceGroupDTO>> GetBalanceGroupListAsync(int parameters);

        Task<int> AddBalanceGroupAsync(AddBalanceGroupParameterSet parameters);

        Task EditBalanceGroupAsync(EditBalanceGroupParameterSet parameters);

        Task RemoveBalanceGroupAsync(int parameters);

        Task<List<DistrNetworkDTO>> GetDistrNetworkListAsync();

        Task<int> AddDistrNetworkAsync(AddDistrNetworkParameterSet parameters);

        Task EditDistrNetworkAsync(EditDistrNetworkParameterSet parameters);

        Task RemoveDistrNetworkAsync(int parameters);

        Task<List<InputStateDTO>> GetInputStateListAsync(GetInputStateListParameterSet parameters);

        Task SetInputStateAsync(SetInputStateParameterSet parameters);

        Task<DayBalanceDataDTO> GetDayBalanceDataAsync(GetDayBalanceDataParameterSet parameters);

        Task RunDivideVolumeAlgorithmAsync(DivideVolumeAlgorithmParameterSet parameters);

        Task RunOwnersDaySumAlgorithmAsync(OwnersDaySumAlgorithmParameterSet parameters);

        Task RunMoveGasSupplyAlgorithmAsync(int parameters);

        Task MoveAuxCostsAsync(MoveAuxCostsParameterSet parameters);

        Task MoveValuesToOtherVersionAsync(MoveValuesToOtherVersionParameterSet parameters);

        Task<Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>>> GetBalanceMeasuringsAsync(GetBalanceMeasuringsParameterSet parameters);

    }

    public sealed class BalancesServiceProxy : DataProviderBase<IBalancesService>, IBalancesServiceProxy
	{
        protected override string ServiceUri => "/Balances/BalancesService.svc";
      


        public Task<List<GasOwnerDTO>> GetGasOwnerListAsync(int? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<GasOwnerDTO>,int?>(channel, channel.BeginGetGasOwnerList, channel.EndGetGasOwnerList, parameters);
        }

        public Task<int> AddGasOwnerAsync(AddGasOwnerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddGasOwnerParameterSet>(channel, channel.BeginAddGasOwner, channel.EndAddGasOwner, parameters);
        }

        public Task EditGasOwnerAsync(EditGasOwnerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditGasOwner, channel.EndEditGasOwner, parameters);
        }

        public Task DeleteGasOwnerAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteGasOwner, channel.EndDeleteGasOwner, parameters);
        }

        public Task SetGasOwnerSortOrderAsync(SetGasOwnerSortOrderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetGasOwnerSortOrder, channel.EndSetGasOwnerSortOrder, parameters);
        }

        public Task<List<GasOwnerDisableDTO>> GetGasOwnerDisableListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<GasOwnerDisableDTO>>(channel, channel.BeginGetGasOwnerDisableList, channel.EndGetGasOwnerDisableList);
        }

        public Task SetGasOwnerDisableAsync(SetGasOwnerDisableParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetGasOwnerDisable, channel.EndSetGasOwnerDisable, parameters);
        }

        public Task SetGasOwnerSystemAsync(SetGasOwnerSystemParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetGasOwnerSystem, channel.EndSetGasOwnerSystem, parameters);
        }

        public Task<ContractDTO> GetContractAsync(GetContractListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<ContractDTO,GetContractListParameterSet>(channel, channel.BeginGetContract, channel.EndGetContract, parameters);
        }

        public Task<List<BalanceValueDTO>> GetBalanceValuesAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<BalanceValueDTO>,int>(channel, channel.BeginGetBalanceValues, channel.EndGetBalanceValues, parameters);
        }

        public Task SaveBalanceValuesAsync(SaveBalanceValuesParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSaveBalanceValues, channel.EndSaveBalanceValues, parameters);
        }

        public Task SetBalanceValueAsync(SetBalanceValueParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetBalanceValue, channel.EndSetBalanceValue, parameters);
        }

        public Task ClearBalanceValuesAsync(ClearBalanceValuesParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginClearBalanceValues, channel.EndClearBalanceValues, parameters);
        }

        public Task<List<SwapDTO>> GetValueSwapsAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<SwapDTO>,int>(channel, channel.BeginGetValueSwaps, channel.EndGetValueSwaps, parameters);
        }

        public Task<int> SetRouteAsync(SetRouteParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,SetRouteParameterSet>(channel, channel.BeginSetRoute, channel.EndSetRoute, parameters);
        }

        public Task<List<RouteDTO>> GetRoutesListAsync(GetRouteListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<RouteDTO>,GetRouteListParameterSet>(channel, channel.BeginGetRoutesList, channel.EndGetRoutesList, parameters);
        }

        public Task<int> AddRouteSectionAsync(AddRouteSectionParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddRouteSectionParameterSet>(channel, channel.BeginAddRouteSection, channel.EndAddRouteSection, parameters);
        }

        public Task AddAllRoutesAsync(GetRouteListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddAllRoutes, channel.EndAddAllRoutes, parameters);
        }

        public Task<int> AddRouteExceptionAsync(AddRouteExceptionParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddRouteExceptionParameterSet>(channel, channel.BeginAddRouteException, channel.EndAddRouteException, parameters);
        }

        public Task EditRouteExceptionAsync(EditRouteExceptionParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditRouteException, channel.EndEditRouteException, parameters);
        }

        public Task DeleteRouteExceptionAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteRouteException, channel.EndDeleteRouteException, parameters);
        }

        public Task<List<DocDTO>> GetDocListAsync(int? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DocDTO>,int?>(channel, channel.BeginGetDocList, channel.EndGetDocList, parameters);
        }

        public Task<int> AddDocAsync(AddDocParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddDocParameterSet>(channel, channel.BeginAddDoc, channel.EndAddDoc, parameters);
        }

        public Task EditDocAsync(EditDocParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditDoc, channel.EndEditDoc, parameters);
        }

        public Task DeleteDocAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteDoc, channel.EndDeleteDoc, parameters);
        }

        public Task<List<TransportDTO>> GetTransportListAsync(HandleTransportListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<TransportDTO>,HandleTransportListParameterSet>(channel, channel.BeginGetTransportList, channel.EndGetTransportList, parameters);
        }

        public Task CalculateTransportListAsync(HandleTransportListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginCalculateTransportList, channel.EndCalculateTransportList, parameters);
        }

        public Task CLearTransportResultsAsync(HandleTransportListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginCLearTransportResults, channel.EndCLearTransportResults, parameters);
        }

        public Task<List<BalSortOrderDTO>> GetSortOrderListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<BalSortOrderDTO>>(channel, channel.BeginGetSortOrderList, channel.EndGetSortOrderList);
        }

        public Task SetSortOrderAsync(SetBalSortOrderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetSortOrder, channel.EndSetSortOrder, parameters);
        }

        public Task<List<CommonEntityDTO>> GetMiscTabEntityListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CommonEntityDTO>,int>(channel, channel.BeginGetMiscTabEntityList, channel.EndGetMiscTabEntityList, parameters);
        }

        public Task AddMiscTabEntityAsync(AddRemoveMiscTabEntityParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddMiscTabEntity, channel.EndAddMiscTabEntity, parameters);
        }

        public Task RemoveMiscTabEntityAsync(AddRemoveMiscTabEntityParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveMiscTabEntity, channel.EndRemoveMiscTabEntity, parameters);
        }

        public Task<List<BalanceGroupDTO>> GetBalanceGroupListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<BalanceGroupDTO>,int>(channel, channel.BeginGetBalanceGroupList, channel.EndGetBalanceGroupList, parameters);
        }

        public Task<int> AddBalanceGroupAsync(AddBalanceGroupParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddBalanceGroupParameterSet>(channel, channel.BeginAddBalanceGroup, channel.EndAddBalanceGroup, parameters);
        }

        public Task EditBalanceGroupAsync(EditBalanceGroupParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditBalanceGroup, channel.EndEditBalanceGroup, parameters);
        }

        public Task RemoveBalanceGroupAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveBalanceGroup, channel.EndRemoveBalanceGroup, parameters);
        }

        public Task<List<DistrNetworkDTO>> GetDistrNetworkListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DistrNetworkDTO>>(channel, channel.BeginGetDistrNetworkList, channel.EndGetDistrNetworkList);
        }

        public Task<int> AddDistrNetworkAsync(AddDistrNetworkParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddDistrNetworkParameterSet>(channel, channel.BeginAddDistrNetwork, channel.EndAddDistrNetwork, parameters);
        }

        public Task EditDistrNetworkAsync(EditDistrNetworkParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditDistrNetwork, channel.EndEditDistrNetwork, parameters);
        }

        public Task RemoveDistrNetworkAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveDistrNetwork, channel.EndRemoveDistrNetwork, parameters);
        }

        public Task<List<InputStateDTO>> GetInputStateListAsync(GetInputStateListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<InputStateDTO>,GetInputStateListParameterSet>(channel, channel.BeginGetInputStateList, channel.EndGetInputStateList, parameters);
        }

        public Task SetInputStateAsync(SetInputStateParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetInputState, channel.EndSetInputState, parameters);
        }

        public Task<DayBalanceDataDTO> GetDayBalanceDataAsync(GetDayBalanceDataParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<DayBalanceDataDTO,GetDayBalanceDataParameterSet>(channel, channel.BeginGetDayBalanceData, channel.EndGetDayBalanceData, parameters);
        }

        public Task RunDivideVolumeAlgorithmAsync(DivideVolumeAlgorithmParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRunDivideVolumeAlgorithm, channel.EndRunDivideVolumeAlgorithm, parameters);
        }

        public Task RunOwnersDaySumAlgorithmAsync(OwnersDaySumAlgorithmParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRunOwnersDaySumAlgorithm, channel.EndRunOwnersDaySumAlgorithm, parameters);
        }

        public Task RunMoveGasSupplyAlgorithmAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRunMoveGasSupplyAlgorithm, channel.EndRunMoveGasSupplyAlgorithm, parameters);
        }

        public Task MoveAuxCostsAsync(MoveAuxCostsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginMoveAuxCosts, channel.EndMoveAuxCosts, parameters);
        }

        public Task MoveValuesToOtherVersionAsync(MoveValuesToOtherVersionParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginMoveValuesToOtherVersion, channel.EndMoveValuesToOtherVersion, parameters);
        }

        public Task<Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>>> GetBalanceMeasuringsAsync(GetBalanceMeasuringsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>>,GetBalanceMeasuringsParameterSet>(channel, channel.BeginGetBalanceMeasurings, channel.EndGetBalanceMeasurings, parameters);
        }

    }
}
