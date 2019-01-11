using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Balances.DayBalance;
using GazRouter.DTO.Balances.Docs;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.OperConsumers;
using GazRouter.DTO.Balances.Plan;
using GazRouter.DTO.Balances.Routes;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.Balances  
{
    [ServiceContract]
    public interface IBalancesService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetGasOwnersList(int? parameters, AsyncCallback callback, object state);
        List<GasOwnerDTO> EndGetGasOwnersList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddGasOwners(AddGasOwnerParameterSet parameters, AsyncCallback callback, object state);
        int EndAddGasOwners(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditGasOwners(EditGasOwnerParameterSet parameters, AsyncCallback callback, object state);
        void EndEditGasOwners(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteGasOwners(int parameters, AsyncCallback callback, object state);
        void EndDeleteGasOwners(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditSortOrderGasOwners(SetGasOwnerSortOrderParameterSet parameters, AsyncCallback callback, object state);
        void EndEditSortOrderGasOwners(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPlan(GetPlanParameterSet parameters, AsyncCallback callback, object state);
        PlanDto EndGetPlan(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginUpdatePlan(UpdatePlanParameterSet parameters, AsyncCallback callback, object state);
        void EndUpdatePlan(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDayBalancesList(DateTime parameters, AsyncCallback callback, object state);
        DayBalancesDTO EndGetDayBalancesList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginUpdateDayBalances(UpdateDayBalanceParameterSet parameters, AsyncCallback callback, object state);
        void EndUpdateDayBalances(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetOperConsumers(object parameters, AsyncCallback callback, object state);
        List<OperConsumerDTO> EndGetOperConsumers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddOperConsumer(AddEditOperConsumerParameterSet parameters, AsyncCallback callback, object state);
        int EndAddOperConsumer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditOperConsumer(AddEditOperConsumerParameterSet parameters, AsyncCallback callback, object state);
        void EndEditOperConsumer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteOperConsumer(int parameters, AsyncCallback callback, object state);
        void EndDeleteOperConsumer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDayInfoBalancesList(DateTime parameters, AsyncCallback callback, object state);
        DayInfoBalancesDTO EndGetDayInfoBalancesList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetRoute(SetRouteParameterSet parameters, AsyncCallback callback, object state);
        int EndSetRoute(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetRoutesList(GetRoutesParameterSet parameters, AsyncCallback callback, object state);
        List<RouteDTO> EndGetRoutesList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddRouteSection(AddRouteSectionParameterSet parameters, AsyncCallback callback, object state);
        int EndAddRouteSection(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddAllRoutes(GetRoutesParameterSet parameters, AsyncCallback callback, object state);
        void EndAddAllRoutes(IAsyncResult result);

        
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddGasOwners2Consumer(BaseGasOwner2ConsumerParameterSet parameters, AsyncCallback callback, object state);
        void EndAddGasOwners2Consumer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteGasOwners2Consumer(BaseGasOwner2ConsumerParameterSet parameters, AsyncCallback callback, object state);
        void EndDeleteGasOwners2Consumer(IAsyncResult result);
    }


    public class BalancesServiceProxy : DataProviderBase<IBalancesService>
	{
        protected override string ServiceUri
        {
            get { return "/Balances/BalancesService.svc"; }
        }

        public Task<List<GasOwnerDTO>> GetGasOwnersListAsync(int? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<GasOwnerDTO>,int?>(channel, channel.BeginGetGasOwnersList, channel.EndGetGasOwnersList, parameters);
        }

        public Task<int> AddGasOwnersAsync(AddGasOwnerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddGasOwnerParameterSet>(channel, channel.BeginAddGasOwners, channel.EndAddGasOwners, parameters);
        }

        public Task EditGasOwnersAsync(EditGasOwnerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditGasOwners, channel.EndEditGasOwners, parameters);
        }

        public Task DeleteGasOwnersAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteGasOwners, channel.EndDeleteGasOwners, parameters);
        }

        public Task EditSortOrderGasOwnersAsync(SetGasOwnerSortOrderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditSortOrderGasOwners, channel.EndEditSortOrderGasOwners, parameters);
        }

        public Task<PlanDto> GetPlanAsync(GetPlanParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<PlanDto,GetPlanParameterSet>(channel, channel.BeginGetPlan, channel.EndGetPlan, parameters);
        }

        public Task UpdatePlanAsync(UpdatePlanParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginUpdatePlan, channel.EndUpdatePlan, parameters);
        }

        public Task<DayBalancesDTO> GetDayBalancesListAsync(DateTime parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<DayBalancesDTO,DateTime>(channel, channel.BeginGetDayBalancesList, channel.EndGetDayBalancesList, parameters);
        }

        public Task UpdateDayBalancesAsync(UpdateDayBalanceParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginUpdateDayBalances, channel.EndUpdateDayBalances, parameters);
        }

        public Task<List<OperConsumerDTO>> GetOperConsumersAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<OperConsumerDTO>>(channel, channel.BeginGetOperConsumers, channel.EndGetOperConsumers);
        }

        public Task<int> AddOperConsumerAsync(AddEditOperConsumerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddEditOperConsumerParameterSet>(channel, channel.BeginAddOperConsumer, channel.EndAddOperConsumer, parameters);
        }

        public Task EditOperConsumerAsync(AddEditOperConsumerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditOperConsumer, channel.EndEditOperConsumer, parameters);
        }

        public Task DeleteOperConsumerAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteOperConsumer, channel.EndDeleteOperConsumer, parameters);
        }

        public Task<DayInfoBalancesDTO> GetDayInfoBalancesListAsync(DateTime parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<DayInfoBalancesDTO,DateTime>(channel, channel.BeginGetDayInfoBalancesList, channel.EndGetDayInfoBalancesList, parameters);
        }

        public Task<int> SetRouteAsync(SetRouteParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,SetRouteParameterSet>(channel, channel.BeginSetRoute, channel.EndSetRoute, parameters);
        }

        public Task<List<RouteDTO>> GetRoutesListAsync(GetRoutesParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<RouteDTO>,GetRoutesParameterSet>(channel, channel.BeginGetRoutesList, channel.EndGetRoutesList, parameters);
        }

        public Task<int> AddRouteSectionAsync(AddRouteSectionParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddRouteSectionParameterSet>(channel, channel.BeginAddRouteSection, channel.EndAddRouteSection, parameters);
        }

        public Task AddAllRoutesAsync(GetRoutesParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddAllRoutes, channel.EndAddAllRoutes, parameters);
        }
        

        public Task AddGasOwners2ConsumerAsync(BaseGasOwner2ConsumerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddGasOwners2Consumer, channel.EndAddGasOwners2Consumer, parameters);
        }

        public Task DeleteGasOwners2ConsumerAsync(BaseGasOwner2ConsumerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteGasOwners2Consumer, channel.EndDeleteGasOwners2Consumer, parameters);
        }

    }
}
