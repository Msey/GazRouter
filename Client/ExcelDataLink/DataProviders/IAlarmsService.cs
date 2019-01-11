using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Alarms;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.Alarms  
{
    [ServiceContract]
    public interface IAlarmsService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAlarmList(GetAlarmListParameterSet parameter, AsyncCallback callback, object state);
        List<AlarmDTO> EndGetAlarmList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddAlarm(AddAlarmParameterSet parameter, AsyncCallback callback, object state);
        int EndAddAlarm(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditAlarm(EditAlarmParameterSet parameter, AsyncCallback callback, object state);
        void EndEditAlarm(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteAlarm(int parameter, AsyncCallback callback, object state);
        void EndDeleteAlarm(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAlarmEventList(int parameter, AsyncCallback callback, object state);
        List<AlarmEventDTO> EndGetAlarmEventList(IAsyncResult result);
    }


    public class AlarmsServiceProxy : DataProviderBase<IAlarmsService>
	{
        protected override string ServiceUri
        {
            get { return "/Alarms/AlarmsService.svc"; }
        }

        public Task<List<AlarmDTO>> GetAlarmListAsync(GetAlarmListParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<AlarmDTO>,GetAlarmListParameterSet>(channel, channel.BeginGetAlarmList, channel.EndGetAlarmList, parameter);
        }

        public Task<int> AddAlarmAsync(AddAlarmParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddAlarmParameterSet>(channel, channel.BeginAddAlarm, channel.EndAddAlarm, parameter);
        }

        public Task EditAlarmAsync(EditAlarmParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditAlarm, channel.EndEditAlarm, parameter);
        }

        public Task DeleteAlarmAsync(int parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteAlarm, channel.EndDeleteAlarm, parameter);
        }

        public Task<List<AlarmEventDTO>> GetAlarmEventListAsync(int parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<AlarmEventDTO>,int>(channel, channel.BeginGetAlarmEventList, channel.EndGetAlarmEventList, parameter);
        }

    }
}
