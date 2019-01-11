using System.Windows;
using Telerik.Windows.Controls;
namespace GazRouter.Common.Ui.Styles
{
    public class PermissionCellStyle : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var permission = (byte)((RegisterItem)item).Permission;
            switch (permission)
            {
                case 0: return DeniedStyle;
                case 1: return ReadStyle;
                case 2: return WriteStyle;
                default: return null;
            }
        }
        public Style DeniedStyle { get; set; }
        public Style ReadStyle { get; set; }
        public Style WriteStyle { get; set; }
    }
}
