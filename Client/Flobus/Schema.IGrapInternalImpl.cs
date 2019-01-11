using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Flobus.Extensions;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Primitives;
using Telerik.Windows;
using Telerik.Windows.Diagrams.Core;
using IAdornerService = GazRouter.Flobus.Services.AdornerService;
using IGraph = GazRouter.Flobus.Model.IGraph;
using SelectionService = GazRouter.Flobus.Services.SelectionService;

namespace GazRouter.Flobus
{
    public partial class Schema : IGraph
    {
        private bool _isMouseCaptured;
        private ManipulationAdorner _manipulationAdorner;

        public IEnumerable<IWidget> Widgets => Items;

        public SelectionMode SelectionMode => IsReadOnly ? SelectionMode.None : SelectionMode.Extended;

        public bool IsMouseCaptured
        {
            get { return _isMouseCaptured; }
            set
            {
                if (_isMouseCaptured == value)
                {
                    return;
                }

                _isMouseCaptured = value;
                if (_isMouseCaptured)
                {
                    CaptureMouse();
                }
                else
                {
                    ReleaseMouseCapture();
                }
            }
        }

        public bool IsPanEnabled { get; set; } = true;
        public bool IsSnapToGridEnabled { get; } = true;

        private IAdornerService AdornerService => ServiceLocator.AdornerService;

        private SelectionService SelectionService => ServiceLocator.SelectionService;

        public bool PublishDiagramEvent(DiagramEvent diagramEvent, object args)
        {
            switch (diagramEvent)
            {
                case DiagramEvent.SelectionBoundsChanged:
                {
                    UpdateAdorners();
                    break;
                }

                case DiagramEvent.Dragging:

                    UpdateItemInformationAdornerInformationTipVisibility(true);
                    break;
                case DiagramEvent.Drag:
                    UpdateItemInformationAdornerPosition();
                    UpdateItemInformationAdornerInformationTipVisibility(false);
                    break;
                case DiagramEvent.DragDelta:
                    UpdateInformationTip(AdornerService.AdornerBounds.TopLeft());
                    break;
            }
            return false;
        }

        public void UpdateRectSelection(Rect bounds)
        {
            if (_rectangleSelectionVisual == null)
            {
                return;
            }

            if (bounds == Rect.Empty)
            {
                _rectangleSelectionVisual.Visibility = Visibility.Collapsed;
            }
            else
            {
                _rectangleSelectionVisual.Visibility = Visibility.Visible;
                _rectangleSelectionVisual.SetLocation(bounds.Left, bounds.Top);
                _rectangleSelectionVisual.Height = bounds.Height;
                _rectangleSelectionVisual.Width = bounds.Width;
            }
        }

        public bool BringIntoView(Rect newViewPort)
        {
            if (newViewPort.IsEmpty)
            {
                return false;
            }

            var width = ItemsSurfaceActualWidth;
            var height = ItemsSurfaceActualHeight;
            var widthFactor = width/newViewPort.Width;
            var heightFactor = height/newViewPort.Height;
            var zoomFactor = Math.Min(widthFactor, heightFactor);

            var sourcePoint = new Point(newViewPort.Center().X, newViewPort.Center().Y);
            var targetPoint = new Point(width/2, height/2);
            var safeZoom = DiagramTransformationService.CoerceZoom(zoomFactor);

            var tranformationInfo = DiagramTransformationService.CalculateFit(sourcePoint, targetPoint, safeZoom);

            _suppressViewportUpdate = true;
            PanInternal(tranformationInfo.PanOffset);
            _suppressViewportUpdate = false;
            ZoomInternal(tranformationInfo.ZoomLevel, tranformationInfo.ZoomCenter, ZoomType.Absolute);

            return true;
        }

        public IAdornerPartResolver GetAdornerPartResolver()
        {
            return _manipulationAdorner;
        }

        public void UpdateAdorners()
        {
            var adornerBounds = CalculateAdornerBounds();

            UpdateManipulationAdorner(adornerBounds);
            UpdatePipelineManipulationAdorner(adornerBounds);
            UpdateItemInformationAdornerPosition();
        }

        private void UpdateManipulationAdorner(Rect adornerBounds)
        {
            if (_manipulationAdorner == null)
            {
                return;
            }

            var isManipulationAdornerVisible = SelectionService.SelectedItems.All(d => d.IsManipulationAdornerVisible);
            if (isManipulationAdornerVisible && adornerBounds.IsValidBounds())
            {
                var actualPosition = _transformationService.TransformZoomToCurrent(adornerBounds);
                _manipulationAdorner.ApplyZoomTranslation(actualPosition.X - adornerBounds.X,
                    actualPosition.Y - adornerBounds.Y);

                adornerBounds.Width = actualPosition.Width;
                adornerBounds.Height = actualPosition.Height;
            }

            var manipulating = adornerBounds != new Rect() && isManipulationAdornerVisible &&
                               !adornerBounds.X.IsNanOrInfinity() && !adornerBounds.Y.IsNanOrInfinity()
                               && !adornerBounds.Width.IsNanOrInfinity() && !adornerBounds.Height.IsNanOrInfinity();
            _manipulationAdorner.Update(SelectionService.SelectedItems, manipulating);

            if (manipulating)
            {
                var adornerX = Math.Floor(adornerBounds.X);
                var adornerY = Math.Floor(adornerBounds.Y);
                var adornerWidth = Math.Ceiling(adornerBounds.Width + (adornerBounds.X - adornerX));
                var adornerHeight = Math.Ceiling(adornerBounds.Height + (adornerBounds.Y - adornerY));

                _manipulationAdorner.Move(new Point(adornerX, adornerY));
                _manipulationAdorner.Resize(adornerWidth, adornerHeight);
            }
        }

        private Rect CalculateAdornerBounds()
        {
            var connection = SelectionService.SelectedItems.FirstOrDefault() as IPipelineWidget;
            var isSingleConnection = connection != null && SelectionService.IsSingleSelected(connection);

            return isSingleConnection ? connection.Bounds : AdornerService.InflatedAdornerBounds;
        }

    }
}