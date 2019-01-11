using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Controls.Scheduling;

namespace GazRouter.Repair.Plan.Gantt
{
	public class CustomDragDependenciesBehavior : GanttDragDependenciesBehavior
    {
        protected override bool CanStartLink(SchedulingLinkState state)
        {
            return false;
        }

        protected override bool CanLink(SchedulingLinkState state)
        {
            return false;
        }
    }
    


}
