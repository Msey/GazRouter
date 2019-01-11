using System;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Utilites
{
    public class DistanceToPointComparer : IComparer<Point>
    {
        public DistanceToPointComparer(Point referencePoint)
        {
            ReferencePoint = referencePoint;
        }

        public int Compare(Point x, Point y)
        {
            if (x == y) return 0;
            var distance1 = ReferencePoint.Delta(x).LengthSquared;
            var distance2 = ReferencePoint.Delta(y).LengthSquared;
            if (Math.Abs(distance1 - distance2) < Telerik.Windows.Diagrams.Core.Utils.Epsilon) return 0;
            return distance1 < distance2 ? -1 : 1;
        }

        private Point ReferencePoint { get; }
    }
}