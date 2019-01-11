using System;
using System.IO;
using GazRouter.DAL.Core;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ExcelReports;

namespace GazRouter.Service.Exchange.Lib.Excel
{

    public static class ExcelHelper
    {

        //public static Dictionary<string, CellValue> GetTemplateValues(Stream stream, EvaluateExcelReportContentParameterSet parameterSet, ExecutionContext context = null)
        //{
        //    return new ExcelProcessor(parameterSet.KeyDate, parameterSet.PeriodType, context).GetTemplateValues(stream);
        //}
        public static List<SerializableTuple4<int, int, int, CellValue>> GetTemplateValues(EvaluateExcelReportContentParameterSet parameterSet, ExecutionContext context = null)
        {
            return new ExcelProcessor(parameterSet.KeyDate, parameterSet.PeriodType, context).GetTemplateValues(parameterSet.CellsToChange);
        }

    }
}