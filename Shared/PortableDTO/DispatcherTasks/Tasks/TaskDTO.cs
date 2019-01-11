using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.StatusTypes;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;

namespace GazRouter.DTO.DispatcherTasks.Tasks
{
    [DataContract]
    public class TaskDTO : BaseDto<Guid>
    {

        public TaskDTO()
        {
            StatusList = new List<TaskStatusDTO>();
        }

		[DataMember]
        public int TaskNumber { get; set; }

        [DataMember]
        public Guid LastVersionId { get; set; }

        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string GlobalTaskId { get; set; }

        [DataMember]
		public StatusType StatusType { get; set; }

        [DataMember]
        public DateTime StatusSetDate { get; set; }

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
        public string StatusTypeCode { get; set; }

        [DataMember]
        public List<TaskStatusDTO> StatusList { get; set; }

        [DataMember]
        public bool IsOverdue { get; set; }

        [DataMember]
        public bool IsComplete { get; set; }

        public bool IsArchive => StatusType == StatusType.Performed || StatusType == StatusType.Annuled;

        
    }
}
