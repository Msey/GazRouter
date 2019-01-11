using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;

namespace GazRouter.DTO.DispatcherTasks.TaskRecords
{
    [DataContract]
	public class TaskRecordDTO : BaseTaskRecord
    {
	    [DataMember]
	    public Guid TaskVersionId { get; set; }
	    
		[DataMember]
        public PeriodType PeriodTypeId { get; set; }

	    [DataMember]
	    public int OrderNo { get; set; }

		[DataMember]
	    public int ExecutedUserId { get; set; }

		[DataMember]
	    public DateTime CreateDate { get; set; }

		[DataMember]
	    public int CreateUserId { get; set; }

		[DataMember]
	    public string CreateUserName{ get; set; }

        [DataMember]
        public string CreateUserDescription { get; set; }


        [DataMember]
        public DateTime? AckDate { get; set; }

        [DataMember]
        public int? AckUserId { get; set; }

        [DataMember]
        public string AckUserName { get; set; }

        [DataMember]
        public string AckUserDescription { get; set; }



        [DataMember]
        public PhysicalType PhysicalTypeId { get; set; }

		[DataMember]
	    public int DictId { get; set; }

		[DataMember]
	    public string PhisicalTypeName { get; set; }

		[DataMember]
	    public string PhisicalTypeSysName { get; set; }

		[DataMember]
	    public int NoteCount { get; set; }

		[DataMember]
		public Guid SiteId { get; set; }

        [DataMember]
        public string SiteName { get; set; }

        
		public string ExtKey { get; set; }
    }
}
