using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DAL.ExcelReport;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ExcelReports;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;

namespace GazRouter.Service.Exchange.Lib.Excel.CellEvaluators
{
    public class SqlCellEvaluator : CellEvaluator
    {
        public SqlCellEvaluator(DateTime timeStamp, PeriodType periodType, ExecutionContext ctx) : base(timeStamp,
            periodType, ctx)
        {
        }

        protected override Regex Regex => new Regex(@"#SQL#([^@]*(@TIMESTAMP\s*(([\+\-])([^\+\-\']+))?)?.*)");

        public void Parse(string input, out string exec)
        {
            var m = Regex.Match(input);
            exec = m.Groups[1].Value;
            if (m.Groups[2].Success)
            {
                var timeStamp = GetCalculatedTimeStamp(m.Groups[3].Value);
                exec = Regex.Replace(exec, m.Groups[2].Value, timeStamp.ToString("dd.MM.yyyy HH:mm:ss"));
            }
        }
        public override CellValue GetValue(string rawValue)
        {
            string exec;
            Parse(rawValue, out exec);
            var result = new RunSqlProcCommand(Context).Execute(exec);

            return new CellValue() {RawValue = result, ValueType = CellValueType.Text};
        }
    }
}