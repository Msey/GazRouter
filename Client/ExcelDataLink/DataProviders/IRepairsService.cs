using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Repairs;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.Repairs  
{
    [ServiceContract]
    public interface IRepairsService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(RepairPlanCompShopDTO))] 
        IAsyncResult BeginGetRepairPlan(GetRepairPlanParameterSet parameters, AsyncCallback callback, object state);
        RepairPlanDataDTO EndGetRepairPlan(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetPlanningStage(SetPlanningStageParameterSet parameters, AsyncCallback callback, object state);
        void EndSetPlanningStage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetComplexList(GetRepairPlanParameterSet parameters, AsyncCallback callback, object state);
        List<ComplexDTO> EndGetComplexList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddComplex(AddComplexParameterSet parameters, AsyncCallback callback, object state);
        int EndAddComplex(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditComplex(EditComplexParameterSet parameters, AsyncCallback callback, object state);
        void EndEditComplex(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteComplex(int parameters, AsyncCallback callback, object state);
        void EndDeleteComplex(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddRepairToComplex(AddRepairToComplexParameterSet parameters, AsyncCallback callback, object state);
        void EndAddRepairToComplex(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginMoveComplex(EditComplexParameterSet parameters, AsyncCallback callback, object state);
        void EndMoveComplex(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddRepair(AddRepairParameterSet parameters, AsyncCallback callback, object state);
        int EndAddRepair(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditRepair(EditRepairParameterSet parameters, AsyncCallback callback, object state);
        void EndEditRepair(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditRepairDates(EditRepairDatesParameterSet parameters, AsyncCallback callback, object state);
        void EndEditRepairDates(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteRepair(int parameters, AsyncCallback callback, object state);
        void EndDeleteRepair(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetKilometerList(Guid parameters, AsyncCallback callback, object state);
        List<double> EndGetKilometerList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetRepairUpdateHistory(int parameters, AsyncCallback callback, object state);
        List<RepairUpdateDTO> EndGetRepairUpdateHistory(IAsyncResult result);
    }


    public class RepairsServiceProxy : DataProviderBase<IRepairsService>
	{
        protected override string ServiceUri
        {
            get { return "/Repairs/RepairsService.svc"; }
        }

        public Task<RepairPlanDataDTO> GetRepairPlanAsync(GetRepairPlanParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<RepairPlanDataDTO,GetRepairPlanParameterSet>(channel, channel.BeginGetRepairPlan, channel.EndGetRepairPlan, parameters);
        }

        public Task SetPlanningStageAsync(SetPlanningStageParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetPlanningStage, channel.EndSetPlanningStage, parameters);
        }

        public Task<List<ComplexDTO>> GetComplexListAsync(GetRepairPlanParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ComplexDTO>,GetRepairPlanParameterSet>(channel, channel.BeginGetComplexList, channel.EndGetComplexList, parameters);
        }

        public Task<int> AddComplexAsync(AddComplexParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddComplexParameterSet>(channel, channel.BeginAddComplex, channel.EndAddComplex, parameters);
        }

        public Task EditComplexAsync(EditComplexParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditComplex, channel.EndEditComplex, parameters);
        }

        public Task DeleteComplexAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteComplex, channel.EndDeleteComplex, parameters);
        }

        public Task AddRepairToComplexAsync(AddRepairToComplexParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddRepairToComplex, channel.EndAddRepairToComplex, parameters);
        }

        public Task MoveComplexAsync(EditComplexParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginMoveComplex, channel.EndMoveComplex, parameters);
        }

        public Task<int> AddRepairAsync(AddRepairParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddRepairParameterSet>(channel, channel.BeginAddRepair, channel.EndAddRepair, parameters);
        }

        public Task EditRepairAsync(EditRepairParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditRepair, channel.EndEditRepair, parameters);
        }

        public Task EditRepairDatesAsync(EditRepairDatesParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditRepairDates, channel.EndEditRepairDates, parameters);
        }

        public Task DeleteRepairAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteRepair, channel.EndDeleteRepair, parameters);
        }

        public Task<List<double>> GetKilometerListAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<double>,Guid>(channel, channel.BeginGetKilometerList, channel.EndGetKilometerList, parameters);
        }

        public Task<List<RepairUpdateDTO>> GetRepairUpdateHistoryAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<RepairUpdateDTO>,int>(channel, channel.BeginGetRepairUpdateHistory, channel.EndGetRepairUpdateHistory, parameters);
        }

    }
}
