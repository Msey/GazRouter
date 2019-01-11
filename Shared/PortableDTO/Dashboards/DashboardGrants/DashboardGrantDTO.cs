using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Dashboards.DashboardGrants
{
	[DataContract]
	public class DashboardGrantDTO
	{
        [DataMember]
		public int UserId { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
		public string UserName { get; set; }

        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public string SiteName { get; set; }

        [DataMember]
        public bool IsVisible { get; set; }

        [DataMember]
		public bool IsEditable { get; set; }

		[DataMember]
		public bool IsGrantable { get; set; }
	}
}