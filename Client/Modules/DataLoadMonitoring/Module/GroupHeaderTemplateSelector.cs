using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace GazRouter.DataLoadMonitoring.Module
{
    public class GroupHeaderTemplateSelector : ScheduleViewDataTemplateSelector
    {
        public DataTemplate HorizontalTemplate { get; set; }
        public DataTemplate VerticalTemplate { get; set; }

        public DataTemplate HorizontalBottomLevelTemplate { get; set; }
        public DataTemplate VerticalBottomLevelTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container, ViewDefinitionBase activeViewDeifinition)
        {
            var groupHeader = container as GroupHeader;
            if (groupHeader != null)
            {
                if (groupHeader.IsBottomLevel)
                {
                    return activeViewDeifinition.GetOrientation() == Orientation.Horizontal ? HorizontalBottomLevelTemplate : VerticalBottomLevelTemplate;
                }

                return activeViewDeifinition.GetOrientation() == Orientation.Horizontal ? HorizontalTemplate : VerticalTemplate;
            }

            return base.SelectTemplate(item, container, activeViewDeifinition);
        }
    }
}
