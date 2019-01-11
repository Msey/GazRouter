namespace GazRouter.DTO.SysEvents
{
    public class AddSysEventParameters
    {
        public SysEventType? EventTypeId { get; set; }
        public SysEventStatus? EventStatusId { get; set; }
        public SysEventStatus? EventStatusIdMii { get; set; }
        public string Description { get; set; }
        public int SeriesId { get; set; }
    }
}