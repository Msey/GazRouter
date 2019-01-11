using System;
using System.Linq;
using GazRouter.DAL.Authorization.User;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DAL.EventLog;
using GazRouter.DAL.EventLog.Attachments;
using GazRouter.DAL.EventLog.EventRecipient;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Dictionaries.EventPriorities;
using GazRouter.DTO.EventLog;
using GazRouter.DTO.EventLog.Attachments;
using GazRouter.DTO.EventLog.EventRecipient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Event
{
    [TestClass]
    public class EventsTest : EventsTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void TestRegisterEvent()
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


            var eventId = RegisterEvent(enterpriseId);

            Assert.IsTrue(eventId != default(int));


            new EditEventCommand(Context).Execute(new EditEventParameterSet
                {
                    EventDate = DateTime.Now,
                    Text = "TestTextAfterEdit",
                    EntityId = enterpriseId,
                    Id = eventId
                });
            var events =
                new GetEventListQuery(Context).Execute(new GetEventListParameterSet
                    {
                        SiteId = enterpriseId,
                        QueryType = EventListType.List
                    });
            var currentevent = events.FirstOrDefault(r => r.Id == eventId);
            Assert.IsNotNull(currentevent);
            Assert.IsTrue(currentevent.Description == "TestTextAfterEdit");
            new DeleteEventCommand(Context).Execute(eventId);
        }

        [TestMethod ,TestCategory(Stable)]
        public void TestAttachementEvent()
        {
            var enterpriseId = GetEnterpriseId();


            var eventId = RegisterEvent(enterpriseId);


            var attachID = new AddEventAttachmentCommand(Context).Execute(new AddEventAttachmentParameterSet { Data = new byte[] { }, EventId = eventId, FileName = "TestFile", Description = "Description" });

            var attachements =
                new GetEventAttachmentListQuery(Context).Execute(eventId);
       
            AssertHelper.IsNotEmpty(attachements);
            Assert.AreEqual(attachID, attachements.Single().Id);

            new DeleteEventAttachmentCommand(Context).Execute(attachID);
            attachements =
                new GetEventAttachmentListQuery(Context).Execute(eventId);

            AssertHelper.IsEmpty(attachements);
        }

        

        [TestMethod ,TestCategory(Stable)]
        public void TestEventRecipient()
        {
            var enterpriseId = GetEnterpriseId();
            var eventId = RegisterEvent(enterpriseId);
            Assert.IsTrue(eventId != default(int));

            var recip =
                new AddEventRecipientCommand(Context).Execute(new AddEventRecipientParameterSet
                    {
                        EventId = eventId,
                        Priority = EventPriority.Normal,
                        SiteId = enterpriseId
                    });
            Assert.IsTrue(recip != default(Guid));
            var recepients = new GetEventRecepientListQuery(Context).Execute(eventId);
            var recipe = recepients.FirstOrDefault(r => r.Id == recip);
            Assert.IsTrue(recipe != null);

            var possibleRecepients = new GetEnterpriseAndSitesQuery(Context).Execute(enterpriseId);
            Assert.IsTrue(possibleRecepients.Any(r => r.Id == recipe.SiteId));


            new AckEventCommand(Context).Execute(new AckEventParameterSet { EventId = eventId, SiteId = enterpriseId });
            recepients = new GetEventRecepientListQuery(Context).Execute(eventId);
            recipe = recepients.FirstOrDefault(r => r.Id == recip);
            Assert.IsTrue(recipe != null);
            new ResetAckEventCoommand(Context).Execute(recip);

            recepients = new GetEventRecepientListQuery(Context).Execute(eventId);
            recipe = recepients.FirstOrDefault(r => r.Id == recip);
            Assert.IsTrue(recipe != null);
            Assert.IsTrue(!recipe.AckDate.HasValue);

            new TakeToControlEventCommand(Context).Execute(new TakeToControlEventParameterSet
                {
                    EventId = eventId,
                    SiteId = enterpriseId
                });

            new BackToNormalEventCommand(Context).Execute(new BackToNormalEventParameterSet
                {
                    EventId = eventId,
                    SiteId = enterpriseId
                });
            new MoveToTrashEventCommand(Context).Execute(new MoveToTrashEventParameterSet
                {
                    EventId = eventId,
                    SiteId = enterpriseId
                });
            new RestoreFromTrashEventCommand(Context).Execute(new RestoreFromTrashEventParameterSet
                {
                    EventId = eventId,
                    SiteId = enterpriseId
                });

            new DeleteEventRecipientCommand(Context).Execute(recip);
            new DeleteEventCommand(Context).Execute(eventId);
        }
    }
}
