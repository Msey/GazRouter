using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Interfaces
{
    public class Segment
    {
        public Segment(double kmBeginning, double kmEnd)
        {
            KmBeginning = kmBeginning;
            KmEnd = kmEnd;
        }
        public double KmBeginning { get; }
        public double KmEnd { get; }
    }
    public class PointSegment
    {
        public PointSegment(Point start, Point end)
        {
            Start = start;
            End = end;
        }
        public Point Start { get; }
        public Point End { get; }
        public PointSegment Substract(Point position) => new PointSegment(Start.Substract(position), End.Substract(position));
        public bool IsContains(Point point) => point.IsXBetween(Start, End) && point.IsYBetween(Start, End);
    }
}
