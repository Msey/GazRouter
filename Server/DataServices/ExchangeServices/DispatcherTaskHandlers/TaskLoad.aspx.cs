using GazRouter.DAL.Bindings.EntityPropertyBindings;
using GazRouter.DAL.Core;
using GazRouter.DAL.Dictionaries.Sources;
using GazRouter.DAL.DispatcherTasks;
using GazRouter.DAL.DispatcherTasks.TaskRecords;
using GazRouter.DAL.DispatcherTasks.TaskRecords.AddTaskRecordCPDD;
using GazRouter.DAL.DispatcherTasks.Tasks;
using GazRouter.DAL.DispatcherTasks.TaskStatuses;
using GazRouter.DAL.EventLog;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DataServices.ExchangeServices.AsduDispTask;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatusTypes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;
using GazRouter.DTO.EventLog;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib;
using GazRouter.Service.Exchange.Lib.Export;
using GazRouter.Service.Exchange.Lib.Run;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GazRouter.DataServices.ExchangeServices.DispatcherTaskHandlers
{
    public partial class TaskLoad : Page
    {
        public static string evExchDatetimePattern = "{DateTime.Now}";
        public static string evExchChangingUserNamePattern = "{template.ChangingUserName}";
        public static string evExchIdPattern = "{template.Id}";
        public static string evExchEventDatetimePattern = "{template.EventDateTime}";
        public static string evExchDescriptionPattern = "{template.EventDescription}";
        public static string evExchangeDateTimeFormat = "yyyy-MM-ddTHH:mm:ss\"\"zzz";
        public static string evExchangeEnterpriseIdPattern = "{template.EnterpriseAsduId}";

        static EventExchangeParams instance = new EventExchangeParams();

        static ExchangeSoapExporter exporter = new ExchangeSoapExporter(instance);

        static MyLogger logger = new MyLogger("dispatcherTaskLogger");

        protected void Page_Load(object sender, EventArgs e)
        {            
            if (Request.HttpMethod != "POST")
            {
                string error = "Wrong request, use POST!";
                Response.Write(error);
                logger.Info(error);
                return;
            }
            MessageProcessing mp = null;
            DispatcherMessage ms = null;

            logger.Info("correct request");

            var validator = new XmlValidationHelper();
            validator.AddSchema(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "DispatcherMessage.xsd");

            logger.Info("validating stream");
            if (validator.ValidateStream(Request.InputStream))
            {
                logger.Info("success");

                var u = User;
                byte[] buffer = new byte[Request.InputStream.Length];

                logger.Info(Encoding.UTF8.GetString(buffer, 0, buffer.Length));

                mp = new MessageProcessing(u.Identity.Name, "");
                ms = mp.ParseInput(buffer);

            }
            else
            {
                logger.Error("Валидация по xsd шаблону не пройдена.");
            }

            if (exporter.SendExchangeEvent("", logger)) 
            {
                logger.Info("Сообщение о доставке ДЗ прошло успешно");
                ms.HeaderSection.Status = status.Consultative_Received;
                mp.ParseMessage(ms);
                logger.Info("Статус ДЗ изменен на Consultative_Received");
            }
            else
            {
                if ((DateTime.Now - ms.HeaderSection.Generated.at).Hours > 6)
                {
                    logger.Info("Время ожидания ДЗ все еще в силе");
                    ms.HeaderSection.Status = status.Timeout;
                    mp.ParseMessage(ms);
                }
                else { logger.Info("Время ожидания ДЗ истекло"); }; // иначе снова проверяем подтверждено ли получение ДЗ
            }
            
            //TestTaskRecords();
        }



        /*
        public void test(MyLogger logger)
        {
            using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, logger))
            {
                var recordId = new AddTaskRecordCPDDCommand(context).Execute(new AddTaskRecordCpddParameterSet
                {
                    TaskId = new Guid("11111111-0000-0000-0000-111111111112"),
                    EntityId = new Guid("11111112-0000-0000-0000-111111111112"),
                    PropertyTypeId = new DTO.Dictionaries.PropertyTypes.PropertyType(),
                    PeriodTypeId = Converters.PeriodTypes(scaleType.P1Q),
                    TargetValue = "",
                    Description = "",
                    CompletionDate = DateTime.Now,
                    UserNameCpdd = "",

                    //SiteId = siteId.HasValue ? siteId.Value : Guid.Empty
                });
            }
        }
        */

        /// <summary>  
        ///  <permission cref = "member" > hhj </ permission >  
        /// </summary> 
        public void TestTaskRecords()
        {
            using (var Context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, new MyLogger("")))
            {
                //var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
                var pipelineId =
                    new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
                    {
                        Name = "Pipeline1",
                        // GasTransportSystemId = gastransport.Id,
                        PipelineTypeId = PipelineType.Bridge,
                        Hidden = true,
                        SortOrder = 1,
                        GasTransportSystemId = 1,
                        KilometerOfStart = 10,
                        KilometerOfEnd = 11
                    });
                var taskid = new AddTaskCommand(Context).Execute(new AddTaskParameterSet
                {
                    Description = "TestDescription1",
                    Subject = "TestSubject1"
                });

                var recordId = new AddTaskRecordPDSCommand(Context).Execute(new AddTaskRecordPdsParameterSet
                {
                    TaskId = taskid,
                    Description = "TestDescription",
                    CompletionDate = DateTime.Now,
                    OrderNo = 1,
                    PeriodTypeId = PeriodType.Twohours,
                    PropertyTypeId = PropertyType.PressureInlet,
                    TargetValue = "TestValue",
                    SiteId = pipelineId,
                    EntityId = pipelineId
                });

                int u = 7;
                /*
                var taskid = new AddTaskCommand(Context).Execute(new AddTaskParameterSet
                {
                    Description = "TestDescription1",
                    Subject = "TestSubject1"
                });
                var taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
                var task = taskList.FirstOrDefault(p => p.Id == taskid);
                var taskVersionId = task.LastVersionId;

           

             //   Guid newGuidSite = CreateSite();
                var recordId = new AddTaskRecordPDSCommand(Context).Execute(new AddTaskRecordPdsParameterSet
                {
                    TaskId = taskid,
                    Description = "TestDescription",
                    CompletionDate = DateTime.Now,
                    OrderNo = 1,
                    PeriodTypeId = PeriodType.Twohours,
                    PropertyTypeId = PropertyType.PressureInlet,
                    TargetValue = "TestValue",
                    SiteId = newGuidSite,
                    EntityId = pipelineId
                });

                new EditTaskRecordPDSCommand(Context).Execute(new EditTaskRecordPdsParameterSet
                {
                    RowId = recordId,
                    EntityId = pipelineId,
                    PeriodTypeId = PeriodType.Twohours,
                    PropertyTypeId = PropertyType.PressureInlet,
                    TargetValue = "NextValue",
                    CompletionDate = DateTime.Now,
                    OrderNo = 1,
                    Description = "TestDescription",
                    SiteId = newGuidSite
                });

                var list = new GetTaskRecordsLpuListQuery(Context).Execute(new GetTaskRecordsPdsParameterSet { SiteId = newGuidSite });

                new SetToControlTaskRecordCommand(Context).Execute(recordId);
                new ResetToControlTaskRecordCommand(Context).Execute(recordId);

                var taskRecordsList = new GetTaskRecordsListQuery(Context).Execute(new GetTaskRecordsCpddParameterSet { IsCpdd = false, TaskVersionId = taskVersionId });


                new SetToControlTaskRecordCommand(Context).Execute(recordId);


                new TaskApprovedPdsCommand(Context).Execute(taskid);

                new TaskRecordsExecutedCommand(Context).Execute(recordId);
                taskRecordsList = new GetTaskRecordsListQuery(Context).Execute(new GetTaskRecordsCpddParameterSet { IsCpdd = false, TaskVersionId = taskVersionId });
                var t1 = taskRecordsList.FirstOrDefault(p => p.Id == recordId);
                new TaskRecordsResetExecutedCommand(Context).Execute(recordId);
                taskRecordsList = new GetTaskRecordsListQuery(Context).Execute(new GetTaskRecordsCpddParameterSet { IsCpdd = false, TaskVersionId = taskVersionId });
                t1 = taskRecordsList.FirstOrDefault(p => p.Id == recordId);

                //var taskRecordsList1 = new GetTaskRecordsLpuListQuery(context).Execute(new GetTaskRecordsPDSParameterSet { SiteId = newGuidSite,IsList = true});
                //Assert.IsTrue(taskRecordsList1.FirstOrDefault(p => p.Id == recordId) != null);
                //new TaskRecordsExecutedCommand(context).Execute(recordId);
                //taskRecordsList1 = new GetTaskRecordsLpuListQuery(context).Execute(new GetTaskRecordsPDSParameterSet { SiteId = newGuidSite, IsList = true });
                //Assert.IsTrue(taskRecordsList1.FirstOrDefault(p => p.Id == recordId) == null);
                //taskRecordsList1 = new GetTaskRecordsLpuListQuery(context).Execute(new GetTaskRecordsPDSParameterSet { SiteId = newGuidSite, IsList = false });
                //Assert.IsTrue(taskRecordsList1.FirstOrDefault(p => p.Id == recordId) != null);

                new RemoveTaskRecordCommand(Context).Execute(recordId);
                */
            }
        }
    }
}





