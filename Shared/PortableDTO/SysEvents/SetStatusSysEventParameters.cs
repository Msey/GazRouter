using System;

namespace GazRouter.DTO.SysEvents
{
    public class SetStatusSysEventParameters
    {
        public SysEventResult? ResultId { get; set; }
        public SysEventStatus? EventStatusId { get; set; }
        public Guid EventId { get; set; }
    }
}