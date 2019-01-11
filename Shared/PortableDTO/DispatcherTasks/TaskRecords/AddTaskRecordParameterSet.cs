using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.DispatcherTasks.TaskRecords
{
    public class AddTaskRecordParameterSet
	{
		public Guid TaskId { get; set; }
		public Guid EntityId { get; set; }
        public PropertyType PropertyTypeId { get; set; }
        public PeriodType PeriodTypeId { get; set; }
		public string TargetValue { get; set; }
		public DateTime CompletionDate { get; set; }
		public int OrderNo { get; set; }
		public string Description { get; set; }
	}

    public class AddTaskRecordCpddParameterSet : AddTaskRecordParameterSet
    {
        public string UserNameCpdd { get; set; }
    }

    public class AddTaskRecordPdsParameterSet : AddTaskRecordParameterSet
    {
        public Guid SiteId { get; set; }
    }

    public class EditTaskRecordCpddParameterSet : AddTaskRecordCpddParameterSet
    {
        public Guid RowId { get; set; }
    }

    public class EditTaskRecordPdsParameterSet : AddTaskRecordPdsParameterSet
    {
        public Guid RowId { get; set; }
    }
}
