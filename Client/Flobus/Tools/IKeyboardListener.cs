using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Tools
{
    public interface IKeyboardListener
    {
        bool KeyDown(KeyArgs key);
        bool KeyUp(KeyArgs key);
    }
}