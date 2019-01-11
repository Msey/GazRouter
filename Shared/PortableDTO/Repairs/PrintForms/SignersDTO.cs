using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.Repairs.PrintForms
{
    [DataContract]
    public class SignersDTO
    {
        [DataMember]
        public List<TargetingUserDTO> To { get; set; }
        [DataMember]
        public List<TargetingUserDTO> From { get; set; }
    }
}
