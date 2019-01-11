using System;
using System.Linq;
using GazRouter.DAL.Dictionaries.EntityTypes;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.DispatcherTasks;
using GazRouter.DAL.DispatcherTasks.RecordNotes;
using GazRouter.DAL.DispatcherTasks.TaskRecords;
using GazRouter.DAL.DispatcherTasks.TaskRecords.AddTaskRecordCPDD;
using GazRouter.DAL.DispatcherTasks.Tasks;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.RecordNotes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.ObjectModel.Pipelines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.DispatcherTask
{
    [TestClass]
    public class TaskRecordNotesTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void TaskRecordNotes()
        {
			var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            var pipelineId =
                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
                    {
                        Name = "Pipeline1",
                        PipelineTypeId = PipelineType.Bridge,
                        Hidden = true,
                        SortOrder = 1,
                        KilometerOfStart = 10,
                        KilometerOfEnd = 11,GasTransportSystemId = gastransport.Id
                    });

            var taskid = new AddTaskCpddCommand(Context).Execute(new AddTaskCpddParameterSet
                {
                    Description = "TestDescription1",
                    Subject = "TestSubject1"
                });
            var taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet {IsEnterprise = true});
            var task = taskList.FirstOrDefault(p => p.Id == taskid);

            Assert.IsTrue(task != null);

            
           const string userNameCpdd = "SystemUser";
            var recordId = new AddTaskRecordCPDDCommand(Context).Execute(new AddTaskRecordCpddParameterSet
                {
                    TaskId = taskid,
                    Description = "TestDescription",
                    CompletionDate = DateTime.Now,
                    OrderNo = 1,
                    PeriodTypeId = PeriodType.Twohours,
                    PropertyTypeId = PropertyType.PressureInlet,
                    TargetValue = "TestValue",
                    EntityId = pipelineId,
                    UserNameCpdd = userNameCpdd
                });

            new EditTaskRecordCPDDCommand(Context).Execute(new EditTaskRecordCpddParameterSet
                                                               {
                                                                   RowId = recordId,
                                                                   EntityId = pipelineId,
                                                                   PeriodTypeId = PeriodType.Twohours,
                                                                   PropertyTypeId = PropertyType.PressureInlet,
                                                                   TargetValue = "EditTestValue",
                                                                   CompletionDate = DateTime.Now,
                                                                   OrderNo = 1,
                                                                   Description = "TestDescription"
                                                               });

            var list = new GetTaskRecordsVersionListQuery(Context).Execute(recordId);
            Assert.IsTrue(list.Any());
            var taskRecordsList =
                new GetTaskRecordsListQuery(Context).Execute(new GetTaskRecordsCpddParameterSet
                    {
                        IsCpdd = true,
                        TaskVersionId = task.LastVersionId
                    });
            var record = taskRecordsList.First(p => p.Id == recordId);
            Assert.IsTrue(record != null);

            var recordNoteId =
                new AddRecordNoteCommand(Context).Execute(
                    new AddRecordNoteParameterSet
                    {
                        EntityId = record.Entity.Id,
                        TaskId = taskid,
                        PropertyTypeId = PropertyType.PressureInlet,
                        UserNameCpdd = userNameCpdd,
                        Note = "TestNote"
                    });

            var notesList =
                new GetRecordNoteListQuery(Context).Execute(
                    new GetRecordNoteListParameterSet
                    {
                        EntityId = record.Entity.Id,
                        TaskId = taskid,
                        PropertyTypeId = PropertyType.PressureInlet
                });
            Assert.IsTrue(notesList.Any());

            new RemoveRecordNoteCommand(Context).Execute(recordNoteId);
        }
    }
}
