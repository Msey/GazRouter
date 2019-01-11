using System.Windows;
using Telerik.Windows.Controls;

namespace GazRouter.ObjectModel
{
    public class GazShapeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return ValveTemplate;
        }

        public DataTemplate ValveTemplate { get; set; }
    }
}