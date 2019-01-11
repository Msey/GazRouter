using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.Folders;
using GazRouter.DTO.ObjectModel.Sites;
namespace GazRouter.DTO.Dashboards
{
    [DataContract]
    public class DashboardDataDTO
    {
        [DataMember]
        public int[] UserRoleIds { get; set; }
        [DataMember]
        public List<FolderDTO> FolderUnionDtos { get; set; }
        [DataMember]
        public List<DashboardDTO> DashDtos { get; set; }
        [DataMember]
        public List<DashboardDTO> ExcelDtos { get; set; }
        [DataMember]
        public Dictionary<int, int> MaxRolesPermissions { get; set; }
        [DataMember]
        public Dictionary<int, int> FolderPanelIdMaxPermission { get; set; }
    }
}
#region trash
//        [DataMember]
//        public List<SiteDTO> Sites { get; set; }
/**/ // todo:
     //        [DataMember]
     //        public List<FolderDTO> DashFoldersDtos { get; set; }
     //        [DataMember]
     //        public List<FolderDTO> ExcelFoldersDtos { get; set; }
     /**/

//        [DataMember]
//        public List<DashboardGrantDTO2> Permissions { get; set; }

//        [DataMember]
//        public int[] RoleIds { get; set; }
#endregion