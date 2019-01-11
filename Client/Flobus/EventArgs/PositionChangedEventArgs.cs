using System.Windows;

namespace GazRouter.Flobus.EventArgs
{
    public class PositionChangedEventArgs : System.EventArgs
    {
        public Point OldPosition { get; private set; }
        public Point NewPosition { get; private set; }

        public PositionChangedEventArgs(Point oldPosition, Point newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
    }
}