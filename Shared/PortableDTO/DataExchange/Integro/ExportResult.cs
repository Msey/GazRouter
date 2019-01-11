using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.DataExchange.Integro
{
    [DataContract]
    public class ExportResult
    {
        [DataMember]
        public ExportResultType ResultType { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string ExportData { get; set; }

        [DataMember]
        public string ExportFileName { get; set; }
        [DataMember]
        public string LogData { get; set; }
    }

    public enum ExportResultType
    {
        Successful,
        Error,
        ValidationError
    }
}
