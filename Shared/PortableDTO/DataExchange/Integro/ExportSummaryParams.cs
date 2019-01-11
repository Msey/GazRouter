using GazRouter.DTO.Dictionaries.PeriodTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.DataExchange.Integro
{
    [DataContract]
    public class ExportSummaryParams
    {
        [DataMember]
        public int? SystemId { get; set; }
        [DataMember]
        public Guid SummaryId { get; set; }
        [DataMember]
        public DateTime PeriodDate { get; set; }
        [DataMember]
        public int? SeriesId { get; set; }
        [DataMember]
        public List<int> ContractIds{ get; set; }        
        [DataMember]
        public PeriodType? PeriodType { get; set; }
        [DataMember]
        public int? ExchangeTaskId { get; set; }
        [DataMember]
        public bool GetResult { get; set; }
        [DataMember]
        public bool GetFromLog { get; set; }
        [DataMember]
        public string FileLogName { get; set; }
        

    }
}
