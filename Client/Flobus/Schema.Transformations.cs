using System.Windows;
using System.Windows.Input;
using GazRouter.Flobus.Primitives;
using Telerik.Windows.Diagrams.Core;
using IGraph = GazRouter.Flobus.Model.IGraph;

namespace GazRouter.Flobus
{
    public partial class Schema
    {
        private bool _isZoomProcessingInProgress;
        private bool _suppressViewportUpdate;
        private bool _isPanProcessingInProgress;

        public void BringIntoCenterView(Point position)
        {
            BringIntoView(position.Add(new Vector(Viewport.Width/-2, Viewport.Height/-2)), Zoom);
        }

        public void BringIntoView(Point position, double zoomLevel = 1)
        {
            if (position.IsNanOrInfinity())
            {
                return;
            }

            var sourcePoint = position;
            var targetPoint = new Point(0, 0);
            var safeZoom = DiagramTransformationService.CoerceZoom(zoomLevel);

            var transformationInfo = DiagramTransformationService.CalculateFit(sourcePoint, targetPoint, safeZoom);

            _suppressViewportUpdate = true;
            PanInternal(transformationInfo.PanOffset);
            _suppressViewportUpdate = false;
            ZoomInternal(transformationInfo.ZoomLevel, transformationInfo.ZoomCenter, ZoomType.Absolute);
        }

        public void ChangeZoom(Point targetPoint, bool isZoomIn)
        {
            var newZoom = CalculateNewZoom(Zoom, isZoomIn);
            var zoomPoint = _transformationService.TranformToOriginal(targetPoint);

            ZoomInternal(newZoom, zoomPoint, ZoomType.Incremental);
        }

        void IGraph.Pan(Point newPosition)
        {
            PanInternal(newPosition);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            ChangeZoom(e.GetPosition(this), e.Delta > 0);
        }

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Schema) d).OnPositionChanged((Point) e.NewValue);
        }

        private void OnZoomChanged(double newZoom)
        {
            var viewportCenter = ItemsSurfaceCenter;
            var newZoomPoint = _transformationService.TranformToOriginal(viewportCenter);

            ZoomInternal(newZoom, newZoomPoint, ZoomType.Incremental);
        }

        private void ZoomInternal(double newZoom, Point newZoomPoint, ZoomType zoomType)
        {
            if (_isZoomProcessingInProgress)
            {
                return;
            }

            _transformationService.Zoom(newZoom, newZoomPoint, zoomType, AdornerService.AdornerBounds);

            PanAndZoomServiceTransformationCompleted(true);
        }

        private void PanAndZoomServiceTransformationCompleted(bool isZoom)
        {
            if (isZoom)
            {
                _isZoomProcessingInProgress = true;
                Zoom = _transformationService.ScaleX;
                _isZoomProcessingInProgress = false;
            }
            else
            {
                _isPanProcessingInProgress = true;
                Position = new Point(_transformationService.TranslationX, _transformationService.TranslationY);
                _isPanProcessingInProgress = false;
            }

            if (_suppressViewportUpdate)
            {
                return;
            }

            UpdateViewport();
            SaveSchemeParams();
        }

        private double CalculateNewZoom(double oldZoom, bool isZoomIn)
        {
            DiagramConstants.ZoomScaleFactor = 0.05;
            var newZoom = isZoomIn
                ? oldZoom + DiagramConstants.ZoomScaleFactor
                : oldZoom - DiagramConstants.ZoomScaleFactor;
            var coercedZoom = DiagramTransformationService.CoerceZoom(newZoom);
            return coercedZoom != newZoom ? oldZoom : newZoom;
        }

        private void PanInternal(Point newPosition)
        {
            if (_isPanProcessingInProgress)
            {
                return;
            }

            _transformationService.Pan(newPosition.X, newPosition.Y);

            PanAndZoomServiceTransformationCompleted(false);
        }

        private void OnPositionChanged(Point newPosition)
        {
            PanInternal(newPosition);
        }

        private void AutoFit()
        {
            var margin = new Thickness(0);
            AutoFit(margin);
        }

        private void AutoFit(Thickness margin)
        {
            var sourceBounds = CalculateEnclosingBounds();
            var targetBounds = new Rect(0, 0, ItemsSurfaceActualWidth, ItemsSurfaceActualHeight);

            if (sourceBounds.IsEmpty || targetBounds.IsEmpty ||
                targetBounds.Width - margin.Left - margin.Right <= 0 ||
                targetBounds.Height - margin.Top - margin.Bottom <= 0)
            {
                return;
            }

            var transformInfo = DiagramTransformationService.CalculateFit(sourceBounds, targetBounds, margin);

            _suppressViewportUpdate = true;
            PanInternal(transformInfo.PanOffset);
            _suppressViewportUpdate = false;
            ZoomInternal(transformInfo.ZoomLevel, transformInfo.ZoomCenter, ZoomType.Absolute);
        }
    }
}