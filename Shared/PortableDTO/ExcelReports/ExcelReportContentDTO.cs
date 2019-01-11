using System.Collections.Generic;
using System.Runtime.Serialization;
namespace GazRouter.DTO.ExcelReports
{
    [DataContract]
    public class ExcelReportContentDTO
    {
        public ExcelReportContentDTO()
        {
            Content = null;
            ChangedCells = new List<SerializableTuple4<int, int, int, CellValue>>();
        }

        [DataMember]
        public int ReportId { get; set; }
        
        [DataMember]
        public byte[] Content{ get; set; }
        
                
        [DataMember]
        public List<SerializableTuple4<int, int, int, CellValue>> ChangedCells { get; set; }
    }
}