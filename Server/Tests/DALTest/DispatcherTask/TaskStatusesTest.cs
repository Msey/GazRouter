using GazRouter.DAL.DispatcherTasks;
using GazRouter.DAL.DispatcherTasks.Tasks;
using GazRouter.DAL.DispatcherTasks.TaskStatuses;
using GazRouter.DTO.Dictionaries.AnnuledReasons;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.DispatcherTask
{
	[TestClass]
    public class TaskStatusesTest : TransactionTestsBase
	{
        [TestMethod ,TestCategory(Stable)]
        public void TaskStatuses()
        {
            var taskId1 = new AddTaskCommand(Context).Execute(new AddTaskParameterSet
                {
                    Description = "TestDescription1",
                    Subject = "TestSubject1"
                });

            new TaskApprovedPdsCommand(Context).Execute(taskId1);

            new TaskPerformedCommand(Context).Execute(taskId1);

            var taskId2 = new AddTaskCommand(Context).Execute(new AddTaskParameterSet
                {
                    Description = "TestDescription2",
                    Subject = "TestSubject2"
                });

            new TaskApprovedPdsCommand(Context).Execute(taskId2);

            new TaskAnnuledCommand(Context).Execute(
                new SetTaskStatusParameterSet
                {
                    TaskId = taskId2,
                    AnnuledReason = AnnuledReason.Other,
                    ReasonDescription = "TestReason"
                });
        }
	}
}
