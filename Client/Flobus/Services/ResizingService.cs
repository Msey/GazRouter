using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GazRouter.Flobus.Visuals;
using Telerik.Windows.Diagrams.Core;
using IGraph = GazRouter.Flobus.Model.IGraph;

namespace GazRouter.Flobus.Services
{
    public class ResizingService : SchemaServiceBase
    {
        internal const int InverseAngle = 180;
        private readonly Dictionary<ShapeWidgetBase, Rect> _shapeToBoundsMap = new Dictionary<ShapeWidgetBase, Rect>();

        private Point _oldDelta;
        private Size _minAdornerSize;
        private Rect _startAdornerBounds;
        private Rect _minAdornerBounds;
        private IEnumerable<ShapeWidgetBase> _selectedItems;
        private double _adornerAngle;
        private Point _reversedStartPoint;
        private ResizeDirection _resizeDirection;
        private Point _pinnedPoint;
        private Point _transformPivot;
        private Point _transformOrigin;
        private Size _maxAdornerSize;

        public ResizingService(IGraph graph) : base(graph)
        {
        }

        /// <summary>
        ///     Occurs when resizing.
        /// </summary>
        public event EventHandler<ResizingEventArgs> Resizing;

        public bool IsResizing { get; private set; }

        public void InitializeResize(IEnumerable<ShapeWidgetBase> newSelectedItems, double adornerAngle,
            Rect adornerBounds, ResizeDirection resizeDirection, Point startPoint)
        {
            _adornerAngle = adornerAngle;
            _startAdornerBounds = adornerBounds;
            _selectedItems = newSelectedItems;
            _transformOrigin = Graph.GetAdornerPartResolver().GetRenderTransformOrigin();

            if (_selectedItems.Count() > 1)
            {
                adornerBounds = adornerBounds.InflateRect(7);
            }
            _transformPivot = new Point(adornerBounds.X + _transformOrigin.X*adornerBounds.Width,
                adornerBounds.Y + _transformOrigin.Y*adornerBounds.Height);
            _reversedStartPoint = Reverse(_transformPivot, startPoint, _adornerAngle);

            CalculateRelativePositions();

            _resizeDirection = resizeDirection;
            _oldDelta = new Point();
            switch (_resizeDirection)
            {
                case ResizeDirection.NorthEastSouthWest:
                    _pinnedPoint = _startAdornerBounds.BottomLeft(_transformOrigin, _adornerAngle);
                    break;
                case ResizeDirection.SouthWestNorthEast:
                    _pinnedPoint = _startAdornerBounds.TopRight(_transformOrigin, _adornerAngle);
                    break;
                case ResizeDirection.NorthWestSouthEast:
                    _pinnedPoint = _startAdornerBounds.BottomRight(_transformOrigin, _adornerAngle);
                    break;
                case ResizeDirection.SouthEastNorthWest:
                    _pinnedPoint = _startAdornerBounds.TopLeft(_transformOrigin, _adornerAngle);
                    break;
            }
        }

        public bool StartResize(Point currentPoint)
        {
            return OnStartResizing(currentPoint);
        }

        /// <summary>
        ///     Completes the resize.
        /// </summary>
        /// <param name="finalBounds">The final bounds.</param>
        /// <param name="mousePosition">The final mouse position.</param>
        public void CompleteResize(Rect finalBounds, Point mousePosition)
        {
            IsResizing = false;
            _shapeToBoundsMap.Clear();
            _selectedItems = null;
        }
        
        public void Resize(Point newPoint)
        {
            var newDelta = CalculateNewDelta(newPoint);

            if (_oldDelta.X != newDelta.X || _oldDelta.Y != newDelta.Y)
            {
                var newAdornerBounds = InternalResize(newDelta);
                _oldDelta = newDelta;
                OnResizing(newAdornerBounds, newPoint);
            }
        }

        private static Point CalculateRelativePosition(Point point, Rect rect)
        {
            var relartiveX = Math.Round((point.X - rect.X) / rect.Width, 4);
            var relartiveY = Math.Round((point.Y - rect.Y) / rect.Height, 4);

            return new Point(relartiveX, relartiveY);
        }

        private void OnResizing(Rect newBounds, Point mousePosition)
        {
            Resizing?.Invoke(this, new ResizingEventArgs(null, newBounds, mousePosition));
        }

