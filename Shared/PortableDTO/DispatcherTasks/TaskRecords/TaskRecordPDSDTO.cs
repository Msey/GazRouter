using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.DispatcherTasks.TaskRecords
{
    [DataContract]
	public class TaskRecordPdsDTO : BaseTaskRecord
    {
	    [DataMember]
		public int TaskNum { get; set; }

        [DataMember]
		public DateTime? PerfDateTime { get; set; }

		[DataMember]
		public string PerfUser { get; set; }

		[DataMember]
		public string AproveUserName { get; set; }
        
        [DataMember]
        public string AproveUserDescription { get; set; }



        [DataMember]
		public string Status { get; set; }

        [DataMember]
        public DateTime? AckDate { get; set; }

        [DataMember]
        public int AckUserId { get; set; }

        [DataMember]
        public string AckUserName { get; set; }

        [DataMember]
        public string AckUserDescription { get; set; }

    }
}
