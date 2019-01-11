
using System;
using System.Collections.Generic;

namespace GazRouter.DTO.DispatcherTasks.Tasks
{
    public class AddTaskParameterSet
	{
        public AddTaskParameterSet()
        {
            SiteIdList = new List<Guid>();
        }

        public string Subject { get; set; }
        public string Description { get; set; }

        public DateTime CompletionDate { get; set; }

        public List<Guid> SiteIdList { get; set; }

        // Идентификатор ДЗ для создания нового ДЗ путем копирования старого
        public Guid SourceTaskId { get; set; }
	}
}
