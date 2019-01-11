using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ExcelReports;

namespace GazRouter.Service.Exchange.Lib.Excel.CellEvaluators
{
    public abstract class CellEvaluator
    {
        protected readonly ExecutionContext Context;
        protected readonly PeriodType PeriodType;
        protected readonly DateTime TimeStamp;

        protected CellEvaluator(DateTime timeStamp, PeriodType periodType, ExecutionContext context)
        {
            TimeStamp = timeStamp;
            PeriodType = periodType;
            Context = context;
        }

        protected abstract Regex Regex { get; }


        public bool IsMatch(string value)
        {
            return Regex.IsMatch(value);
        }


        public virtual CellValue GetValue(string rawValue)
        {
            return new CellValue {ValueType = CellValueType.Empty};
        }

        protected DateTime GetCalculatedTimeStamp(string expression)
        {
            double value = 0;
            try
            {
                expression = string.IsNullOrEmpty(expression) ? "0" : expression;
                value = Convert.ToDouble(new DataTable().Compute(expression, string.Empty));
            }
            catch (Exception)
            {
            }
            return TimeStamp + TimeSpan.FromHours(value * 24);
        }

    }
}