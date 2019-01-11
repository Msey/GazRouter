using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Rendering.Virtualization;

namespace GazRouter.Repair.Plan.Gantt
{
    public class CustomTimeLineContainerSelector: DefaultTimeLineContainerSelector
    {
        private static readonly ContainerTypeIdentifier limiterEventInfoContainerType =
            ContainerTypeIdentifier.FromType<TimeLineLimiterContainer>();

        private static readonly ContainerTypeIdentifier warningEventInfoContainerType =
            ContainerTypeIdentifier.FromType<TimeLineWarningContainer>();


        public override ContainerTypeIdentifier GetContainerType(object item)
        {
            if (item is TimeLineLimiterEventInfo)
                return limiterEventInfoContainerType;

            if (item is TimeLineWarningEventInfo)
                return warningEventInfoContainerType;

            return base.GetContainerType(item);
        }
    }


}
