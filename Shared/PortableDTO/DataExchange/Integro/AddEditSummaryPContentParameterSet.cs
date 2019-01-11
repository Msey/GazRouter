using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.DataExchange.Integro
{
    [DataContract]
    public class AddEditSummaryPContentParameterSet
    {

        [DataMember]
        //p_SUMMARY_PARAMETER_ID
        public Guid SummaryParamId { get; set; }
        [DataMember]
        //p_ENTITY_ID
        public Guid EntityId { get; set; }
        [DataMember]
        //p_ENTITY_ID
        public Guid EntityIdOriginal { get; set; }
        [DataMember]
        // p_PROPERTY_TYPE_ID
        public int PropertyTypeId { get; set; }
    }
}
