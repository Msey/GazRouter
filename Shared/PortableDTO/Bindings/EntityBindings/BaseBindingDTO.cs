using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Bindings.EntityBindings
{
    [DataContract]
    public class BaseBindingDTO
    {
        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public List<BindingDTO> Entities { get; set; }
    }
}