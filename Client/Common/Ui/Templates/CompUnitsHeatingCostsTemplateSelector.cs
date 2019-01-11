using System;
using System.Windows;
using Telerik.Windows.Controls;
namespace GazRouter.Common.Ui.Templates
{
    public class CompUnitsHeatingCostsTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is InputData)) return null;
            //
            var data = (InputData)item;
            switch (data.Template)
            {
                case TemplateType.Formula2: return Formula2Template;
                case TemplateType.Formula3: return Formula3Template;
                case TemplateType.Default:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return Default;
        }
        public DataTemplate Default { get; set; }
        public DataTemplate Formula2Template { get; set; }
        public DataTemplate Formula3Template { get; set; }
    }
}
