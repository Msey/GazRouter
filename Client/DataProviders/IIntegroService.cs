using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Bindings.ExchangeEntities;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.DataExchange.ASUTPImport;
using GazRouter.DTO.DataExchange.DataSource;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Transformation;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Exchange.ExchangeSettings;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.DataExchange.Integro;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.Integro  
{
    [ServiceContract]
    public interface IIntegroService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginExportSummary(ExportSummaryParams parameters, AsyncCallback callback, object state);
        ExportResult EndExportSummary(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSaveSummaryExchTask(SummaryExchTaskParamSet param, AsyncCallback callback, object state);
        Guid EndSaveSummaryExchTask(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSummariesList(object parameters, AsyncCallback callback, object state);
        List<SummaryDTO> EndGetSummariesList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSummariesListByParams(GetSummaryParameterSet parameters, AsyncCallback callback, object state);
        List<SummaryDTO> EndGetSummariesListByParams(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddEditSummary(AddEditSummaryParameterSet parameters, AsyncCallback callback, object state);
        void EndAddEditSummary(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteSummary(Guid parameters, AsyncCallback callback, object state);
        void EndDeleteSummary(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddEditSummaryParam(AddEditSummaryPParameterSet parameters, AsyncCallback callback, object state);
        void EndAddEditSummaryParam(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddSummaryParamList(List<AddEditSummaryPParameterSet> parameters, AsyncCallback callback, object state);
        SummatyLoadResult EndAddSummaryParamList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddEditSummaryParamContent(AddEditSummaryPContentParameterSet parameters, AsyncCallback callback, object state);
        void EndAddEditSummaryParamContent(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSummariesParamList(Guid summaryId, AsyncCallback callback, object state);
        List<SummaryParamDTO> EndGetSummariesParamList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSummariesParamContentList(Guid parameter, AsyncCallback callback, object state);
        List<SummaryParamContentDTO> EndGetSummariesParamContentList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteSummaryParam(Guid parameters, AsyncCallback callback, object state);
        void EndDeleteSummaryParam(IAsyncResult result);
    }

	public interface IIntegroServiceProxy
	{

        Task<ExportResult> ExportSummaryAsync(ExportSummaryParams parameters);

        Task<Guid> SaveSummaryExchTaskAsync(SummaryExchTaskParamSet param);

        Task<List<SummaryDTO>> GetSummariesListAsync();

        Task<List<SummaryDTO>> GetSummariesListByParamsAsync(GetSummaryParameterSet parameters);

        Task AddEditSummaryAsync(AddEditSummaryParameterSet parameters);

        Task DeleteSummaryAsync(Guid parameters);

        Task AddEditSummaryParamAsync(AddEditSummaryPParameterSet parameters);

        Task<SummatyLoadResult> AddSummaryParamListAsync(List<AddEditSummaryPParameterSet> parameters);

        Task AddEditSummaryParamContentAsync(AddEditSummaryPContentParameterSet parameters);

        Task<List<SummaryParamDTO>> GetSummariesParamListAsync(Guid summaryId);

        Task<List<SummaryParamContentDTO>> GetSummariesParamContentListAsync(Guid parameter);

        Task DeleteSummaryParamAsync(Guid parameters);

    }

    public sealed class IntegroServiceProxy : DataProviderBase<IIntegroService>, IIntegroServiceProxy
	{
        protected override string ServiceUri => "/Integro/IntegroService.svc";
      


        public Task<ExportResult> ExportSummaryAsync(ExportSummaryParams parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<ExportResult,ExportSummaryParams>(channel, channel.BeginExportSummary, channel.EndExportSummary, parameters);
        }

        public Task<Guid> SaveSummaryExchTaskAsync(SummaryExchTaskParamSet param)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,SummaryExchTaskParamSet>(channel, channel.BeginSaveSummaryExchTask, channel.EndSaveSummaryExchTask, param);
        }

        public Task<List<SummaryDTO>> GetSummariesListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<SummaryDTO>>(channel, channel.BeginGetSummariesList, channel.EndGetSummariesList);
        }

        public Task<List<SummaryDTO>> GetSummariesListByParamsAsync(GetSummaryParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<SummaryDTO>,GetSummaryParameterSet>(channel, channel.BeginGetSummariesListByParams, channel.EndGetSummariesListByParams, parameters);
        }

        public Task AddEditSummaryAsync(AddEditSummaryParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddEditSummary, channel.EndAddEditSummary, parameters);
        }

        public Task DeleteSummaryAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteSummary, channel.EndDeleteSummary, parameters);
        }

        public Task AddEditSummaryParamAsync(AddEditSummaryPParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddEditSummaryParam, channel.EndAddEditSummaryParam, parameters);
        }

        public Task<SummatyLoadResult> AddSummaryParamListAsync(List<AddEditSummaryPParameterSet> parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<SummatyLoadResult,List<AddEditSummaryPParameterSet>>(channel, channel.BeginAddSummaryParamList, channel.EndAddSummaryParamList, parameters);
        }

        public Task AddEditSummaryParamContentAsync(AddEditSummaryPContentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddEditSummaryParamContent, channel.EndAddEditSummaryParamContent, parameters);
        }

        public Task<List<SummaryParamDTO>> GetSummariesParamListAsync(Guid summaryId)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<SummaryParamDTO>,Guid>(channel, channel.BeginGetSummariesParamList, channel.EndGetSummariesParamList, summaryId);
        }

        public Task<List<SummaryParamContentDTO>> GetSummariesParamContentListAsync(Guid parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<SummaryParamContentDTO>,Guid>(channel, channel.BeginGetSummariesParamContentList, channel.EndGetSummariesParamContentList, parameter);
        }

        public Task DeleteSummaryParamAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteSummaryParam, channel.EndDeleteSummaryParam, parameters);
        }

    }
}
