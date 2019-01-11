using GazRouter.DTO.EventLog;

namespace GazRouter.Modes.EventLog
{
    public interface IEventTab
    {
        EventDTO EventDTO { get; set; }
        string Header { get; }
        bool IsActive {set; }

        EventListType Type { get; set; }
    }
}