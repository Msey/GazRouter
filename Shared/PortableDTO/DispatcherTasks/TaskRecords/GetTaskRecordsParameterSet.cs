using System;

namespace GazRouter.DTO.DispatcherTasks.TaskRecords
{
	public abstract class GetTaskRecordsParameterSet
	{
        public bool IsCpdd { get; set; }
	}

	public class GetTaskRecordsCpddParameterSet : GetTaskRecordsParameterSet
	{
		public Guid TaskVersionId { get; set; }
	}

	public class GetTaskRecordsPdsParameterSet
	{
		public Guid SiteId { get; set; }
		public bool IsArchive { get; set; }
        public DateTime? BeginDate  { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
