using System;
using Telerik.Windows.Controls.GanttView;

namespace GazRouter.Repair.Plan.Gantt
{
    public class RepairTask : GanttTask
    {
        
        public RepairTask(Repair repair)
            : base(repair.Dto.StartDate, repair.Dto.EndDate, repair.ObjectName.Replace(Environment.NewLine, " "))
        {
            RepairItem = repair;
        }

        public Repair RepairItem { get; }
    }

    public class GroupTask : GanttTask
    {
        public GroupTask(DateTime start, DateTime end, string title)
            : base(start, end, title) { }
    }

}
