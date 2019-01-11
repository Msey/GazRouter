using System.Windows;
using Telerik.Windows.Controls;

namespace GazRouter.Flobus.FloScheme
{
    public interface ISupportContextMenu
    {
        void FillMenu(RadContextMenu menu, Point mousePosition, Schema schema);
    }
}