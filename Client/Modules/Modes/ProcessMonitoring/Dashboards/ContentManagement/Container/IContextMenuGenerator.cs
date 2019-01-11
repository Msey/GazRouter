using System.Windows.Input;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container
{
    public interface IContextMenuGenerator
    {
        void FillMenu(RadContextMenu menu, MouseButtonEventArgs e);
    }
}