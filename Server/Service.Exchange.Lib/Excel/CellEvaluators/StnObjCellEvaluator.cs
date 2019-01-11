using System;
using System.Linq;
using System.Text.RegularExpressions;
using GazRouter.DAL.Core;
using GazRouter.DAL.GasCosts;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ExcelReports;
using GazRouter.DTO.GasCosts;
using Utils.Extensions;

namespace GazRouter.Service.Exchange.Lib.Excel.CellEvaluators
{
    public class StnObjCellEvaluator : CellEvaluator
    {
        public StnObjCellEvaluator(DateTime timeStamp, PeriodType periodType, ExecutionContext ctx) : base(timeStamp,
            periodType, ctx)
        {
        }

        protected override Regex Regex
            => new Regex(@"#CELL_STN_OBJ#([^#]*)#([^#]*)#([^#]*)#([^#]*)#([^#]*)#(@TIMESTAMP\s*(([+-])([^+-]+))?)");

        public void Parse(string input, out Guid entityId, out CostType costType, out Target targetId,
            out TimeSpan span, out DateTime timeStamp)
        {
            var m = Regex.Match(input);
            var guid = m.Groups[1].Value;
            entityId = new Guid(guid).Convert();
            costType = (CostType) Convert.ToInt32(m.Groups[2].Value);
            targetId = (Target) Convert.ToInt32(m.Groups[3].Value);

            var hdm = Convert.ToString(m.Groups[4].Value);
            var shift = Convert.ToInt32(m.Groups[5].Value);
            span = hdm == "H"
                ? TimeSpan.FromHours(shift)
                : hdm == "D"
                    ? TimeSpan.FromDays(shift)
                    : TimeSpan.FromDays(30 * shift);

            timeStamp = GetCalculatedTimeStamp(m.Groups[7].Value);
        }


        public override CellValue GetValue(string rawValue)
        {
            DateTime timeStamp;
            Guid entityId;
            CostType costType;
            Target targetId;
            TimeSpan span;
            Parse(rawValue, out entityId, out costType, out targetId, out span, out timeStamp);
            var @params = new GetGasCostSumVolumeParameterSet
            {
                Target = targetId,
                CostType = costType,
                BeginDate = timeStamp.Date.Add(-span),
                EndDate = timeStamp.Date,
                EntityId = entityId
            };
            return new GetGasCostSumVolumeQuery(Context).Execute(@params).Select(gc => gc.MeasuredVolume)
                .Where(v => v.HasValue)
                .Select(v => new CellValue {Number = (double) v, ValueType = CellValueType.Number}).FirstOrDefault();
        }

    }
}