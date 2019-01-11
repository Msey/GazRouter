using System;
using System.Windows;
using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Controls.Scheduling;

namespace GazRouter.Repair.Plan.Gantt
{
    public class CustomDragDropBehavior : GanttDragDropBehavior
    {
        public static readonly DependencyProperty ChangesAllowedProperty =
            DependencyProperty.Register("ChangesAllowed", typeof (bool), typeof (CustomDragDropBehavior),
                new PropertyMetadata(true));

        public bool ChangesAllowed
        {
            get { return (bool) GetValue(ChangesAllowedProperty); }
            set { SetValue(ChangesAllowedProperty, value); }
        }

        protected override bool CanStartDrag(SchedulingDragDropState state)
        {
            if (!base.CanStartDrag(state))
            {
                return false;
            }

            var task = state.DraggedItem as GanttTask;

            return ChangesAllowed && !(task.IsMilestone || task.IsSummary);
        }

        protected override bool CanDrop(SchedulingDragDropState state)
        {
            if (base.CanDrop(state))
            {
                var length = state.DestinationSlot.End - state.DestinationSlot.Start;
                var start = state.DestinationSlot.Start.Date;

                var task = state.DraggedItem as RepairTask;
                if (task == null) return false;
                
                // Новая дата начала работ должна быть не меньше даты поставки МТР
                if (start < task.RepairItem.Dto.PartsDeliveryDate)
                {
                    start = task.RepairItem.Dto.PartsDeliveryDate;
                }

                // Проверки для комплексов ПАО "Газпром"
                if (task.RepairItem.Dto.Complex.Id > 0 && !task.RepairItem.Dto.Complex.IsLocal)
                {
                    // Новая дата начала должна быть не меньше даты начала комплекса
                    if (start < task.RepairItem.Dto.Complex.StartDate)
                    {
                        start = task.RepairItem.Dto.Complex.StartDate;
                    }

                    // По продолжительности работа не должна вылезать за дату окончания комплекса
                    if (start + length > task.RepairItem.Dto.Complex.EndDate)
                    {
                        start = task.RepairItem.Dto.Complex.EndDate -
                                TimeSpan.FromDays(Math.Ceiling(length.TotalDays));
                    }
                }
                
                var end = start + TimeSpan.FromDays(Math.Ceiling(length.TotalDays));

                state.DestinationSlot.Start = start;
                state.DestinationSlot.End = end;

                return true;
            }
            return false;
        }
    }
}