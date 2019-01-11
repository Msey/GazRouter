using System.Windows;
using GazRouter.DTO.GasCosts;
using Telerik.Windows.Controls;
namespace GazRouter.Modes.GasCosts2.Styles
{
    public class StatesRegularStyle : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (!(item is StateItem)) return null;
            //
            var stateItem = (StateItem) item;
            if (stateItem.CostType == CostType.None)
            {
                return DefaultStyle;
            }
            if (stateItem.Regular == 1)
            {
                return RegularStyle;
            }
            else
            {
                return NonRegularBStyle;
            }
        }
        public Style DefaultStyle { get; set; }
        public Style RegularStyle { get; set; }
        public Style NonRegularBStyle { get; set; }
    }
}
