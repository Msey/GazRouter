using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Model;
using Telerik.Windows.Diagrams.Core;
using IGraph = GazRouter.Flobus.Model.IGraph;
using System;

namespace GazRouter.Flobus.Services
{
    public class ManipulationPointService : SchemaServiceBase
    {
        private IPipelineWidget _pipeline;
        private Dictionary<double, Point> _initialPoints;

        public ManipulationPointService(IGraph graph) : base(graph)
        {
        }

        public IPipelineEditPoint ManipulationPoint { get; set; }
        public bool IsManipulating { get; private set; }

        public void InitializeManipulation(IPipelineEditPoint manipulationPoint)
        {
            ManipulationPoint = manipulationPoint;

            _pipeline = manipulationPoint.Pipeline;

            //      _pointIndex = manipulationPoint.Pipeline.Points.F(manipulationPoint.PipelinePoint);
            _initialPoints = _pipeline.Points.Where(p => p.Type == PointType.Intermediate)
                .ToDictionary(p => p.Km, x => x.Position.Substract(_pipeline.Position));
        }

        public bool StartManipulate(Point currentPoint)
        {
            IsManipulating = true;
          ManipulationPoint.IsManipulating = true;
            return true;
        }

        public void CleanManipulation()
        {
            IsManipulating = false;

            if (ManipulationPoint == null)
            {
                return;
            }

            ManipulationPoint.IsManipulating = false;
            ManipulationPoint = null;
            _pipeline = null;
        }

        public void Manipulate(Point newPoint)
        {
            //  _pipeline.Position = newPoint;
            var pipeline = _pipeline;
            
            var points = _initialPoints;
            var end =
                pipeline.ManipulationPoints.First(x => x.Type == PointType.Last).Position.Substract(pipeline.Position);
            var start =
                pipeline.ManipulationPoints.First(x => x.Type == PointType.First).Position.Substract(pipeline.Position);
            
            var position = newPoint;

            switch (ManipulationPoint.Type)
            {
                case PointType.First:
                case PointType.Last:
                    ManipulationPoint.Position = newPoint;
                    pipeline.UpdateDefferedGeometry(start, end, points.Select(p => p.Value).ToArray());
                    break;
                case PointType.Intermediate:
                    {
                        points[ManipulationPoint.PipelinePoint.Km] = ManipulationPoint.Position.Substract(pipeline.Position);
                        ManipulationPoint.Position = position; //.Add(pipeline.Position);
                        _pipeline.UpdateDefferedGeometry(start, end, points.Select(p => p.Value).ToArray());
                        break;
                    }
                case PointType.Infra:
                    {
                        ManipulationPoint.Position = GetNearestProjectionPoint(newPoint, ManipulationPoint.PipelinePoint, pipeline);
                        pipeline.UpdateDefferedGeometry(start, end, points.Select(p => p.Value).ToArray());
                        break;
                    }
            }
        }

        public bool CompleteManipulation(Point currentPoint)
        {
            IsManipulating = false;
            return true;
        }

        internal void FindNeighbours(IPipelinePoint position, IPipelineWidget pipeline, out Point prevPoint,
            out Point nextPoint)
        {
            var manipulationPoints = pipeline.Points.ToList();
            var index = manipulationPoints.IndexOf(position);
            prevPoint = index != 0 ? manipulationPoints[index - 1].Position : pipeline.StartPoint;
            nextPoint = index != manipulationPoints.Count - 1
                ? manipulationPoints[index + 1].Position
                : pipeline.EndPoint;
        }
        internal Point GetNearestProjectionPoint(Point position, IPipelinePoint current, IPipelineWidget pipeline)
        {
            Dictionary<Point, double> NearestPointsList = new Dictionary<Point, double>();
            var PointList = pipeline.Points.ToList();
            var LimitList = new LinkedList<IPipelinePoint>(PointList.Where(p => p.Type == PointType.Infra || p.Type == PointType.First || p.Type == PointType.Last));
            IPipelinePoint[] NearestLimits = new IPipelinePoint[] { LimitList.Find(current).Previous.Value, LimitList.Find(current).Next.Value };
            for (int i = PointList.IndexOf(NearestLimits[0]); i < PointList.IndexOf(NearestLimits[1]); i++)
            {
                var startPosition = PointList[i].Position;
                var endPosition = PointList[i+1].Position;
                var projection = Telerik.Windows.Diagrams.Core.Utils.ProjectPointOnLine(position, startPosition, endPosition);
                Point projection_point;
                if (projection.IsXBetween(startPosition, endPosition) && projection.IsYBetween(startPosition, endPosition))
                    projection_point = projection;
                else projection_point = startPosition.Delta(position).Length < endPosition.Delta(position).Length ? startPosition : endPosition;
                if (!NearestPointsList.ContainsKey(projection_point)) NearestPointsList.Add(projection_point, position.Delta(projection_point).Length);
            }
            return NearestPointsList.OrderBy(p => p.Value).FirstOrDefault().Key;
        }
                
    }
}