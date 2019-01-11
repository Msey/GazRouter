using System;
using System.Linq;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataLoadMonitoring;
using GazRouter.DTO.DataLoadMonitoring;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Utils.Extensions;

namespace GazRouter.Service.Exchange.Lib.Excel.CellEvaluators
{
    public class GsVolumeChangeFieldEvaluator : CellEvaluator
    {
        protected override Regex Regex
            => new Regex(@"#FIELD_GS_CH#([^#]*)#([^#]*)#([^#]*)#([^#]*)#([^#]*)#(@TIMESTAMP\s*(([+-])([^+-]+))?)");

        public GsVolumeChangeFieldEvaluator(DateTime timeStamp, PeriodType periodType, ExecutionContext ctx) : base(timeStamp, periodType, ctx)
        {
        }

        public void Parse(string input, out Guid pipeLineId, out int kms, out int kme, out TimeSpan span, out DateTime timeStamp)
        {
            var m = Regex.Match(input);
            pipeLineId = new Guid(m.Groups[1].Value).Convert();
            kms = Convert.ToInt32(m.Groups[2].Value);
            kme = Convert.ToInt32(m.Groups[3].Value);

            var hdm = Convert.ToString(m.Groups[4].Value);
            var shift = Convert.ToInt32(m.Groups[5].Value);
            span = hdm == "H" ? TimeSpan.FromHours(shift) : hdm == "D" ? TimeSpan.FromDays(shift) : TimeSpan.FromDays(30*shift);

            timeStamp = GetCalculatedTimeStamp(m.Groups[7].Value);
        }


        protected override void _Process(IXLCell cell)
        {
            DateTime timeStamp;
            Guid pipeLineId;
            int kms, kme;
            var input = cell.GetString();
            TimeSpan span;
            Parse(input, out pipeLineId, out kms, out kme, out span, out timeStamp);
            var @params = new GasSupplySumParameterSet
            {
                KeyDate = timeStamp,
                CountDays = span.Days,
                KmBegin = kms,
                KmEnd = kme,
                PipelineId = pipeLineId,
                PeriodType = PeriodType
            };


            var values = new GetSumGasVolumeByEnterprise(Context).Execute(@params).Select(dto => dto.GazVolumeChange).ToArray();

            SetValues(cell, values);
        }
    }
}