using System;
using GazRouter.DAL.EventLog;
using GazRouter.DTO.EventLog;

namespace DALTest.Event
{
    public abstract class EventsTestBase : DalTestBase
    {
        protected int RegisterEvent(Guid enterpriseId)
        {
            return new RegisterEventCommand(Context).Execute(new RegisterEventParameterSet
                {
                    EventDate = DateTime.Now,
                    Text = "TestText1",
                    EntityId = enterpriseId
                });
        }
    }
}