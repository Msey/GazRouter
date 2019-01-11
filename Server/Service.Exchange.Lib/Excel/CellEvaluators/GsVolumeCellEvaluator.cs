using System;
using System.Linq;
using System.Text.RegularExpressions;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataLoadMonitoring;
using GazRouter.DTO.DataLoadMonitoring;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ExcelReports;
using Utils.Extensions;

namespace GazRouter.Service.Exchange.Lib.Excel.CellEvaluators
{
    public class GsVolumeCellEvaluator : CellEvaluator
    {
        public GsVolumeCellEvaluator(DateTime timeStamp, PeriodType periodType, ExecutionContext ctx) : base(timeStamp,
            periodType, ctx)
        {
        }

        protected override Regex Regex => new Regex(
            @"#CELL_GS#([^#]*)#([^#]*)#([^#]*)#(@TIMESTAMP\s*(([+-])([^+-]+))?)");

        public void Parse(string input, out Guid pipeLineId, out int kms, out int kme, out DateTime timeStamp)
        {
            var m = Regex.Match(input);
            var guidCapture = m.Groups[1].Value;
            pipeLineId = new Guid(guidCapture).Convert();
            kms = Convert.ToInt32(m.Groups[2].Value);
            kme = Convert.ToInt32(m.Groups[3].Value);
            timeStamp = GetCalculatedTimeStamp(m.Groups[5].Value);
        }
        public override CellValue GetValue(string rawValue)
        {
            DateTime timeStamp;
            Guid pipeLineId;
            int kms, kme;
            Parse(rawValue, out pipeLineId, out kms, out kme, out timeStamp);
            var @params = new GasSupplySumParameterSet
            {
                KeyDate = timeStamp,
                CountDays = 0,
                KmBegin = kms,
                KmEnd = kme,
                PipelineId = pipeLineId,
                PeriodType = PeriodType
            };

            return new GetSumGasVolumeByEnterprise(Context).Execute(@params).Select(gs => gs.GazVolume)
                .Where(v => v.HasValue)
                .Select(v => new CellValue {Number = (double) v, ValueType = CellValueType.Number}).FirstOrDefault();
        }
    }
}