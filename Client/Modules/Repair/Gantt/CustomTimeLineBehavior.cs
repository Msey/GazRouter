using System.Collections.Generic;
using System.Windows.Media;
using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Controls.Scheduling;
using Telerik.Windows.Core;

namespace GazRouter.Repair.Gantt
{
    public class CustomTimeLineBehavior: DefaultGanttTimeLineVisualizationBehavior
    {
        protected override IEnumerable<IEventInfo> GetEventInfos(TimeLineVisualizationState state, HierarchicalItem hierarchicalItem)
        {
            foreach (var eventInfo in base.GetEventInfos(state, hierarchicalItem))
            {
                yield return eventInfo;
            }

            var task = (GanttRepairTask) hierarchicalItem.SourceItem;

            if (!task.IsSummary && !task.RepairItem.IsExternalCondition)
            {
                // Ограничители даты поставки МТР
                if (task.RepairItem.Dto.PartsDeliveryDate.DayOfYear > 1)
                {
                    var range = new Range<long>(
                        task.RepairItem.Dto.PartsDeliveryDate.Ticks,
                        task.RepairItem.Dto.PartsDeliveryDate.Ticks);

                    if (range.IntersectsWith(state.VisibleTimeRange))
                        yield return new TimeLineLimiterEventInfo(range, hierarchicalItem.Index, Color.FromArgb(0xff, 0xDC, 0x14, 0x3C));
                }

                // Ограничители комплекса
                if (task.RepairItem.ComplexId > 0)
                {
                    var startDate = task.RepairItem.Dto.PartsDeliveryDate != task.RepairItem.Dto.Complex.StartDate
                        ? task.RepairItem.Dto.Complex.StartDate
                        : task.RepairItem.Dto.Complex.StartDate.AddHours(2);
                    var startRange = new Range<long>(startDate.Ticks, startDate.Ticks);

                    if (startRange.IntersectsWith(state.VisibleTimeRange))
                        yield return new TimeLineLimiterEventInfo(startRange, hierarchicalItem.Index, Color.FromArgb(0xFF, 0xFF, 0x8C, 0x00));


                    var endDate = task.RepairItem.Dto.PartsDeliveryDate != task.RepairItem.Dto.Complex.EndDate
                        ? task.RepairItem.Dto.Complex.EndDate
                        : task.RepairItem.Dto.Complex.EndDate.AddHours(2);
                    var endRange = new Range<long>(endDate.Ticks, endDate.Ticks);

                    if (endRange.IntersectsWith(state.VisibleTimeRange))
                        yield return new TimeLineLimiterEventInfo(endRange, hierarchicalItem.Index, Color.FromArgb(0xFF, 0xFF, 0x8C, 0x0));


                    // Если если даты проведения работ не соответсвуют датам проведения комплекса
                    if (task.RepairItem.HasComplexError)
                    {
                        var warningRange = new Range<long>(task.RepairItem.StartDatePlan.Ticks,
                            task.RepairItem.StartDatePlan.Ticks);
                        if (warningRange.IntersectsWith(state.VisibleTimeRange))
                            yield return new TimeLineWarningEventInfo(warningRange, hierarchicalItem.Index, task.RepairItem.ComplexErrorString);
                    }
                }

                
            }

        }
    }


}
