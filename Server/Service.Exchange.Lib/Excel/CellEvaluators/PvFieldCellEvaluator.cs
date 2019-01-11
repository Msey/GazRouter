using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using GazRouter.DAL.Core;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;

namespace GazRouter.Service.Exchange.Lib.Excel.CellEvaluators
{
    public class PvFieldCellEvaluator : CellEvaluator
    {
        protected override Regex Regex
            => new Regex(@"#FIELD_PV#(.*)#(.*)#(.*)#([DHM])#(.*)#(@TIMESTAMP\s*(([+-])([^+-]+))?)");

        public PvFieldCellEvaluator(DateTime timeStamp, PeriodType periodType, ExecutionContext ctx) : base(timeStamp, periodType, ctx)
        {
        }

        public void Parse(string input, out Guid entityId, out PropertyType pt, out PeriodType period,
            out TimeSpan span,
            out DateTime timeStamp)
        {
            var m = Regex.Match(input);
            entityId = new Guid(m.Groups[1].Value).Convert();
            pt = (PropertyType) Convert.ToInt32(m.Groups[2].Value);
            period = (PeriodType) Convert.ToInt32(m.Groups[3].Value);
            timeStamp = GetCalculatedTimeStamp(m.Groups[7].Value);
            var hdm = Convert.ToString(m.Groups[4].Value);
            var shift = Convert.ToInt32(m.Groups[5].Value);
            span = hdm == "H"
                ? TimeSpan.FromHours(shift)
                : hdm == "D" ? TimeSpan.FromDays(shift) : TimeSpan.FromDays(30*shift);
        }

        protected override void _Process(IXLCell cell)
        {
            DateTime timeStamp;
            Guid entityId;
            PropertyType pt;
            PeriodType period;
            var input = cell.GetString();
            TimeSpan span;
            Parse(input, out entityId, out pt, out period, out span, out timeStamp);
            var @param = new GetEntityPropertyValueListParameterSet
            {
                StartDate = timeStamp.Add(-period.ToTimeSpan()),
                EndDate = timeStamp,
                PeriodType = period,
                EntityIdList = new List<Guid> {entityId}
            };
            var dict = new GetEntityPropertyValueListQuery(Context).Execute(@param);

            if (dict.ContainsKey(entityId) && dict[entityId].ContainsKey(pt))
            {
                var values = dict?[entityId][pt]
                    .Where(prop => prop.Date <= timeStamp && timeStamp - span < prop.Date)
                    .Where(prop => prop.PeriodTypeId == period)
                    .Select(ExchangeHelper.GetValue).ToArray();
                SetValues(cell, values);
            }
            else
            {
                SetValue(cell, null);
            }
        }
    }
}