using GazRouter.DTO.DataExchange.ExchangeTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.DataExchange.Integro
{
    [DataContract]
    [KnownType(typeof(AddEditSummaryParameterSet))]
    [KnownType(typeof(EditExchangeTaskParameterSet))]
    public class SummaryExchTaskParamSet
    {
        [DataMember]
        public AddEditSummaryParameterSet SummatyParam { get; set; }
        [DataMember]
        public EditExchangeTaskParameterSet ExchTaskParam { get; set; }
    }
}
