using System;
using System.Linq;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataLoadMonitoring;
using GazRouter.DTO.DataLoadMonitoring;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.Service.Exchange.Lib.Excel.CellEvaluators
{
    public class GsVolumeEntFieldEvaluator : CellEvaluator
    {
        protected override Regex Regex
            => new Regex(@"#FIELD_GS_ENT#([^#]*)#([^#]*)#(@TIMESTAMP\s*(([+-])([^+-]+))?)");

        public GsVolumeEntFieldEvaluator(DateTime timeStamp, PeriodType periodType, ExecutionContext ctx) : base(timeStamp, periodType, ctx)
        {
        }

        private void Parse(string input, out TimeSpan span, out DateTime timeStamp)
        {
            var m = Regex.Match(input);
            var hdm = Convert.ToString(m.Groups[1].Value);
            var shift = Convert.ToInt32(m.Groups[2].Value);
            span = hdm == "H" ? TimeSpan.FromHours(shift) : hdm == "D" ? TimeSpan.FromDays(shift) : TimeSpan.FromDays(30*shift);

            timeStamp = GetCalculatedTimeStamp(m.Groups[4].Value);
        }


        protected override void _Process(IXLCell cell)
        {
            DateTime timeStamp;
            var input = cell.GetString();
            TimeSpan span;
            Parse(input, out span, out timeStamp);
            var @params = new GasSupplySumParameterSet
            {
                KeyDate = timeStamp,
                CountDays = span.Days,
                PeriodType = PeriodType
            };


            var values = new GetSumGasVolumeByEnterprise(Context).Execute(@params).Select(dto => dto.GazVolume).ToArray();

            SetValues(cell, values);
        }
    }
}