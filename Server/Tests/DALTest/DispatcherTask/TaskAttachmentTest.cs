using System;
using System.Linq;
using GazRouter.DAL.DataStorage;
using GazRouter.DAL.DispatcherTasks;
using GazRouter.DAL.DispatcherTasks.Attachments;
using GazRouter.DAL.DispatcherTasks.Tasks;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.Attachments;
using GazRouter.DTO.DispatcherTasks.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.DispatcherTask
{
    [TestClass]
    public class TaskAttachmentTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void AddAttachment()
        {
            var taskid = new AddTaskCpddCommand(Context).Execute(new AddTaskCpddParameterSet
            {
                Description = "TestDescription1",
                Subject = "TestSubject1"
            });
            var taskList = new GetTaskListQuery(Context).Execute(
                new GetTaskListParameterSet
                {
                    IsEnterprise = true
                });
            Assert.IsTrue(taskList.FirstOrDefault(p => p.Id == taskid) != null);
            var data = new byte[1000];
            new Random(Guid.NewGuid().GetHashCode()).NextBytes(data);

            var attacmentId = new AddTaskAttachmentCommand(Context).Execute(
                new AddTaskAttachmentParameterSet
                {
                    TaskId = taskid,
                    Data = data,
                    Description = "TestDescription",
                    FileName = "TestFileName.txt",
                });
            var attachment = new GetTaskAttachmentListQuery(Context).Execute(taskid).FirstOrDefault(p => p.Id == attacmentId);
            Assert.IsNotNull(attachment);
            var blob = new GetBlobByIdQuery(Context).Execute(attachment.BlobId);
            CollectionAssert.AreEqual(data, blob.Data);
        }
    }
}
