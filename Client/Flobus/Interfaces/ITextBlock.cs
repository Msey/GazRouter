using System.Windows;

namespace GazRouter.Flobus.Interfaces
{
    public interface ITextBlock
    {
        Point Position { get; set; }

        string Text { get; set; }

        int TextAngle { get; set; }
    }
}