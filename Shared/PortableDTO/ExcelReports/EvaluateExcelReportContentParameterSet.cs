using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.ExcelReports
{
    public class EvaluateExcelReportContentParameterSet 
    {
        public EvaluateExcelReportContentParameterSet()
        {
            IsEditMode = true;
            PeriodType = PeriodType.Twohours;
            CellsToChange = new List<SerializableTuple4<int, int, int, string>>();
        }

        public int ReportId { get; set; }
        public DateTime KeyDate { get; set; }
        public bool IsEditMode { get; set; }
        public PeriodType PeriodType { get; set; }
        public List<SerializableTuple4<int, int, int, string>> CellsToChange { get; set; }
    }
}