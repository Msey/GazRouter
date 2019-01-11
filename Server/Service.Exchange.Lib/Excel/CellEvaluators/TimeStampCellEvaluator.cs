using System;
using System.Text.RegularExpressions;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ExcelReports;

namespace GazRouter.Service.Exchange.Lib.Excel.CellEvaluators
{
    public class TimeStampCellEvaluator : CellEvaluator
    {
        public TimeStampCellEvaluator(DateTime timeStamp, PeriodType periodType, ExecutionContext ctx) : base(timeStamp,
            periodType, ctx)
        {
        }

        protected override Regex Regex => new Regex(@"@TIMESTAMP\s*(([+-])([^+-]+))?");

        private void Parse(string input, out DateTime timeStamp)
        {
            var m = Regex.Match(input);
            timeStamp = GetCalculatedTimeStamp(m.Groups[1].Value);
        }

        public override CellValue GetValue(string rawValue)
        {
            DateTime timeStamp;
            var input = rawValue;
            Parse(input, out timeStamp);
            return new CellValue {ValueType = CellValueType.DateTime, DateTime = timeStamp};
        }

    }
}