using System.Collections.Generic;
using GazRouter.DAL.Alarms;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DTO.Alarms;

namespace GazRouter.DataServices.Alarms
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class AlarmsService : ServiceBase, IAlarmsService
    {
        public List<AlarmDTO> GetAlarmList(GetAlarmListParameterSet parameter)
        {
            return ExecuteRead<GetAlarmListQuery, List<AlarmDTO>, GetAlarmListParameterSet>(parameter);
        }

        public List<AlarmEventDTO> GetAlarmEventList(int parameter)
        {
            return ExecuteRead<GetAlarmEventListQuery, List<AlarmEventDTO>, int>(parameter);
        }


        public int AddAlarm(AddAlarmParameterSet parameter)
        {
            return ExecuteRead<AddAlarmCommand, int, AddAlarmParameterSet>(parameter);
        }

        public void EditAlarm(EditAlarmParameterSet parameter)
        {
            ExecuteNonQuery<EditAlarmCommand, EditAlarmParameterSet>(parameter);
        }

        public void DeleteAlarm(int parameter)
        {
            ExecuteNonQuery<DeleteAlarmCommand, int>(parameter);
        }
        
    }
}
