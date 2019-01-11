using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Alarms;

namespace GazRouter.DataServices.Alarms
{
    [Service("Тревоги")]
    [ServiceContract]
    public interface IAlarmsService
    {
        [ServiceAction("Получение списка уставок")]
        [OperationContract]
        List<AlarmDTO> GetAlarmList(GetAlarmListParameterSet parameter);

        [ServiceAction("Добавление уставки")]
        [OperationContract]
        int AddAlarm(AddAlarmParameterSet parameter);

        [ServiceAction("Редактирование уставки")]
        [OperationContract]
        void EditAlarm(EditAlarmParameterSet parameter);

        [ServiceAction("Удаление уставки")]
        [OperationContract]
        void DeleteAlarm(int parameter);

        [ServiceAction("Получение списка тревог по уставке")]
        [OperationContract]
        List<AlarmEventDTO> GetAlarmEventList(int parameter);

        
    }
}
