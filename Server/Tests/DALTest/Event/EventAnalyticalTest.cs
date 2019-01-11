using System;
using System.Linq;
using System.Threading;
using GazRouter.DAL.Authorization.User;
using GazRouter.DAL.EventLog.EventAnalytical;
using GazRouter.DAL.EventLog.EventRecipient;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Dictionaries.EventPriorities;
using GazRouter.DTO.EventLog.EventAnalytical;
using GazRouter.DTO.EventLog.EventRecipient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.Event
{
    [TestClass]
    public class EventAnalyticalTest : EventsTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void GetEventAnalyticalList()
        {
            var enterpriseId = GetEnterpriseId();
            Assert.IsTrue(enterpriseId != default(Guid));

            var user = GetCurrentUser();
            new EditUserCommand(Context).Execute(new EditUserParameterSet
                {
                    Id = user.Id,
                    Description = user.Description,
                    UserName = user.UserName,
                    SiteId = enterpriseId
                });

            var siteId = CreateSite(enterpriseId);

            var eventId = RegisterEvent(enterpriseId);
            Assert.IsTrue(eventId != default(int));
            new AddEventRecipientCommand(Context).Execute(new AddEventRecipientParameterSet
                {
                    EventId = eventId,
                    SiteId = siteId,
                    Priority = EventPriority.Normal
                });
            var analytical =
                new GetEventAnalyticalListQuery(Context).Execute(new EventAnalyticalParameterSet
                    {
                        DateBegin = DateTime.Now.AddMinutes(-1),
                        DateEnd = DateTime.Now.AddDays(1)
                    });
            Assert.IsTrue(analytical.Any());
            Thread.Sleep(500);
            new AckEventCommand(Context).Execute(new AckEventParameterSet { EventId = eventId, SiteId = siteId });
            analytical =
                new GetEventAnalyticalAckListQuery(Context).Execute(new EventAnalyticalParameterSet
                    {
                        DateBegin = DateTime.Now.AddMinutes(-1),
                        DateEnd = DateTime.Now.AddDays(2)
                    });
            Assert.IsTrue(analytical.Any());
        }
    }
}
