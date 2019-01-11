using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PeriodTypes;
namespace GazRouter.DTO.Dashboards.Dashboard
{
	[DataContract]
	public class DashboardDTO : BaseDto<int>
	{
		[DataMember]
		public string DashboardName { get; set; }
		[DataMember]
		public string CreatorUserId { get; set; }
		[DataMember]
		public string CreatorUserName { get; set; }
        [DataMember]
        public int? FolderId { get; set; }
        [DataMember]
		public DateTime CreateDate { get; set; }
		[DataMember]
		public int? SortOrder { get; set; }
        [DataMember]
        public PeriodType PeriodTypeId { get; set; }
        [DataMember]
        public int RowType { get; set; }
        [DataMember]
        public int IsDeleted { get; set; }

        #region deprecated
        [DataMember]public bool IsEditable { get; set; }
        [DataMember]public bool IsGrantable { get; set; }
#endregion
    }
}