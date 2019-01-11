using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Repairs;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.DTO.Repairs.RepairReport;
using GazRouter.DTO.Repairs.RepairWorks;
using GazRouter.DTO.Repairs.Agreed;
using GazRouter.DTO.Repairs.Workflow;
using GazRouter.DTO.Dictionaries.PlanTypes;
using GazRouter.DTO.Repairs.PrintForms;
using GazRouter.DTO.Dictionaries.RepairTypes;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.Repairs  
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
        [ServiceKnownType(typeof(RepairPlanCompShopDTO))] 
        IAsyncResult BeginGetWorkflowList(GetRepairWorkflowsParameterSet parameters, AsyncCallback callback, object state);
        RepairPlanDataDTO EndGetWorkflowList(IAsyncResult result);

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
        IAsyncResult BeginGetRepairUpdateHistory(int parameters, AsyncCallback callback, object state);
        List<RepairUpdateDTO> EndGetRepairUpdateHistory(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddRepairReport(RepairReportParametersSet parameters, AsyncCallback callback, object state);
        int EndAddRepairReport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditRepairReport(RepairReportParametersSet parameters, AsyncCallback callback, object state);
        void EndEditRepairReport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteRepairReport(int parameters, AsyncCallback callback, object state);
        void EndDeleteRepairReport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetRepairReportsByRepair(int parameters, AsyncCallback callback, object state);
        List<RepairReportDTO> EndGetRepairReportsByRepair(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddRepairReportAttachment(RepairReportAttachmentParamentersSet parameters, AsyncCallback callback, object state);
        int EndAddRepairReportAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteRepairReportAttachment(int parameters, AsyncCallback callback, object state);
        void EndDeleteRepairReportAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetRepairReportAttachments(int parameters, AsyncCallback callback, object state);
        List<RepairReportAttachmentDTO> EndGetRepairReportAttachments(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddRepairWorkAttachment(AddRepairWorkAttachmentParameterSet parameters, AsyncCallback callback, object state);
        int EndAddRepairWorkAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditRepairWorkAttachment(EditRepairWorkAttachmentParameterSet parameters, AsyncCallback callback, object state);
        void EndEditRepairWorkAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveRepairWorkAttachment(int parameters, AsyncCallback callback, object state);
        void EndRemoveRepairWorkAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveAllRepairWorkAttachments(int parameters, AsyncCallback callback, object state);
        void EndRemoveAllRepairWorkAttachments(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetRepairAttachementsList(int parameters, AsyncCallback callback, object state);
        List<RepairWorkAttachmentDTO> EndGetRepairAttachementsList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAgreedRepairRecordList(int parameters, AsyncCallback callback, object state);
        List<AgreedRepairRecordDTO> EndGetAgreedRepairRecordList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddAgreedRepairRecord(AddEditAgreedRepairRecordParameterSet parameters, AsyncCallback callback, object state);
        int EndAddAgreedRepairRecord(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditAgreedRepairRecord(AddEditAgreedRepairRecordParameterSet parameters, AsyncCallback callback, object state);
        void EndEditAgreedRepairRecord(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteAgreedRepairRecord(int parameters, AsyncCallback callback, object state);
        void EndDeleteAgreedRepairRecord(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPrintDocumentPattern(string DocType, AsyncCallback callback, object state);
        byte[] EndGetPrintDocumentPattern(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetRepairAgreementsList(int parameters, AsyncCallback callback, object state);
        List<AgreedRepairRecordDTO> EndGetRepairAgreementsList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginChangeWorkflowState(ChangeRepairWfParametrSet parameters, AsyncCallback callback, object state);
        void EndChangeWorkflowState(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetWorkflowHistory(int parameters, AsyncCallback callback, object state);
        List<WorkflowHistoryDTO> EndGetWorkflowHistory(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSighers(GetSignersSet parameters, AsyncCallback callback, object state);
        SignersDTO EndGetSighers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetRepairPlanTypes(object parameters, AsyncCallback callback, object state);
        List<PlanTypeDTO> EndGetRepairPlanTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetRepairTypes(object parameters, AsyncCallback callback, object state);
        List<RepairTypeDTO> EndGetRepairTypes(IAsyncResult result);
    }

	public interface IRepairsServiceProxy
	{

        Task<RepairPlanDataDTO> GetRepairPlanAsync(GetRepairPlanParameterSet parameters);

        Task<RepairPlanDataDTO> GetWorkflowListAsync(GetRepairWorkflowsParameterSet parameters);

        Task SetPlanningStageAsync(SetPlanningStageParameterSet parameters);

        Task<List<ComplexDTO>> GetComplexListAsync(GetRepairPlanParameterSet parameters);

        Task<int> AddComplexAsync(AddComplexParameterSet parameters);

        Task EditComplexAsync(EditComplexParameterSet parameters);

        Task DeleteComplexAsync(int parameters);

        Task AddRepairToComplexAsync(AddRepairToComplexParameterSet parameters);

        Task MoveComplexAsync(EditComplexParameterSet parameters);

        Task<int> AddRepairAsync(AddRepairParameterSet parameters);

        Task EditRepairAsync(EditRepairParameterSet parameters);

        Task EditRepairDatesAsync(EditRepairDatesParameterSet parameters);

        Task DeleteRepairAsync(int parameters);

        Task<List<RepairUpdateDTO>> GetRepairUpdateHistoryAsync(int parameters);

        Task<int> AddRepairReportAsync(RepairReportParametersSet parameters);

        Task EditRepairReportAsync(RepairReportParametersSet parameters);

        Task DeleteRepairReportAsync(int parameters);

        Task<List<RepairReportDTO>> GetRepairReportsByRepairAsync(int parameters);

        Task<int> AddRepairReportAttachmentAsync(RepairReportAttachmentParamentersSet parameters);

        Task DeleteRepairReportAttachmentAsync(int parameters);

        Task<List<RepairReportAttachmentDTO>> GetRepairReportAttachmentsAsync(int parameters);

        Task<int> AddRepairWorkAttachmentAsync(AddRepairWorkAttachmentParameterSet parameters);

        Task EditRepairWorkAttachmentAsync(EditRepairWorkAttachmentParameterSet parameters);

        Task RemoveRepairWorkAttachmentAsync(int parameters);

        Task RemoveAllRepairWorkAttachmentsAsync(int parameters);

        Task<List<RepairWorkAttachmentDTO>> GetRepairAttachementsListAsync(int parameters);

        Task<List<AgreedRepairRecordDTO>> GetAgreedRepairRecordListAsync(int parameters);

        Task<int> AddAgreedRepairRecordAsync(AddEditAgreedRepairRecordParameterSet parameters);

        Task EditAgreedRepairRecordAsync(AddEditAgreedRepairRecordParameterSet parameters);

        Task DeleteAgreedRepairRecordAsync(int parameters);

        Task<byte[]> GetPrintDocumentPatternAsync(string DocType);

        Task<List<AgreedRepairRecordDTO>> GetRepairAgreementsListAsync(int parameters);

        Task ChangeWorkflowStateAsync(ChangeRepairWfParametrSet parameters);

        Task<List<WorkflowHistoryDTO>> GetWorkflowHistoryAsync(int parameters);

        Task<SignersDTO> GetSighersAsync(GetSignersSet parameters);

        Task<List<PlanTypeDTO>> GetRepairPlanTypesAsync();

        Task<List<RepairTypeDTO>> GetRepairTypesAsync();

    }

    public sealed class RepairsServiceProxy : DataProviderBase<IRepairsService>, IRepairsServiceProxy
	{
        protected override string ServiceUri => "/Repairs/RepairsService.svc";
      


        public Task<RepairPlanDataDTO> GetRepairPlanAsync(GetRepairPlanParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<RepairPlanDataDTO,GetRepairPlanParameterSet>(channel, channel.BeginGetRepairPlan, channel.EndGetRepairPlan, parameters);
        }

        public Task<RepairPlanDataDTO> GetWorkflowListAsync(GetRepairWorkflowsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<RepairPlanDataDTO,GetRepairWorkflowsParameterSet>(channel, channel.BeginGetWorkflowList, channel.EndGetWorkflowList, parameters);
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

        public Task<List<RepairUpdateDTO>> GetRepairUpdateHistoryAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<RepairUpdateDTO>,int>(channel, channel.BeginGetRepairUpdateHistory, channel.EndGetRepairUpdateHistory, parameters);
        }

        public Task<int> AddRepairReportAsync(RepairReportParametersSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,RepairReportParametersSet>(channel, channel.BeginAddRepairReport, channel.EndAddRepairReport, parameters);
        }

        public Task EditRepairReportAsync(RepairReportParametersSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditRepairReport, channel.EndEditRepairReport, parameters);
        }

        public Task DeleteRepairReportAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteRepairReport, channel.EndDeleteRepairReport, parameters);
        }

        public Task<List<RepairReportDTO>> GetRepairReportsByRepairAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<RepairReportDTO>,int>(channel, channel.BeginGetRepairReportsByRepair, channel.EndGetRepairReportsByRepair, parameters);
        }

        public Task<int> AddRepairReportAttachmentAsync(RepairReportAttachmentParamentersSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,RepairReportAttachmentParamentersSet>(channel, channel.BeginAddRepairReportAttachment, channel.EndAddRepairReportAttachment, parameters);
        }

        public Task DeleteRepairReportAttachmentAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteRepairReportAttachment, channel.EndDeleteRepairReportAttachment, parameters);
        }

        public Task<List<RepairReportAttachmentDTO>> GetRepairReportAttachmentsAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<RepairReportAttachmentDTO>,int>(channel, channel.BeginGetRepairReportAttachments, channel.EndGetRepairReportAttachments, parameters);
        }

        public Task<int> AddRepairWorkAttachmentAsync(AddRepairWorkAttachmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddRepairWorkAttachmentParameterSet>(channel, channel.BeginAddRepairWorkAttachment, channel.EndAddRepairWorkAttachment, parameters);
        }

        public Task EditRepairWorkAttachmentAsync(EditRepairWorkAttachmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditRepairWorkAttachment, channel.EndEditRepairWorkAttachment, parameters);
        }

        public Task RemoveRepairWorkAttachmentAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveRepairWorkAttachment, channel.EndRemoveRepairWorkAttachment, parameters);
        }

        public Task RemoveAllRepairWorkAttachmentsAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveAllRepairWorkAttachments, channel.EndRemoveAllRepairWorkAttachments, parameters);
        }

        public Task<List<RepairWorkAttachmentDTO>> GetRepairAttachementsListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<RepairWorkAttachmentDTO>,int>(channel, channel.BeginGetRepairAttachementsList, channel.EndGetRepairAttachementsList, parameters);
        }

        public Task<List<AgreedRepairRecordDTO>> GetAgreedRepairRecordListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<AgreedRepairRecordDTO>,int>(channel, channel.BeginGetAgreedRepairRecordList, channel.EndGetAgreedRepairRecordList, parameters);
        }

        public Task<int> AddAgreedRepairRecordAsync(AddEditAgreedRepairRecordParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddEditAgreedRepairRecordParameterSet>(channel, channel.BeginAddAgreedRepairRecord, channel.EndAddAgreedRepairRecord, parameters);
        }

        public Task EditAgreedRepairRecordAsync(AddEditAgreedRepairRecordParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditAgreedRepairRecord, channel.EndEditAgreedRepairRecord, parameters);
        }

        public Task DeleteAgreedRepairRecordAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteAgreedRepairRecord, channel.EndDeleteAgreedRepairRecord, parameters);
        }

        public Task<byte[]> GetPrintDocumentPatternAsync(string DocType)
        {
            var channel = GetChannel();
            return ExecuteAsync<byte[],string>(channel, channel.BeginGetPrintDocumentPattern, channel.EndGetPrintDocumentPattern, DocType);
        }

        public Task<List<AgreedRepairRecordDTO>> GetRepairAgreementsListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<AgreedRepairRecordDTO>,int>(channel, channel.BeginGetRepairAgreementsList, channel.EndGetRepairAgreementsList, parameters);
        }

        public Task ChangeWorkflowStateAsync(ChangeRepairWfParametrSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginChangeWorkflowState, channel.EndChangeWorkflowState, parameters);
        }

        public Task<List<WorkflowHistoryDTO>> GetWorkflowHistoryAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<WorkflowHistoryDTO>,int>(channel, channel.BeginGetWorkflowHistory, channel.EndGetWorkflowHistory, parameters);
        }

        public Task<SignersDTO> GetSighersAsync(GetSignersSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<SignersDTO,GetSignersSet>(channel, channel.BeginGetSighers, channel.EndGetSighers, parameters);
        }

        public Task<List<PlanTypeDTO>> GetRepairPlanTypesAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<PlanTypeDTO>>(channel, channel.BeginGetRepairPlanTypes, channel.EndGetRepairPlanTypes);
        }

        public Task<List<RepairTypeDTO>> GetRepairTypesAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<RepairTypeDTO>>(channel, channel.BeginGetRepairTypes, channel.EndGetRepairTypes);
        }

    }
}
