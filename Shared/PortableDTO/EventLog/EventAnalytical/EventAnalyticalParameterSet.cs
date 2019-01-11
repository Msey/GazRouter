using System;

namespace GazRouter.DTO.EventLog.EventAnalytical
{
    public class EventAnalyticalParameterSet
    {
        public Guid? SiteId { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
    }
}