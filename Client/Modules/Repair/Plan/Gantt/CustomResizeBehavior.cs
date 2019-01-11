using System;
using System.Windows;
using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Controls.Scheduling;

namespace GazRouter.Repair.Plan.Gantt
{
    public class CustomResizeBehavior : SchedulingResizeBehavior
    {
        public static readonly DependencyProperty ChangesAllowedProperty =
            DependencyProperty.Register("ChangesAllowed", typeof (bool), typeof (CustomResizeBehavior),
                new PropertyMetadata(true));

        public bool ChangesAllowed
        {
            get { return (bool) GetValue(ChangesAllowedProperty); }
            set { SetValue(ChangesAllowedProperty, value); }
        }


        protected override bool CanStartResize(SchedulingResizeState state)
        {
            if (!base.CanStartResize(state))
                return false;

            var task = state.ResizedItem as GanttTask;
            return ChangesAllowed && !(task.IsMilestone || task.IsSummary);
        }

        protected override bool CanResize(SchedulingResizeState state)
        {
            if (base.CanResize(state))
            {
                var task = state.ResizedItem as RepairTask;
                if (task == null) return false;

                if (state.IsResizeFromEnd)
                {
                    // Проверки для комплексов ПАО "Газпром"
                    if (task.RepairItem.HasComplex
                        && !task.RepairItem.Dto.Complex.IsLocal
                        && state.DestinationSlot.End > task.RepairItem.Dto.Complex.EndDate)
                        state.DestinationSlot.End = task.RepairItem.Dto.Complex.EndDate;

                    var difference = state.DestinationSlot.End - state.DestinationSlot.End.Date;
                    state.DestinationSlot.End = state.DestinationSlot.End.Date +
                                                TimeSpan.FromDays(Math.Round(difference.TotalDays));
                }
                else
                {
                    // Проверки для комплексов ПАО "Газпром"
                    if (task.RepairItem.HasComplex
                        && !task.RepairItem.Dto.Complex.IsLocal
                        && state.DestinationSlot.Start < task.RepairItem.Dto.Complex.StartDate)
                        state.DestinationSlot.Start = task.RepairItem.Dto.Complex.StartDate;

                    var difference = state.DestinationSlot.Start - state.DestinationSlot.Start.Date;
                    state.DestinationSlot.Start = state.DestinationSlot.Start.Date +
                                                  TimeSpan.FromDays(Math.Round(difference.TotalDays));
                }
                return true;
            }
            return false;
        }
    }
}
