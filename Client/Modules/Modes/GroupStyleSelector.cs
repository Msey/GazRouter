using System.Windows;
using GazRouter.Modes.GasCosts.Summary;
using Telerik.Windows.Controls;

namespace GazRouter.Modes
{
    public class GroupStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is GasCostsSummaryGroup)
                return GroupStyle;
            return base.SelectStyle(item, container);
        }

        public Style GroupStyle { get; set; }
      
    }
}