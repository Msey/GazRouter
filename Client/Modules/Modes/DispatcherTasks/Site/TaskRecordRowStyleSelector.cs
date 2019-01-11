using System.Windows;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.DispatcherTasks.Site
{
    public class TaskRecordRowStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var task = item as TaskRecordItem;

            if (task != null && !task.IsAck) return NonAckStyle;

            return NormalStyle;
        }

        public Style NonAckStyle { get; set; }

        public Style NormalStyle { get; set; }

    }
}
