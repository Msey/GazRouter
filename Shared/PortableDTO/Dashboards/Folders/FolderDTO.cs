using System.Runtime.Serialization;
namespace GazRouter.DTO.Dashboards.Folders
{
	[DataContract]
    public class FolderDTO : NamedDto<int>
	{
		[DataMember]
		public string CreatorUserId { get; set; }
		[DataMember]
		public int? ParentId { get; set; }
		[DataMember]
		public int SortOrder{ get; set; }
	}
}
//[DataMember]//		public string UserName { get; set; }