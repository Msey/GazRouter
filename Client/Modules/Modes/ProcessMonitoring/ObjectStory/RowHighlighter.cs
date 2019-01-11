using System.Windows;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.ProcessMonitoring.ObjectStory
{
    public class RowHighlighter : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var i = item as StoryItemBase;
            if (i != null && i.IsSelected)
                return HighlightStyle;

            return NormalStyle;
        }

        public Style HighlightStyle { get; set; }
        public Style NormalStyle { get; set; }
    }
}