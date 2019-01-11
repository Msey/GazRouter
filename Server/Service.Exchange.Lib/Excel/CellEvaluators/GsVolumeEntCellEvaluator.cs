using System;
using System.Linq;
using System.Text.RegularExpressions;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataLoadMonitoring;
using GazRouter.DTO.DataLoadMonitoring;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ExcelReports;

namespace GazRouter.Service.Exchange.Lib.Excel.CellEvaluators
{
    public class GsVolumeEntCellEvaluator : CellEvaluator
    {
        public GsVolumeEntCellEvaluator(DateTime timeStamp, PeriodType periodType, ExecutionContext ctx) : base(
            timeStamp, periodType, ctx)
        {
        }

        protected override Regex Regex
            => new Regex(@"#CELL_GS_ENT#(@TIMESTAMP\s*(([+-])([^+-]+))?)");

        private void Parse(string input, out DateTime timeStamp)
        {
            var m = Regex.Match(input);
            timeStamp = GetCalculatedTimeStamp(m.Groups[2].Value);
        }

        public override CellValue GetValue(string rawValue)
        {
            DateTime timeStamp;
            Parse(rawValue, out timeStamp);
            var @params = new GasSupplySumParameterSet
            {
                KeyDate = timeStamp,
                CountDays = 0,
                PeriodType = PeriodType
            };

            return new GetSumGasVolumeByEnterprise(Context).Execute(@params).Select(gs => gs.GazVolume)
                .Where(v => v.HasValue)
                .Select(v => new CellValue {ValueType = CellValueType.Number, Number = (double) v}).FirstOrDefault();
        }

    }
}