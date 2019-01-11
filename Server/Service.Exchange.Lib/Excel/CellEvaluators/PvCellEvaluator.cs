using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GazRouter.DAL.Core;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ExcelReports;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;

namespace GazRouter.Service.Exchange.Lib.Excel.CellEvaluators
{
    public class PvCellEvaluator : CellEvaluator
    {
        public PvCellEvaluator(DateTime timeStamp, PeriodType periodType, ExecutionContext ctx) : base(timeStamp,
            periodType, ctx)
        {
        }

        protected override Regex Regex => new Regex(@"#CELL_PV#(.*)#(\d*)#(\d*)#(@TIMESTAMP\s*(([+-])([^+-]+))?)");

        public void Parse(string input, out Guid entityId, out PropertyType pt, out PeriodType period,
            out DateTime timeStamp)
        {
            var m = Regex.Match(input);
            entityId = new Guid(m.Groups[1].Value).Convert();
            pt = (PropertyType) Convert.ToInt32(m.Groups[2].Value);
            period = (PeriodType) Convert.ToInt32(m.Groups[3].Value);
            timeStamp = GetCalculatedTimeStamp(m.Groups[5].Value);
        }
        public override CellValue GetValue(string rawValue)
        {
            DateTime timeStamp;
            Guid entityId;
            PropertyType pt;
            PeriodType period;
            Parse(rawValue, out entityId, out pt, out period, out timeStamp);
            var param = new GetEntityPropertyValueListParameterSet
            {
                StartDate = timeStamp.Add(-period.ToTimeSpan()),
                EndDate = timeStamp,
                PeriodType = period,
                EntityIdList = new List<Guid> {entityId}
            };
            var dict = new GetEntityPropertyValueListQuery(Context).Execute(param);
            if (dict.ContainsKey(entityId) && dict[entityId].ContainsKey(pt))
            {
                var propertyValueDTO = dict[entityId][pt]
                    .Where(p => p.Date == timeStamp)
                    .FirstOrDefault(v => v.PeriodTypeId == period);
                return ExchangeHelper.GetCellValue(propertyValueDTO);
            }
            return null;
        }
    }
}