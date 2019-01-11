using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GazRouter.DTO.DataExchange.DataSource;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Exchange.ExchangeSettings;
using GazRouter.Utils;

namespace GazRouter.DataProviders.DataExchange
{
    public class DataExchangeProvider : DataProviderBase<IDataExchangeService>
    {
        protected override string ServiceUri
        {
            get { return "/DataExchange/DataExchangeService.svc"; }

        }
        

        public void GetExchangeEntityList(GetExchangeEntityListParameterSet parameters, Func<List<ExchangeEntityDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetExchangeEntityList, channel.EndGetExchangeEntityList, callback, parameters, behavior);
        }

        public void AddExchangeEntity(AddEditExchangeEntityParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginAddExchangeEntity, channel.EndAddExchangeEntity, callback, parameters, behavior);
        }

        //public void RunExchangeConfig(RunExchangeConfigParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        //{
        //    var channel = GetChannel();
        //    Execute(channel, channel.BeginRunConfig, channel.EndRunConfig, callback, parameters, behavior);
        //}

        //public void GetEnterpriseExchangeNeighbourList(Func<List<EnterpriseDTO>, Exception, bool> callback, IClientBehavior behavior)
        //{
        //    var channel = GetChannel();
        //    Execute(channel, channel.BeginGetEnterpriseExchangeNeighbourList, channel.EndGetEnterpriseExchangeNeighbourList, callback, behavior);
        //}


        public void AddExchangeTask(AddExchangeTaskParameterSet parameter, Func<int, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginAddExchangeTask, channel.EndAddExchangeTask, callback, parameter, behavior);
        }


        public Task<int> AddExchangeTaskAsync(AddExchangeTaskParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<int, AddExchangeTaskParameterSet>(channel, channel.BeginAddExchangeTask, channel.EndAddExchangeTask, parameter);
        }


        public void AddDataSource(AddDataSourceParameterSet parameter, Func<int, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginAddDataSource, channel.EndAddExchangeTask, callback, parameter, behavior);
        }

        public void GetExchangeTaskList(GetExchangeTaskListParameterSet parameter, Func<List<ExchangeTaskDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetExchangeTaskList, channel.EndGetExchangeTaskList, callback, parameter, behavior);
        }

        public void EditExchangeTask(EditExchangeTaskParameterSet parameter, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginEditExchangeTask, channel.EndEditExchangeTask, callback, parameter, behavior);
        }

        public Task EditExchangeTaskAsync(EditExchangeTaskParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditExchangeTask, channel.EndEditExchangeTask, parameter);
        }


        public void DeleteExchangeTask(int parameter, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginDeleteExchangeTask, channel.EndEditExchangeTask, callback, parameter, behavior);
        }




    }
}