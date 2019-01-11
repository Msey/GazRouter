using System.Runtime.Serialization;

namespace GazRouter.DTO.ExcelReports
{
    [DataContract]
    public class DashboardGrantDTO
    {
        [DataMember]
        public int DashboardId { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public bool IsEdit { get; set; }

        [DataMember]
        public bool IsGrantable { get; set; }
    }
}