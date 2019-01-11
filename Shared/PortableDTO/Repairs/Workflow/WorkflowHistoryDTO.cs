using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.Repairs.Workflow
{
    [DataContract]
    public class WorkflowHistoryDTO
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int RepairID { get; set; }
        [DataMember]
        public int WFStateCode { get; set; }
        [DataMember]
        public string WFState { get; set; }
        [DataMember]
        public int WStateCode { get; set; }
        [DataMember]
        public string WState { get; set; }
        [DataMember]
        public DateTime EventDate { get; set; }
        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string UserDescription { get; set; }
        [DataMember]
        public Guid UserSiteID { get; set; }
        [DataMember]
        public string SiteName { get; set; }
    }
}