        private Rect InternalResize(Point sizeDelta)
        {
            var newAdornerSize = new Size(Math.Max(_minAdornerSize.Width, _startAdornerBounds.Width + sizeDelta.X),
                Math.Max(_minAdornerSize.Height, _startAdornerBounds.Height + sizeDelta.Y));

            newAdornerSize = new Size(Math.Min(_maxAdornerSize.Width, newAdornerSize.Width),
                Math.Min(_maxAdornerSize.Height, newAdornerSize.Height));

            var newAdornerBounds = UpdateAdornerBounds(newAdornerSize);

            if (_minAdornerBounds != Rect.Empty && !newAdornerBounds.Contains(_minAdornerBounds))
            {
                newAdornerBounds.Union(_minAdornerBounds);
            }

            var newAdornerCenter = newAdornerBounds.Center();

            foreach (var item in _selectedItems)
            {
                ResizeShape(item, newAdornerBounds, newAdornerCenter);
            }

            return newAdornerBounds;
        }

        private void ResizeShape(ShapeWidgetBase shape, Rect newAdornerBounds, Point rotationPivot)
        {
            var relativeBounds = _shapeToBoundsMap[shape];

            Size newShapSize;
            if (IsInverse(Math.Abs(shape.RotationAngle - _adornerAngle)))
            {
                newShapSize = new Size(newAdornerBounds.Height*relativeBounds.Width,
                    newAdornerBounds.Width*relativeBounds.Height);
            }
            else
            {
                newShapSize = new Size(newAdornerBounds.Width*relativeBounds.Width,
                    newAdornerBounds.Height*relativeBounds.Height);
            }
            newShapSize = new Size(Math.Max(newShapSize.Width, DiagramConstants.MinimumShapeSize),
                Math.Max(newShapSize.Height, DiagramConstants.MinimumShapeSize));

            if (_selectedItems.Count() > 1)
            {
                var originalOrigin = new Point(newAdornerBounds.X + newAdornerBounds.Width*relativeBounds.X,
                    newAdornerBounds.Y + newAdornerBounds.Height*relativeBounds.Y);
                var originalPosition =
                    originalOrigin.Substract(new Point(newShapSize.Width*shape.RenderTransformOrigin.X,
                        newShapSize.Height*shape.RenderTransformOrigin.Y));

                var rotatedOrigin = originalOrigin.Rotate(rotationPivot, _adornerAngle);
                var rotatedPosition = originalPosition.Rotate(rotationPivot, _adornerAngle);

                shape.Position = rotatedPosition.Rotate(rotatedOrigin, -_adornerAngle);
            }
            else
            {
                shape.Position = newAdornerBounds.TopLeft();
            }

            shape.Width = newShapSize.Width;
            shape.Height = newShapSize.Height;
        }

        private Rect UpdateAdornerBounds(Size newSize)
        {
            var inverseRect = new Rect(_pinnedPoint, newSize);
            var newPosition = _pinnedPoint;

            switch (_resizeDirection)
            {
                case ResizeDirection.NorthWestSouthEast:
                    newPosition = inverseRect.BottomRight(new Point(), _adornerAngle + InverseAngle);
                    break;
                case ResizeDirection.NorthEastSouthWest:
                    newPosition = inverseRect.BottomLeft(new Point(), _adornerAngle + InverseAngle);
                    break;
                case ResizeDirection.SouthEastNorthWest:
                    break;
                case ResizeDirection.SouthWestNorthEast:
                    newPosition = inverseRect.TopRight(new Point(), _adornerAngle + InverseAngle);
                    break;
            }
            inverseRect = new Rect(newPosition, newSize);

            var inverseOrigin = new Point(inverseRect.X + _transformOrigin.X*inverseRect.Width,
                inverseRect.Y + _transformOrigin.Y*inverseRect.Height);
            newPosition = newPosition.Rotate(inverseOrigin.Rotate(inverseRect.TopLeft(), _adornerAngle), -_adornerAngle);

            return new Rect(newPosition, newSize);
        }

        private Point CalculateNewDelta(Point newPoint)
        {
            var reversedCurrentPoint = Reverse(_startAdornerBounds.Center(), newPoint, _adornerAngle);
            var newDelta = new Point(reversedCurrentPoint.X - _reversedStartPoint.X,
                reversedCurrentPoint.Y - _reversedStartPoint.Y);
            newDelta = Graph.IsSnapToGridEnabled ? newDelta.Snap(Graph.Snap, Graph.Snap) : newDelta;

            return newDelta;
        }

        private Point Reverse(Point rotationCenter, Point point, double angle)
        {
            var newRotateTransform = new RotateTransform
            {
                Angle = angle,
                CenterX = rotationCenter.X,
                CenterY = rotationCenter.Y
            };
            return newRotateTransform.Inverse.Transform(point);
        }

