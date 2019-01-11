using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.StatusTypes;

namespace GazRouter.DTO.DispatcherTasks.TaskStatuses
{
    [DataContract]
    public class TaskStatusDTO : BaseDto<Guid>
    {
		[DataMember]
        public Guid TaskId { get; set; }

        [DataMember]
		public StatusType StatusTypeId { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public int CreateUserId { get; set; }

        [DataMember]
        public string CreateUserName { get; set; }

        [DataMember]
        public string CreateUserDescription { get; set; }

        [DataMember]
        public string StatusTypeName { get; set; }

		[DataMember]
		public string GlobalTaskId { get; set; }

		[DataMember]
		public bool IsLastVersion { get; set; }

        [DataMember]
        public string Reason { get; set; }
    }
}
