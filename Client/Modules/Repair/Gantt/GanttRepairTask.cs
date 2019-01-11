using System;
using Telerik.Windows.Controls.GanttView;

namespace GazRouter.Repair.Gantt
{
    public class GanttRepairTask : GanttTask
    {
        public GanttRepairTask(DateTime start, DateTime end, string title)
            : base(start, end, title)
        {
            RepairItem = null;
        }

        public GanttRepairTask(RepairItem repair)
            : base(repair.StartDatePlan, repair.EndDatePlan, repair.ObjectName.Replace(Environment.NewLine, " "))
        {
            RepairItem = repair;

        }

        public RepairItem RepairItem { get; }
    }

}
