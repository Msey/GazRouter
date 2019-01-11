using GazRouter.DTO.DispatcherTasks.TaskStatuses;

namespace GazRouter.Modes.DispatcherTasks.PDS
{
    public interface ITaskDetailsTab
    {
        string Header { get; }
        TaskStatusDTO Status { get; set; }
        void Refresh();
    }
}