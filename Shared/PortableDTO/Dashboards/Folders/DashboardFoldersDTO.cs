using System.Runtime.Serialization;
namespace GazRouter.DTO.Dashboards.Folders
{
    [DataContract]
    public class DashboardFoldersDTO
    {
        [DataMember]
        public int DashboardId { get; set; }
        [DataMember]
        public int FolderId { get; set; }
        [DataMember]
        public int? SortOrder { get; set; }
    }
}
