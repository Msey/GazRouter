using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Bindings.ExchangeEntities;
using GazRouter.DTO.DataExchange.DataSource;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Exchange.ExchangeSettings;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.DataExchange  
{
    [ServiceContract]
    public interface IDataExchangeService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDataSourceList(GetDataSourceListParameterSet parameter, AsyncCallback callback, object state);
        List<DataSourceDTO> EndGetDataSourceList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddDataSource(AddDataSourceParameterSet parameter, AsyncCallback callback, object state);
        int EndAddDataSource(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditDataSource(EditDataSourceParameterSet parameter, AsyncCallback callback, object state);
        void EndEditDataSource(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteDataSource(int parameter, AsyncCallback callback, object state);
        void EndDeleteDataSource(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetExchangeTaskList(GetExchangeTaskListParameterSet parameter, AsyncCallback callback, object state);
        List<ExchangeTaskDTO> EndGetExchangeTaskList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddExchangeTask(AddExchangeTaskParameterSet parameter, AsyncCallback callback, object state);
        int EndAddExchangeTask(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditExchangeTask(EditExchangeTaskParameterSet parameter, AsyncCallback callback, object state);
        void EndEditExchangeTask(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteExchangeTask(int parameter, AsyncCallback callback, object state);
        void EndDeleteExchangeTask(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRunExchangeTask(RunExchangeTaskParameterSet parameters, AsyncCallback callback, object state);
        void EndRunExchangeTask(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetExchangeEntityList(GetExchangeEntityListParameterSet parameter, AsyncCallback callback, object state);
        List<ExchangeEntityDTO> EndGetExchangeEntityList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddExchangeEntity(AddEditExchangeEntityParameterSet parameter, AsyncCallback callback, object state);
        void EndAddExchangeEntity(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditExchangeEntity(AddEditExchangeEntityParameterSet parameter, AsyncCallback callback, object state);
        void EndEditExchangeEntity(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteExchangeEntity(AddEditExchangeEntityParameterSet parameter, AsyncCallback callback, object state);
        void EndDeleteExchangeEntity(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetExchangeEntity(AddEditExchangeEntityParameterSet parameter, AsyncCallback callback, object state);
        void EndSetExchangeEntity(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetExchangePropertyList(GetExchangeEntityListParameterSet parameter, AsyncCallback callback, object state);
        List<ExchangePropertyDTO> EndGetExchangePropertyList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetExchangeProperty(SetExchangePropertyParameterSet parameter, AsyncCallback callback, object state);
        void EndSetExchangeProperty(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetExchangeLog(GetExchangeLogParameterSet parameter, AsyncCallback callback, object state);
        List<ExchangeLogDTO> EndGetExchangeLog(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEnterpriseExchangeNeighbourList(object parameters, AsyncCallback callback, object state);
        List<EnterpriseDTO> EndGetEnterpriseExchangeNeighbourList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetTypicalExchangeEnterpriseList(object parameters, AsyncCallback callback, object state);
        List<NeighbourEnterpriseExchangeTask> EndGetTypicalExchangeEnterpriseList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditTimer(TimerSettingsDTO parameters, AsyncCallback callback, object state);
        void EndEditTimer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginChangeTimerStatus(TimerSettingsDTO parameters, AsyncCallback callback, object state);
        void EndChangeTimerStatus(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetTimers(object parameters, AsyncCallback callback, object state);
        List<TimerSettingsDTO> EndGetTimers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginImportTypicalExchange(string parameters, AsyncCallback callback, object state);
        void EndImportTypicalExchange(IAsyncResult result);
    }


    public class DataExchangeServiceProxy : DataProviderBase<IDataExchangeService>
	{
        protected override string ServiceUri
        {
            get { return "/DataExchange/DataExchangeService.svc"; }
        }

        public Task<List<DataSourceDTO>> GetDataSourceListAsync(GetDataSourceListParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DataSourceDTO>,GetDataSourceListParameterSet>(channel, channel.BeginGetDataSourceList, channel.EndGetDataSourceList, parameter);
        }

        public Task<int> AddDataSourceAsync(AddDataSourceParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddDataSourceParameterSet>(channel, channel.BeginAddDataSource, channel.EndAddDataSource, parameter);
        }

        public Task EditDataSourceAsync(EditDataSourceParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditDataSource, channel.EndEditDataSource, parameter);
        }

        public Task DeleteDataSourceAsync(int parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteDataSource, channel.EndDeleteDataSource, parameter);
        }

        public Task<List<ExchangeTaskDTO>> GetExchangeTaskListAsync(GetExchangeTaskListParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ExchangeTaskDTO>,GetExchangeTaskListParameterSet>(channel, channel.BeginGetExchangeTaskList, channel.EndGetExchangeTaskList, parameter);
        }

        public Task<int> AddExchangeTaskAsync(AddExchangeTaskParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddExchangeTaskParameterSet>(channel, channel.BeginAddExchangeTask, channel.EndAddExchangeTask, parameter);
        }

        public Task EditExchangeTaskAsync(EditExchangeTaskParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditExchangeTask, channel.EndEditExchangeTask, parameter);
        }

        public Task DeleteExchangeTaskAsync(int parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteExchangeTask, channel.EndDeleteExchangeTask, parameter);
        }

        public Task RunExchangeTaskAsync(RunExchangeTaskParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRunExchangeTask, channel.EndRunExchangeTask, parameters);
        }

        public Task<List<ExchangeEntityDTO>> GetExchangeEntityListAsync(GetExchangeEntityListParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ExchangeEntityDTO>,GetExchangeEntityListParameterSet>(channel, channel.BeginGetExchangeEntityList, channel.EndGetExchangeEntityList, parameter);
        }

        public Task AddExchangeEntityAsync(AddEditExchangeEntityParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddExchangeEntity, channel.EndAddExchangeEntity, parameter);
        }

        public Task EditExchangeEntityAsync(AddEditExchangeEntityParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditExchangeEntity, channel.EndEditExchangeEntity, parameter);
        }

        public Task DeleteExchangeEntityAsync(AddEditExchangeEntityParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteExchangeEntity, channel.EndDeleteExchangeEntity, parameter);
        }

        public Task SetExchangeEntityAsync(AddEditExchangeEntityParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetExchangeEntity, channel.EndSetExchangeEntity, parameter);
        }

        public Task<List<ExchangePropertyDTO>> GetExchangePropertyListAsync(GetExchangeEntityListParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ExchangePropertyDTO>,GetExchangeEntityListParameterSet>(channel, channel.BeginGetExchangePropertyList, channel.EndGetExchangePropertyList, parameter);
        }

        public Task SetExchangePropertyAsync(SetExchangePropertyParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetExchangeProperty, channel.EndSetExchangeProperty, parameter);
        }

        public Task<List<ExchangeLogDTO>> GetExchangeLogAsync(GetExchangeLogParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ExchangeLogDTO>,GetExchangeLogParameterSet>(channel, channel.BeginGetExchangeLog, channel.EndGetExchangeLog, parameter);
        }

        public Task<List<EnterpriseDTO>> GetEnterpriseExchangeNeighbourListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<EnterpriseDTO>>(channel, channel.BeginGetEnterpriseExchangeNeighbourList, channel.EndGetEnterpriseExchangeNeighbourList);
        }

        public Task<List<NeighbourEnterpriseExchangeTask>> GetTypicalExchangeEnterpriseListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<NeighbourEnterpriseExchangeTask>>(channel, channel.BeginGetTypicalExchangeEnterpriseList, channel.EndGetTypicalExchangeEnterpriseList);
        }

        public Task EditTimerAsync(TimerSettingsDTO parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditTimer, channel.EndEditTimer, parameters);
        }

        public Task ChangeTimerStatusAsync(TimerSettingsDTO parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginChangeTimerStatus, channel.EndChangeTimerStatus, parameters);
        }

        public Task<List<TimerSettingsDTO>> GetTimersAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<TimerSettingsDTO>>(channel, channel.BeginGetTimers, channel.EndGetTimers);
        }

        public Task ImportTypicalExchangeAsync(string parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginImportTypicalExchange, channel.EndImportTypicalExchange, parameters);
        }

    }
}
