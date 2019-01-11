using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;
using Telerik.Windows.Diagrams.Core;
using GazRouter.Flobus.Interfaces;

namespace GazRouter.Flobus.Utilites
{
    public class GeometryExtensions
    {
        public static Geometry CreateLineGeometry([NotNull] PolylineSpecification specs)
        {
            if (specs == null) throw new ArgumentNullException(nameof(specs));
            if (specs.StartPoint == specs.EndPoint && specs.Points == null) return null;

            var geometry = new PathGeometry();

            if (specs.DrawBridges)
            {
                if (specs.Crossings.SegmentCrossings[0].Count > 0)
                    specs.Crossings.SegmentCrossings[0].RemoveAt(0);
                specs.Crossings.SegmentCrossings[0].Insert(0, specs.StartPoint);
                var l = specs.Crossings.SegmentCrossings[specs.Crossings.SegmentCrossings.Count - 1];
                if (l.Any())
                    l.Remove(l.Last());
                l.Add(specs.EndPoint);
            }

            if (specs.Gaps.Count > 0)
            {
                List<PointSegment> segments = new List<PointSegment>();

                segments.Add(new PointSegment ( specs.StartPoint, specs.Gaps.First().Start));
                for (int i = 1; i < specs.Gaps.Count; i++)
                    segments.Add(new PointSegment (specs.Gaps[i-1].End, specs.Gaps[i].Start));
                segments.Add(new PointSegment ( specs.Gaps.Last().End, specs.EndPoint ));
                int index = 0;
                foreach (var segment in segments)
                {
                    var flags = GetPolylineLineFigures(new PolylineSpecification()
                    {
                        StartPoint = segment.Start,
                        EndPoint = segment.End,
                        Points = specs.SplitPoints[index],
                        Gaps = specs.Gaps,
                        DrawBridges = specs.DrawBridges,
                        Crossings = specs.Crossings,
                    });
                    geometry.Figures.AddRange(flags);
                    index++;
                }
            }
            else
            {
                var flags = GetPolylineLineFigures(new PolylineSpecification()
                {
                    StartPoint = specs.StartPoint,
                    EndPoint = specs.EndPoint,
                    Points = specs.Points,
                    Gaps = specs.Gaps,
                    DrawBridges = specs.DrawBridges,
                    Crossings = specs.Crossings,
                });
                geometry.Figures.AddRange(flags);
            }


            return geometry;
        }

        private static IEnumerable<PathFigure> GetPolylineLineFigures(PolylineSpecification specs)
        {
            var startPointResult = specs.StartPoint;
            var endPointResult = specs.EndPoint;
            var figures = new List<PathFigure>();
            var figure = new PathFigure() {IsClosed = false, IsFilled = false, StartPoint = startPointResult};
            figures.Add(figure);

            if (specs.DrawBridges)
            {
                if (specs.Crossings == null)
                    throw new ArgumentException("Crossing нужны чтобы посроить пересечения", nameof(specs));
                var crad = DiagramConstants.CrossingRadius;
                var crossings = specs.Crossings;
                var localPipelinePoints = specs.Points.ToList();
                localPipelinePoints.Insert(0, startPointResult);
                localPipelinePoints.Add(endPointResult);
                var segmentCount = specs.Points.Count + 1;
                for (int segmentIndex = 0; segmentIndex < segmentCount; ++segmentIndex)
                {
                    var segmentCrossing = crossings.SegmentCrossings[segmentIndex];

                    for (int segmentPositionIndex = 0;
                        segmentPositionIndex < segmentCrossing.Count - 1;
                        ++segmentPositionIndex)
                    {
                        var segmentStartPoint = segmentCrossing[segmentPositionIndex];
                        var segmentEndPoint = segmentCrossing[segmentPositionIndex + 1];


                        if (segmentPositionIndex%2 == 0)
                        {
                            figure.Segments.Add(new LineSegment() {Point = segmentEndPoint});
                        }
                        else
                        {
                            var radv = segmentEndPoint.Delta(segmentStartPoint)/
                                       segmentStartPoint.Distance(segmentEndPoint)*crad;

                            var radvPerpLeft = radv.Perpendicular();
                            var radvPerpUpper = radvPerpLeft.Y > 0 ? -radvPerpLeft : radvPerpLeft;
                            var dir = radvPerpLeft.Y > 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
                            var arcSize = new Size(crad, crad);
                            figure.Segments.Add(new ArcSegment()
                            {
                                Point = segmentStartPoint + radv + radvPerpUpper,
                                Size = arcSize,
                                SweepDirection = dir
                            });
                            figure.Segments.Add(new LineSegment() {Point = segmentEndPoint - radv + radvPerpUpper});
                            figure.Segments.Add(new ArcSegment()
                            {
                                Point = segmentStartPoint,
                                Size = arcSize,
                                SweepDirection = dir
                            });
                        }
                    }
                }
            }
            else
            {
                foreach (var point in specs.Points)
                    figure.Segments.Add(new LineSegment {Point = point});
                figure.Segments.Add(new LineSegment() {Point = specs.EndPoint});
            }
            return figures;
        }

        public static Geometry GetPathGeometry([NotNull] string abbreviatedGeometry)
        {
            if (abbreviatedGeometry == null) throw new ArgumentNullException(nameof(abbreviatedGeometry));

            var geometry =new PathGeometry() {Figures = new PathFigureCollection()};
            var index = 0;
            while ((index < abbreviatedGeometry.Length) && char.IsWhiteSpace(abbreviatedGeometry,index))
            {
                index++;
            }
            if (index < abbreviatedGeometry.Length && abbreviatedGeometry[index] == 'F')
            {
                index++;
                while ((index < abbreviatedGeometry.Length) && char.IsWhiteSpace(abbreviatedGeometry, index)) index++;
                if ((index == abbreviatedGeometry.Length)
                    || ((abbreviatedGeometry[index] !='0') && (abbreviatedGeometry[index] != '1'))) throw new FormatException();

                geometry.FillRule = (abbreviatedGeometry[index] == '0') ? FillRule.EvenOdd : FillRule.Nonzero;
                index++;
            }
            new InternalGeometryParser(geometry).Parse(abbreviatedGeometry, index);
            return geometry;

        }
    }
}