using System;
using System.Linq;
using System.Windows;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Model;
using Telerik.Windows.Diagrams.Core;
using ManipulationPointService = GazRouter.Flobus.Services.ManipulationPointService;
using SelectionService = GazRouter.Flobus.Services.SelectionService;
using ServiceLocator = GazRouter.Flobus.Services.ServiceLocator;
using System.Collections.Generic;

namespace GazRouter.Flobus.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class PipelineManipulationTool : ToolBase, IMouseListener
    {
        public const string ToolName = "PipelineManipulation Tool";
        private ServiceLocator _serviceLocator;

        public PipelineManipulationTool() : base(ToolName)
        {
        }

        public IPipelineEditPoint ActiveManipulationPoint { get; set; }

        private ManipulationPointService ManipulationService => _serviceLocator.ManipulationPointService;

        private SelectionService SelectionService => _serviceLocator.SelectionService;

        public bool MouseDown(PointerArgs e)
        {
            if (!IsActive)
            {
                return false;
            }

            if (ActiveManipulationPoint != null)
            {
                ManipulationService.InitializeManipulation(ActiveManipulationPoint);
            }

            var p = e.TransformedPoint;
            if (ToolService.IsControlDown)
            {
                if (!_serviceLocator.SelectionService.SelectedPipelines.Any())
                {
                    return true;
                }
                var pipeline = SelectionService.SelectedPipelines.First();
                var relativePoint = new Point(p.X - 5, p.Y - 5);
                var rect = new Rect(relativePoint, new Size(10, 10));
                if (pipeline.Points.Any(point => rect.Contains(point.Position)))
                {
                    RemovePoint(pipeline, p);
                }
                else
                {
                    AddPoint(pipeline, p);
                }
            }
            Graph.IsMouseCaptured = true;

            return true;
        }

        public bool MouseMove(PointerArgs e)
        {
            if (!IsActive || ActiveManipulationPoint?.Pipeline == null)
            {
                return false;
            }

            if (ManipulationService.IsManipulating || StartManipulate(e.TransformedPoint))
            {
                if (false)
                {
                    //Graph.IsSnapToGridEnabled
                    var p = _serviceLocator.SnappingService.SnapPoint(e.TransformedPoint);
                    ManipulationService.Manipulate(p);
                }
                else
                {
                    Point p = ActiveManipulationPoint.PipelinePoint.Align(e.TransformedPoint);
                    ManipulationService.Manipulate(e.TransformedPoint);
                }
            }

            return true;
        }

        public override void Initialize(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public bool MouseUp(PointerArgs e)
        {
            if (!IsActive)
            {
                return false;
            }

            if (ManipulationService.IsManipulating)
            {
                if (ManipulationService.CompleteManipulation(e.TransformedPoint))
                {
                    var pipeline = ActiveManipulationPoint.Pipeline;
                    switch (ActiveManipulationPoint.Type)
                    {
                        case PointType.First:
                            pipeline.StartPoint = e.TransformedPoint;
                            pipeline.Update();
                            break;

                        case PointType.Last:
                            pipeline.EndPoint = e.TransformedPoint;
                            pipeline.Update();
                            break;

                        case PointType.Intermediate:
                            ActiveManipulationPoint.PipelinePoint.Position = e.TransformedPoint;
                            pipeline.Update();
                            break;
                        case PointType.Infra:
                            var pos = ManipulationService.GetNearestProjectionPoint(e.TransformedPoint, ActiveManipulationPoint.PipelinePoint, pipeline);
                            ActiveManipulationPoint.Pipeline.MovePoint(ActiveManipulationPoint.PipelinePoint, pos);
                            ActiveManipulationPoint.PipelinePoint.Position = pos;
                            ActiveManipulationPoint.Pipeline.RecalculateIntermediateKm(ActiveManipulationPoint.PipelinePoint);
                            pipeline.Update();

                            break;
                    }
                }
            }

            CompleteToolAction();
            ToolService.ActivatePrimaryTool();
            return false;
        }

        private void CompleteToolAction()
        {
            ActiveManipulationPoint = null;
            ManipulationService.CleanManipulation();
        }

        private void RemovePoint(IPipelineWidget pipeline, Point p)
        {
            throw new NotImplementedException();
/*
            if (!pipeline.Points.Any()) return;

            var index = -1;
            for (int i = 0; i < pipeline.Points.Count; i++)
            {
                if (p.AroundPoint(pipeline.Points[i].Position, 5))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1) return;

            var oldPoints = pipeline.Points.Clone();
            var newPoints = pipeline.Points.Clone();

            newPoints.RemoveAt(index);

            pipeline.SetPipelinePoints(newPoints);
            pipeline.Update();
*/
        }

        private void AddPoint(IPipelineWidget pipeline, Point point)
        {
            pipeline.AddPoint(point);
        }

        private bool StartManipulate(Point currentPoint)
        {
            return ManipulationService.StartManipulate(currentPoint);
        }
    }
}