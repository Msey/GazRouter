using System.Runtime.Serialization;
namespace GazRouter.DTO.Dashboards
{
    [DataContract]
    public class DashSortOrderDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int? SortOrder { get; set; }
        [DataMember]
        public InfopanelItemType Type { get; set; }
    }
}
