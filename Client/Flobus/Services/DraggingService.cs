using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Visuals;
using Telerik.Windows.Diagrams.Core;
using IGraph = GazRouter.Flobus.Model.IGraph;
using PositionChangedEventArgs = GazRouter.Flobus.EventArgs.PositionChangedEventArgs;

namespace GazRouter.Flobus.Services
{
    /// <summary>
    ///     Сервис отвечающий за такснаие элемнов по схеме
    /// </summary>
    public class DraggingService : SchemaServiceBase
    {
        private Point _startDragPoint;
        private Point _currentDragPoint;
        private List<ISchemaItem> _draggingItems;

        public DraggingService(IGraph graph) : base(graph)
        {
        }

        public event EventHandler<PositionChangedEventArgs> Dragging;
        public event EventHandler<PositionChangedEventArgs> CompleteDragging;
        public event EventHandler<CancelingPositionChangedEventArgs> StartDragging;

        public bool IsDragging { get; private set; }

        public void InitializeDrag(Point point)
        {
            _currentDragPoint = _startDragPoint = point;
        }

        public bool StartDrag(IEnumerable<ISchemaItem> items, Point currentPoint)
        {
            var canDrag = OnStartDragging(currentPoint);
            if (canDrag)
            {
                _draggingItems = items.ToList();
                Graph.IsMouseCaptured = true;
            }
            return canDrag;
        }

        public bool CanDrag(Point newPoint)
        {
            if (IsDragging)
            {
                return true;
            }

            var deltaX = Math.Abs(_startDragPoint.X - newPoint.X);
            var deltaY = Math.Abs(_startDragPoint.Y - newPoint.Y);
            if (deltaX > DiagramConstants.StartDragDelta || deltaY > DiagramConstants.StartDragDelta)
            {
                return true;
            }
            return false;
        }

        public void Drag(Point newPoint)
        {
            var isDragValid = false;

            if (newPoint == _currentDragPoint || _draggingItems == null)
            {
                return;
            }

            var offset = newPoint.Subtract(_currentDragPoint);

            foreach (var draggingItem in _draggingItems.OrderBy(m => m is IPipelineWidget ? 1 : 0))
            {
                var pipeline = draggingItem as IPipelineWidget;
                if (pipeline != null)
                {
                    isDragValid |= DragPipeline(pipeline, offset);
                }
                else if (draggingItem is PipelineElementWidget)
                {
                    isDragValid |= DragPipelineElement((PipelineElementWidget) draggingItem, offset);
                }
                else
                {
                    draggingItem.Position = draggingItem.Position.Add(offset);
                    isDragValid = true;
                }
            }

            var correctedNewPoint = isDragValid ? _currentDragPoint.Add(offset) : _currentDragPoint;

            OnDragging(_currentDragPoint, correctedNewPoint, _draggingItems);
            _currentDragPoint = correctedNewPoint;
        }

        public void CompleteDrag()
        {
            IsDragging = false;

            OnCompleteDragging();

            _draggingItems = null;
        }

        private void OnDragging(Point oldValue, Point newValue, IEnumerable<ISchemaItem> items)
        {
            var args = new PositionChangedEventArgs(oldValue, newValue);
            Dragging?.Invoke(this, args);
        }

        private void OnCompleteDragging()
        {
            var e = new PositionChangedEventArgs(_startDragPoint, _currentDragPoint);
            CompleteDragging?.Invoke(this, e);
        }

        private bool DragPipeline(IPipelineWidget pipeline, Vector offset)
        {
            /*        var startPointCached = pipeline.StartPoint;
            var endPointCached = pipeline.EndPoint;

              pipeline.StartPoint = startPointCached.Add(offset);
            pipeline.EndPoint = endPointCached.Add(offset);*/
            pipeline.Move(offset);
            /*  foreach (var p in pipeline.Points)
            {
                p.Position = p.Position.Add(offset);
            }*/
            pipeline.Update();
            return true;
        }

        private bool DragPipelineElement(PipelineElementWidget pipelineElementWidget, Vector offset)
        {
            pipelineElementWidget.Pipeline.MoveAlong(pipelineElementWidget.PipelinePoint, offset);
            return true;
            /*       Point prevPoint, nextPoint;
            ActiveManipulationPoint.PipelinePoint.Position = e.TransformedPoint;
            ManipulationService.FindNeighbours(ActiveManipulationPoint, pipeline, out prevPoint, out nextPoint);

            ActiveManipulationPoint.PipelinePoint.Position = Telerik.Windows.Diagrams.Core.Utils.ProjectPointOnLine(e.TransformedPoint, prevPoint, nextPoint);*/
        }

        private bool OnStartDragging(Point currentPoint)
        {
            IsDragging = true;
            var handler = StartDragging;
            if (handler != null)
            {
                var args = new CancelingPositionChangedEventArgs(_startDragPoint, currentPoint);
                handler(this, args);
                IsDragging = !args.Cancel;
            }
            return IsDragging;
        }
    }
}