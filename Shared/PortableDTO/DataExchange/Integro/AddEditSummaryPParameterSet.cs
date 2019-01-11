using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.DataExchange.Integro
{
    [DataContract]
    [KnownType(typeof(AddEditSummaryPContentParameterSet))]
    public class AddEditSummaryPParameterSet
    {

        //p_SUMMARY_PARAMETER_ID
        [DataMember]
        public Guid SummaryParamId { get; set; }
        //p_SUMMARY_ID
        [DataMember]
        public Guid SummaryId { get; set; }
        // p_NAME
        [DataMember]
        public string Name { get; set; }
        //p_PARAMETER_GID
        [DataMember]
        public string ParametrGid { get; set; }
        //p_AGGREGATE
        [DataMember]
        public string Aggregate { get; set; }
        // List of AddEditSummaryPContentParameterSet
        [DataMember]
        public List<AddEditSummaryPContentParameterSet> ContentList { get; set; }

    }
}
