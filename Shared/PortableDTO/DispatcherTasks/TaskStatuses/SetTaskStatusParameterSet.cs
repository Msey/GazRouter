using System;
using GazRouter.DTO.Dictionaries.AnnuledReasons;
using GazRouter.DTO.Dictionaries.StatusTypes;

namespace GazRouter.DTO.DispatcherTasks.TaskStatuses
{
    public class SetTaskStatusParameterSet
	{
        public Guid TaskId { get; set; }

        public StatusType StatusType { get; set; }

        public string UserNameCpdd { get; set; }

        public AnnuledReason? AnnuledReason { get; set; }

        public string ReasonDescription { get; set; }
    }
}
