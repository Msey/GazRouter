using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.ExcelReports
{
    [DataContract]
    public class CellValue
    {
        [DataMember]
        public string RawValue { get; set; }

        [DataMember]
        public double Number { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }

        [DataMember]
        public CellValueType ValueType { get; set; }
    }
}