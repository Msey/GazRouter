using System;
using GazRouter.DTO.Dictionaries.AnnuledReasons;

namespace GazRouter.DTO.DispatcherTasks.TaskStatuses
{
    public class TaskAnnuledParameterSet
	{
        public Guid TaskId { get; set; }
        public AnnuledReason AnnuledReason { get; set; }
        public string ReasonDescription { get; set; }
	}
}
