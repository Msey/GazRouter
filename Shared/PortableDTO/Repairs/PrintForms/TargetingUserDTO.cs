using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.Repairs.PrintForms
{
    [DataContract]
    public class TargetingUserDTO
    {
        [DataMember]
        public string Position { get; set; }
        [DataMember]
        public string FIO { get; set; }
        [DataMember]
        public string Fax { get; set; }
        [DataMember]
        public int SortOrder { get; set; }
        [DataMember]
        public bool IsHead { get; set; }
    }
}
