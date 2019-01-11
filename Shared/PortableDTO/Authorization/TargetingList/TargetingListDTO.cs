using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.Authorization.TargetingList
{
    [DataContract]
    public class TargetingListDTO : BaseDto<int>
    {
        //    l.entity_type_id, l.sortorder, l.agreed_list_id, 
        //a.agreed_user_id, a.user_id, a.fio, a.position, a.start_date, a.end_date, a.is_head, 
        //en.entity_id, en.entity_name Site_Name
        
        [DataMember]
        public int SortOrder { get; set; }
        [DataMember]
        public int EntityTypeId { get; set; }
        [DataMember]
        public string EntityName { get; set; }
        [DataMember]
        public int AgreedUserId { get; set; }
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string FIO { get; set; }
        [DataMember]
        public string Position { get; set; }
        [DataMember]
        public string Fax { get; set; }
        [DataMember]
        public bool IsHead { get; set; }
        [DataMember]
        public Guid SiteId { get; set; }
        [DataMember]
        public string SiteName { get; set; }
        [DataMember]
        public bool IsCpdd { get; set; }
    }
}
