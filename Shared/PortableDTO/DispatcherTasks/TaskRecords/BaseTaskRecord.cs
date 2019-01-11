using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.DTO.DispatcherTasks.TaskRecords
{
    [DataContract]
    public class BaseTaskRecord : BaseDto<Guid>
    {

        [DataMember]
        public CommonEntityDTO Entity { get; set; }

        [DataMember]
        public Guid TaskId { get; set; }
        
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string PropertyTypeName { get; set; }

        [DataMember]
        public string TargetValue { get; set; }

        [DataMember]
        public DateTime? ExecutedDate { get; set; }

        [DataMember]
        public string ExecutedUserName { get; set; }
        
        [DataMember]
        public string ExecutedUserDescription { get; set; }

        [DataMember]
        public PropertyType PropertyTypeId { get; set; }

        [DataMember]
        public DateTime? CompletionDate { get; set; }

        [DataMember]
        public bool IsSpecialControl { get; set; }

        [DataMember]
        public string SpecialControlUserName { get; set; }

        [DataMember]
        public string SpecialControlUserDescription { get; set; }

        [DataMember]
        public int? SpecialControlUserId { get; set; }
    }
}