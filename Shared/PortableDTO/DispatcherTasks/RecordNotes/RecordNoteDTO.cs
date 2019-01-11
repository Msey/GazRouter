using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.DispatcherTasks.RecordNotes
{
    [DataContract]
    public class RecordNoteDTO : BaseDto<Guid>
    {
		[DataMember]
        public Guid TaskId { get; set; }

        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public int PropertyTypeId { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public int CreateUserId { get; set; }

        [DataMember]
        public string CreateUserName { get; set; }

        [DataMember]
        public string CreateUserDescription { get; set; }
    }
}
