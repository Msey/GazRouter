using System.Linq;
using GazRouter.DAL.DispatcherTasks;
using GazRouter.DAL.DispatcherTasks.Tasks;
using GazRouter.DAL.DispatcherTasks.TaskStatuses;
using GazRouter.DTO.Dictionaries.AnnuledReasons;
using GazRouter.DTO.Dictionaries.StatusTypes;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.DispatcherTask
{
	[TestClass]
    public class TestAddTask : TransactionTestsBase
	{
        [TestMethod ,TestCategory(Stable)]
        public void SHOULD_PASS_FROM_SUBMITED_TO_APPROVED_TO_PERFOMED()
        {
            var taskId = new AddTaskCpddCommand(Context).Execute(new AddTaskCpddParameterSet
                {
                    Description = "TestDescription",
                    Subject = "TestSubject",
                    GlobalTaskId = "TestGlobalTaskId3",
                    UserNameCpdd = "TestUserNameCpdd"
                });

            var taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
            var task = taskList.FirstOrDefault(p => p.Id == taskId);
            Assert.IsTrue(task != null);
            Assert.IsTrue(task.StatusType == StatusType.OnSubmit);


            new TaskSubmitedCommand(Context).Execute(new SetTaskStatusParameterSet {TaskId = taskId, UserNameCpdd = "TestUser"});
            taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
            task = taskList.FirstOrDefault(p => p.Id == taskId);
            Assert.IsTrue(task != null);
            Assert.IsTrue(task.StatusType == StatusType.Submitted);

            new TaskApprovedCPDDCommand(Context).Execute(new SetTaskStatusParameterSet
                {
                    TaskId = taskId,
                    UserNameCpdd = "TestUser"
                });
            taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
            task = taskList.FirstOrDefault(p => p.Id == taskId);
            Assert.IsTrue(task != null);
            Assert.IsTrue(task.StatusType == StatusType.ApprovedByCpdd);

            new TaskApprovedPdsCommand(Context).Execute(taskId);
            taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
            task = taskList.FirstOrDefault(p => p.Id == taskId);
            Assert.IsTrue(task != null);
            Assert.IsTrue(task.StatusType == StatusType.ApprovedForSite);

            new TaskPerformedCommand(Context).Execute(taskId);
            taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
            task = taskList.FirstOrDefault(p => p.Id == taskId);
            Assert.IsTrue(task != null);
            Assert.IsTrue(task.StatusType == StatusType.Performed);
        }

	    [TestMethod ,TestCategory(Stable)]
        public void SHOULD_PASS_FROM_ONSUBMITED_TO_CORRECTED_TO_ONSUBMITED()
        {
            var taskId = new AddTaskCpddCommand(Context).Execute(new AddTaskCpddParameterSet
                {
                    Description = "TestDescription1",
                    Subject = "TestSubject1",
                    GlobalTaskId = "TestGlobalTaskId1",
                    UserNameCpdd = "TestUserNameCpdd1"
                });
            var taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
            Assert.IsTrue(taskList.FirstOrDefault(p => p.Id == taskId) != null);


            var taskVersionId = new TaskCorrectingCommand(Context).Execute(new SetTaskStatusParameterSet { TaskId = taskId });
            taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
            var task = taskList.FirstOrDefault(p => p.Id == taskId);
            Assert.IsTrue(task != null);
            Assert.IsTrue(task.StatusType == StatusType.Correcting);

            task = new GetTaskByVersionIdQuery(Context).Execute(taskVersionId);
            Assert.IsTrue(task != null);

            new TaskCorrectedCommand(Context).Execute(new SetTaskStatusParameterSet { TaskId = taskId, UserNameCpdd = "TestUser" });
            taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
            task = taskList.FirstOrDefault(p => p.Id == taskId);
            Assert.IsTrue(task != null);
            Assert.IsTrue(task.StatusType == StatusType.Corrected);

        }


	    [TestMethod ,TestCategory(Stable)]
        public void SHOULD_PASS_FROM_CREATED_TO_APPROVED_FORSITE_TO_PERFORMED()
	    {
	        var taskId = new AddTaskCommand(Context).Execute(new AddTaskParameterSet
	            {
	                Description = "TestDescription1",
	                Subject = "TestSubject1"
	            });
	        var taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
	        var task = taskList.FirstOrDefault(p => p.Id == taskId);
	        Assert.IsNotNull(task);
	        Assert.IsTrue(task.StatusType == StatusType.Created);

	        new TaskApprovedPdsCommand(Context).Execute(taskId);
	        taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
	        task = taskList.FirstOrDefault(p => p.Id == taskId);
	        Assert.IsNotNull(task);
	        Assert.IsTrue(task.StatusType == StatusType.ApprovedForSite);

	        new TaskPerformedCommand(Context).Execute(taskId);
	        taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
	        task = taskList.FirstOrDefault(p => p.Id == taskId);
	        Assert.IsNotNull(task);
	        Assert.IsTrue(task.StatusType == StatusType.Performed);
	    }

	    [TestMethod ,TestCategory(Stable)]
        public void SHOULD_PASS_FROM_CREATED_TO_APPROVED_FORSITE_TO_ANNULED()
        {
            var taskId = new AddTaskCommand(Context).Execute(new AddTaskParameterSet
                {
                    Description = "TestDescription1",
                    Subject = "TestSubject1"
                });
            var taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
            Assert.IsTrue(taskList.FirstOrDefault(p => p.Id == taskId) != null);
            Assert.IsTrue(taskList.FirstOrDefault(p => p.Id == taskId).StatusType == StatusType.Created);

            new TaskApprovedPdsCommand(Context).Execute(taskId);
            taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
            Assert.IsTrue(taskList.FirstOrDefault(p => p.Id == taskId) != null);
            Assert.IsTrue(taskList.FirstOrDefault(p => p.Id == taskId).StatusType == StatusType.ApprovedForSite);

            new TaskAnnuledCommand(Context).Execute(new SetTaskStatusParameterSet
                {
                    ReasonDescription = "No reasone",
                    AnnuledReason = AnnuledReason.CancelPDS,
                    TaskId = taskId
                });
            taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
            Assert.IsTrue(taskList.FirstOrDefault(p => p.Id == taskId) != null);
            Assert.IsTrue(taskList.FirstOrDefault(p => p.Id == taskId).StatusType == StatusType.Annuled);
        }

		[TestMethod ,TestCategory(Stable)]
		public void TestDeleteTask()
		{
			var taskId = new AddTaskCpddCommand(Context).Execute(new AddTaskCpddParameterSet
			{
				Description = "TestDescription",
				Subject = "TestSubject",
				GlobalTaskId = "TestGlobalTaskId3",
				UserNameCpdd = "TestUserNameCpdd"
			});

			var taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
			var task = taskList.FirstOrDefault(p => p.Id == taskId);
			Assert.IsTrue(task != null);
			new DeleteTaskCommand(Context).Execute(taskId);
			taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
			task = taskList.FirstOrDefault(p => p.Id == taskId);
			Assert.IsTrue(task == null);
		}
	}
}
