using System.Runtime.Serialization;
namespace GazRouter.DTO.Dashboards.DashboardContent
{
    [DataContract]
    public class DashboardContentDTO
    {
        [DataMember]
        public int DashboardId { get; set; }
        
        [DataMember]
        public string Content{ get; set; }
    }
}