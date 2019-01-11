using GazRouter.DTO.Dictionaries.PeriodTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.DataExchange.Integro
{
    [DataContract]
    [KnownType(typeof(SummaryParamContentDTO))]
    public class SummaryParamDTO : NamedDto<Guid>
    {
        [DataMember]
        public Guid SummaryId { get; set; }

        [DataMember]
        public string ParameterGid { get; set; }

        [DataMember]
        public string Aggregate { get; set; }

        [DataMember]
        public List<SummaryParamContentDTO> SummaryParamContentList { get; set; }
    }
}
