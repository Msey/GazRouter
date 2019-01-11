using System;
using System.Linq;
using GazRouter.DAL.Alarms;
using GazRouter.DAL.Authorization.User;
using GazRouter.DTO.Alarms;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Dictionaries.AlarmTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.Alarms
{
    [TestClass]
    public class AlarmsTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void TestAddAlarm()
        {
            var enterpriseId = GetEnterpriseId();

            var user = GetCurrentUser();
            new EditUserCommand(Context).Execute(new EditUserParameterSet
                {
                    Id = user.Id,
                    Description = user.Description,
                    UserName = user.UserName,
                    SiteId = enterpriseId
                });


            var entity = GetCompShop(Context);

            // добавление
            var alarmId = 
            new AddAlarmCommand(Context).Execute(
                new AddAlarmParameterSet
                {
                    ActivationDate = DateTime.Now,
                    AlarmTypeId = AlarmType.LowerLimit,
                    Description = "BlaBla",
                    EntityId = entity,
                    ExpirationDate = DateTime.Now.AddDays(2),
                    PeriodTypeId = PeriodType.Twohours,
                    PropertyTypeId = PropertyType.PressureInlet,
                    Setting = 10
                });

            
            Assert.IsTrue(alarmId != default(int));

            var alarmList = 
            new GetAlarmListQuery(Context).Execute(
                new GetAlarmListParameterSet
                {
                    SiteId = enterpriseId
                });

            Assert.IsTrue(alarmList != null && alarmList.Count > 0);
            Assert.IsTrue(alarmList.Any(a => a.Id == alarmId));



            // редактирование
            new EditAlarmCommand(Context).Execute(
                new EditAlarmParameterSet
                {
                    AlarmId = alarmId,
                    ActivationDate = DateTime.Now,
                    AlarmTypeId = AlarmType.LowerLimit,
                    Description = "NuNu",
                    EntityId = entity,
                    ExpirationDate = DateTime.Now.AddDays(2),
                    PeriodTypeId = PeriodType.Twohours,
                    PropertyTypeId = PropertyType.PressureInlet,
                    Setting = 30
                });


            alarmList =
            new GetAlarmListQuery(Context).Execute(
                new GetAlarmListParameterSet
                {
                    SiteId = enterpriseId
                });
            Assert.IsTrue(alarmList != null && alarmList.Count > 0);
            Assert.IsTrue(alarmList.Any(a => a.Id == alarmId));
            var alrm = alarmList.Single(a => a.Id == alarmId);
            Assert.IsTrue(alrm.Description == "NuNu" && alrm.Setting == 30);


            // получение списка событий по уставке
            var eventList = new GetAlarmEventListQuery(Context).Execute(alarmId);
            Assert.IsNotNull(eventList);

            new DeleteAlarmCommand(Context).Execute(alarmId);
        }

        
    }
}
