using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.Repairs.PrintForms
{
    [DataContract]
    public class GetSignersSet
    {
        [DataMember]
        public int EntityTypeId { get; set; }
        [DataMember]
        public Guid ToId { get; set; }
        [DataMember]
        public Guid? FromId { get; set; }
        [DataMember]
        public bool IsCpdd { get; set; } = false;
    }
}
