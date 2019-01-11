using System;
using System.Windows;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Visuals
{
    public class GeometrySegment
    {
        private Point _end;


        public GeometrySegment(Point start, Point end)
        {
            if (start.X != end.X && start.Y != end.Y)
            {
                throw new ArgumentException();
            }
            Start = start;
            Orientation = Start.X == end.X ? Orientation.Vertical : Orientation.Horizontal;
            End = end;
        }

        public Point Start { get; }
        public Orientation Orientation { get; }

//        public  int Lenght { get; private set; }
        public Point End
        {
            get
            {
                return _end;
            }
            set
            {
                if ((Orientation == Orientation.Horizontal && Start.Y != value.Y) ||
                    (Orientation == Orientation.Vertical && Start.X != value.X))
                    throw new ArgumentException();
                _end = value;
            }
        }
    }
}