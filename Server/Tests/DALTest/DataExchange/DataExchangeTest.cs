using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.DataExchange.DataSource;
using GazRouter.DAL.DataExchange.ExchangeEntity;
using GazRouter.DAL.DataExchange.ExchangeLog;
using GazRouter.DAL.DataExchange.ExchangeProperty;
using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.DataSource;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.TransportTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.DataExchange
{
    [TestClass]
    public class DataExchangeTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void Test()
        {
            // Добаваление источника данных
            var sourceId = new AddDataSourceCommand(Context).Execute(
                new AddDataSourceParameterSet
                {
                    Name = "Test028",
                    Description = "TestTest",
                    SysName = "TestTestTest"
                });

            Assert.IsNotNull(sourceId);

            // Получение списка источников данных
            var sourceList = new GetDataSourceListQuery(Context).Execute(null);
            Assert.IsTrue(sourceList.Any(s => s.Id == sourceId));
            
            
            // Редактирование источника данных
            new EditDataSourceCommand(Context).Execute(
                new EditDataSourceParameterSet
                {
                    Id = sourceId,
                    Name = "Test029",
                    Description = "TestTest023",
                    SysName = "TestTestTestXXX"
                });

            
            // Добавить задачу обмена
            var taskId = new AddExchangeTaskCommand(Context).Execute(
                new AddExchangeTaskParameterSet
                {
                    Name = "Test000",
                    ExchangeTypeId = ExchangeType.Import,
                    DataSourceId = sourceId,
                    FileNameMask = "xx.xx",
                    IsCritical = true,
                    IsTransform = true,
                    PeriodTypeId = PeriodType.Twohours,
                    Transformation = "",
                    TransportTypeId = TransportType.Ftp,
                    ExchangeStatus = ExchangeStatus.Off,
                    TransportAddress = "10.240.5.222",
                    TransportLogin = "login",
                    TransportPassword = "swordfish"
                });

            Assert.IsNotNull(taskId);

            // Получить список задач
            var taskList = new GetExchangeTaskListQuery(Context).Execute(
                new GetExchangeTaskListParameterSet
                {
                    Id = taskId
                });
            Assert.IsTrue(taskList.Any(t => t.Id == taskId));

            // Изменить задачу
            new EditExchangeTaskCommand(Context).Execute(
                new EditExchangeTaskParameterSet
                {
                    Id = taskId,
                    Name = "Test011",
                    ExchangeTypeId = ExchangeType.Export,
                    ExchangeStatus = ExchangeStatus.Scheduled,
                    DataSourceId = sourceId,
                    FileNameMask = "yy.yy",
                    IsCritical = false,
                    IsTransform = false,
                    PeriodTypeId = PeriodType.Day,
                    Transformation = "111",
                    TransportTypeId = TransportType.Email,
                    TransportAddress = "xx@yy.zz",
                    TransportLogin = "sa",
                    TransportPassword = "sha"
                });


            var entityId = GetCompShop(Context);

            // Добавить объект обмена
            new AddExchangeEntityCommand(Context).Execute(
                new AddEditExchangeEntityParameterSet
                {
                    ExchangeTaskId = taskId,
                    EntityId = entityId,
                    ExtId = "xx",
                    IsActive = true
                });

            // Получить список объектов обмена
            var ents = new GetExchangeEntityListQuery(Context).Execute(
                new GetExchangeEntityListParameterSet
                {
                    ExchangeTaskIdList = new List<int> {taskId}
                });

            var ents2 = new GetExchangeEntityListQuery(Context).Execute(
                new GetExchangeEntityListParameterSet
                {
                    ExchangeTaskIdList = new List<int> {taskId},
                    IsActive = true
                });

            // Изменить объект обмена
            new EditExchangeEntityCommand(Context).Execute(
                new AddEditExchangeEntityParameterSet
                {
                    ExchangeTaskId = taskId,
                    EntityId = entityId,
                    ExtId = "yy",
                    IsActive = false
                });


            // Задать идентификатор для свойства
            new SetExchangePropertyCommand(Context).Execute(
                new SetExchangePropertyParameterSet
                {
                    ExchangeTaskId = taskId,
                    EntityId = entityId,
                    PropertyTypeId = PropertyType.PressureInlet,
                    ExtId = "gg"
                });

            // Получить список свойств объекта учавствующих в обмене
            new GetExchangePropertyListQuery(Context).Execute(
                new GetExchangeEntityListParameterSet
                {
                    ExchangeTaskIdList = new List<int> {taskId}
                });



            // Получить лог
            new GetExchangeLogQuery(Context).Execute(
                new GetExchangeLogParameterSet
                {
                    ExchangeTaskId = taskId,
                    StartDate = DateTime.Now.AddDays(-3),
                    EndDate = DateTime.Now
                });



            // Удалить объект учавствующий в обмене
            new DeleteExchangeEntityCommand(Context).Execute(
                new AddEditExchangeEntityParameterSet
                {
                    EntityId = entityId,
                    ExchangeTaskId = taskId
                });


            // Удалить задачу обмена
            new DeleteExchangeTaskCommand(Context).Execute(taskId);


            // Удалить источник данных
            new DeleteDataSourceCommand(Context).Execute(sourceId);
            

            
        }

        
    }
}
