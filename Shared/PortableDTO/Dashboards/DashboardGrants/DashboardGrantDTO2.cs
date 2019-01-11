using System;
using System.Runtime.Serialization;
namespace GazRouter.DTO.Dashboards.DashboardGrants
{
    [DataContract]
    public class DashboardGrantDTO2
    {
        [DataMember]
        public int ItemId { get; set; }
        [DataMember]
        public Guid SiteId { get; set; }
        [DataMember]
        public int Permission { get; set; }
    }
}
/**/
//[DataMember]
//public int RoleId { get; set; }
/**/
