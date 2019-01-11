using GazRouter.Flobus.Interfaces;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Utilites
{
    public sealed class PolylineSpecification
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public bool DrawBridges { get; set; }
        public CrossingsData Crossings { get; set; }
        public IList<Point> Points { get; set; }
        public List<List<Point>> SplitPoints { get; set; }
        [NotNull]
        public IList<PointSegment> Gaps { get; set; }
    }
}