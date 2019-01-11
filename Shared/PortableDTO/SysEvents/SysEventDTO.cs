using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.SysEvents
{
    [DataContract]
    public class SysEventDTO : BaseDto<Guid>
    {
        public string Description { get; set; }

        public string SysName { get; set; }
        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; }
        public SysEventStatus? StatusId { get; set; }
        public int SeriesId { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? EntityId { get; set; }
        public PeriodType? PeriodTypeId { get; set; }
        public DateTime KeyDate { get; set; }
        public SysEventResult? ResultId { get; set; }
    }
}