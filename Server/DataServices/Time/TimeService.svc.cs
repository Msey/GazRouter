using System;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DTO;

namespace GazRouter.DataServices.Time
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class TimeService : ServiceBase, ITimeService
    {
        public DateTime GetTimeServer()
        {
            return DateTime.Now;
        }

        public Guid GetServerState(Module parameters)
        {
            return ModelState.ModulesStates[parameters];
        }
    }
}