        private bool OnStartResizing(Point currentPoint)
        {
            IsResizing = true;
            return true;
        }

        private void CalculateRelativePositions()
        {
            _shapeToBoundsMap.Clear();

            if (_selectedItems == null)
            {
                return;
            }

            var maxDecreaseHeightDelta = double.PositiveInfinity;
            var maxDecreaseWidthDelta = double.PositiveInfinity;
            var maxIncreaseWidthDelta = double.PositiveInfinity;
            var maxIncreaseHeightDelta = double.PositiveInfinity;

            var minTopLeftPoint = new Point(double.PositiveInfinity, double.PositiveInfinity);
            var maxBottomRightPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
            foreach (var item in _selectedItems)
            {
                if (!minTopLeftPoint.IsNanOrInfinity() && !maxBottomRightPoint.IsNanOrInfinity())
                {
                    _minAdornerBounds = new Rect(minTopLeftPoint, maxBottomRightPoint);
                }
                else
                {
                    _minAdornerBounds = Rect.Empty;
                }

                var shape = item;
                if (shape != null)
                {
                    var transformPivot = shape.Bounds.Pivot(shape.RenderTransformOrigin)
                        .Rotate(_transformPivot, -_adornerAngle);
                    var relativePosition = CalculateRelativePosition(transformPivot, _startAdornerBounds);

                    var relativeSize = new Size();
                    if (IsInverse(Math.Abs(shape.RotationAngle - _adornerAngle)))
                    {
                        relativeSize = new Size(shape.Bounds.Width/_startAdornerBounds.Height,
                            shape.Bounds.Height/_startAdornerBounds.Width);
                        maxDecreaseHeightDelta = Math.Min(maxDecreaseHeightDelta,
                            (shape.Bounds.Width - shape.MinWidth)/shape.Bounds.Width*_startAdornerBounds.Height);
                        maxDecreaseWidthDelta = Math.Min(maxDecreaseWidthDelta,
                            (shape.Bounds.Height - shape.MinHeight)/shape.Bounds.Height*_startAdornerBounds.Width);

                        maxIncreaseHeightDelta = Math.Min(maxIncreaseHeightDelta,
                            (shape.MaxWidth - shape.Bounds.Width)/shape.Bounds.Width*_startAdornerBounds.Height);
                        maxIncreaseWidthDelta = Math.Min(maxIncreaseWidthDelta,
                            (shape.MaxHeight - shape.Bounds.Height)/shape.Bounds.Height*_startAdornerBounds.Width);
                    }
                    else
                    {
                        relativeSize = new Size(shape.Bounds.Width/_startAdornerBounds.Width,
                            shape.Bounds.Height/_startAdornerBounds.Height);
                        maxDecreaseHeightDelta = Math.Min(maxDecreaseHeightDelta,
                            (shape.Bounds.Height - shape.MinHeight)/shape.Bounds.Height*_startAdornerBounds.Height);
                        maxDecreaseWidthDelta = Math.Min(maxDecreaseWidthDelta,
                            (shape.Bounds.Width - shape.MinWidth)/shape.Bounds.Width*_startAdornerBounds.Width);

                        maxIncreaseHeightDelta = Math.Min(maxIncreaseHeightDelta,
                            (shape.MaxHeight - shape.Bounds.Height)/shape.Bounds.Height*_startAdornerBounds.Height);
                        maxIncreaseWidthDelta = Math.Min(maxIncreaseWidthDelta,
                            (shape.MaxWidth - shape.Bounds.Width)/shape.Bounds.Width*_startAdornerBounds.Width);
                    }

                    _shapeToBoundsMap.Add(shape, new Rect(relativePosition, relativeSize));
                }
            }

            _minAdornerSize =
                new Size(
                    Math.Max(DiagramConstants.MinimumAdornerSize, _startAdornerBounds.Width - maxDecreaseWidthDelta),
                    Math.Max(DiagramConstants.MinimumAdornerSize, _startAdornerBounds.Height - maxDecreaseHeightDelta));

            _maxAdornerSize = new Size(_startAdornerBounds.Width + maxIncreaseWidthDelta,
                _startAdornerBounds.Height + maxIncreaseHeightDelta);
        }

        private bool IsInverse(double combinedAngle)
        {
            if (_selectedItems.Count() > 1 && ((combinedAngle > 45 && combinedAngle < 135) ||
                                               (combinedAngle > 225 && combinedAngle < 315)))
            {
                return true;
            }

            return false;
        }

    }
}