using System;
using System.Linq;
using GazRouter.DAL.DataStorage;
using GazRouter.DAL.EventLog.Attachments;
using GazRouter.DTO.EventLog.Attachments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.Event
{
    [TestClass]
    public class EventAttachmentTest : EventsTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void AddRemoveAttachment()
        {
            Guid enterpriseId = GetEnterpriseId();
            var eventId = RegisterEvent(enterpriseId);

            var data = new byte[1000];
            new Random(Guid.NewGuid().GetHashCode()).NextBytes(data);

            var attacmentId = new AddEventAttachmentCommand(Context).Execute(
                new AddEventAttachmentParameterSet
                    {
                        EventId = eventId,
                        Data = data,
                        Description = "TestDescription",
                        FileName = "TestFileName.txt"
                    });
            
            var attachment =
                new GetEventAttachmentListQuery(Context).Execute(eventId).First();
            var blob = new GetBlobByIdQuery(Context).Execute(attachment.BlobId);

            new EditEventAttachmentCommand(Context).Execute(new EditEventAttachmentParameterSet
                                                                {
                                                                    EventAttachmentId = attacmentId, 
                                                                    Description = "test",
                                                                    FileName = "TestFileName.txt"
                                                                });

            attachment =
                new GetEventAttachmentListQuery(Context).Execute(eventId).First();

            Assert.IsTrue(attachment.Description == "test");

            CollectionAssert.AreEqual(data, blob.Data);

            new DeleteEventAttachmentCommand(Context).Execute(attacmentId);
            blob = new GetBlobByIdQuery(Context).Execute(attachment.BlobId);

            Assert.IsNull(blob);
        }
    }
}
