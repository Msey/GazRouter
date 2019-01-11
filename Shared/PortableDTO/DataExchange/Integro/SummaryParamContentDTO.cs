using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.DataExchange.Integro
{
    [DataContract]
    public class SummaryParamContentDTO : NamedDto<Guid>
    {
        [DataMember]
        public Guid SummaryParamId { get; set; }

        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public int PropertyTypeId { get; set; }
    }
}
