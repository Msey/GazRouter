using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.Repairs.RepairReport
{
    [DataContract]
    public class RepairReportAttachmentParamentersSet : BaseDto<int>
    {
        [DataMember]
        public int RepairReportId { get; set; }
        [DataMember]
        public string Description { get; set; }        
        [DataMember]
        public byte[] Data { get; set; }
        [DataMember]
        public string Filename { get; set; }
    }
}
