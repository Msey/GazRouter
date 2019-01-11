using System;
using System.Linq;
using GazRouter.DAL.SysEvents;
using GazRouter.DTO.SysEvents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Event
{
    [TestClass]
    public class SysEventTests : TransactionTestsBase
    {

        [TestMethod ,TestCategory(UnStable)]
        public void FullTest()
        {
            var events = new GetSysEventListQuery(Context).Execute(new GetSysEventListParameters {EventStatusId = SysEventStatus.Waiting});
            var ev = events.FirstOrDefault();
            if (ev != null)
            {
                new SetStatusSysEventCommand(Context).Execute(new SetStatusSysEventParameters
                {
                    EventId = ev.Id,
                    ResultId = SysEventResult.Error,
                    EventStatusId = SysEventStatus.Finished
                });

                Assert.IsTrue(new GetSysEventListQuery(Context).Execute(new GetSysEventListParameters { EventStatusId = SysEventStatus.Finished }).Any(e => e.Id == ev.Id));
            }
            var createDate = DateTime.Today.AddDays(-10);
            events = new GetSysEventListQuery(Context).Execute(new GetSysEventListParameters { CreateDate = createDate});
            Assert.IsTrue(events.Any());
            Assert.IsTrue(events.TrueForAll(ev1 => ev1.CreateDate >= createDate));

        }
    }
}