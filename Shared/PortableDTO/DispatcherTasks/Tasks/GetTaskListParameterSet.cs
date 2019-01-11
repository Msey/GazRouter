using System;

namespace GazRouter.DTO.DispatcherTasks.Tasks
{
    public class GetTaskListParameterSet
    {
        public Guid? SiteId { get; set; }

        public bool IsEnterprise { get; set; }

        public bool IsArchive { get; set; }

        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
    }
}
