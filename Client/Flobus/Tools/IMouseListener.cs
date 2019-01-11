using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Tools
{
    public interface IMouseListener
    {
        bool MouseMove(PointerArgs e);
        bool MouseUp(PointerArgs e);
        bool MouseDown(PointerArgs e);
    }
}