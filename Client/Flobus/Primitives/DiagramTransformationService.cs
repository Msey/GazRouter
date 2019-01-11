using System;
using System.Windows;
using System.Windows.Media;
using GazRouter.Flobus.PanZoom;
using JetBrains.Annotations;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Primitives
{
    internal class DiagramTransformationService
    {
        private readonly ScaleTransform _scaleLevelTransform;
        private readonly TranslateTransform _scaleOffsetTransform;
        private readonly TransformGroup _transfrom;
        private readonly TranslateTransform _translation;
        private readonly TransformGroup _scaleGroup;
        private ManipulationAdorner _adorner;
        private TranslateTransform _adornerScaleTranslation;
        private TranslateTransform _adornerTranslation;

        internal DiagramTransformationService()
        {
            _scaleLevelTransform = new ScaleTransform();
            _scaleOffsetTransform = new TranslateTransform();
            _scaleGroup = new TransformGroup();
            _scaleGroup.Children.Add(_scaleLevelTransform);
            _scaleGroup.Children.Add(_scaleOffsetTransform);

            _transfrom = new TransformGroup();
            _transfrom.Children.Add(_scaleGroup);
            _translation = new TranslateTransform();
            _transfrom.Children.Add(_translation);
        }

        public double ScaleX => _scaleLevelTransform.ScaleX;

        public double TranslationX => _translation.X;

        public double TranslationY => _translation.Y;

        public static DiagramTransformationInfo CalculateFit(Point sourcePoint, Point targetPoint, double zoom)
        {
            if (sourcePoint.X.IsNanOrInfinity() || sourcePoint.Y.IsNanOrInfinity() ||
                targetPoint.X.IsNanOrInfinity() || targetPoint.Y.IsNanOrInfinity() ||
                zoom.IsNanOrInfinity() || zoom <= 0)
            {
                return new DiagramTransformationInfo(new Point(0, 0), 1, new Point(0, 0));
            }

            var panVector = new Point((targetPoint.X - sourcePoint.X)*zoom, (targetPoint.Y - sourcePoint.Y)*zoom);
            var zoomVector = new Point(targetPoint.X*(zoom - 1), targetPoint.Y*(zoom - 1));
            var totalPanVector = panVector.Substract(zoomVector);
            totalPanVector = new Point(Math.Round(totalPanVector.X), Math.Round(totalPanVector.Y));
            return new DiagramTransformationInfo(totalPanVector, zoom, new Point(0, 0));
        }

        public static DiagramTransformationInfo CalculateFit(Rect sourceBounds, Rect targetBounds,
            Thickness targetBoundsMargin)
        {
            double fittingZoom = 1;
            var realTargetBoundsWidth = targetBounds.Width - targetBoundsMargin.Left - targetBoundsMargin.Right;
            var realTargetBoundsHeight = targetBounds.Height - targetBoundsMargin.Top - targetBoundsMargin.Bottom;

            if (targetBounds.IsEmpty || sourceBounds.IsEmpty ||
                realTargetBoundsWidth <= 0 || realTargetBoundsHeight <= 0)
            {
                return new DiagramTransformationInfo(new Point(0, 0), fittingZoom, new Point(0, 0));
            }

            var realTargetBounds = targetBounds;
            realTargetBounds.Width = realTargetBoundsWidth;
            realTargetBounds.Height = realTargetBoundsHeight;
            realTargetBounds.X = targetBounds.X + targetBoundsMargin.Left;
            realTargetBounds.Y = targetBounds.Y + targetBoundsMargin.Top;

            if (realTargetBounds.Width < sourceBounds.Width || realTargetBounds.Height < sourceBounds.Height)
            {
                var zoomX = realTargetBounds.Width/sourceBounds.Width;
                var zoomY = realTargetBounds.Height/sourceBounds.Height;
                fittingZoom = CoerceZoom(Math.Min(zoomX, zoomY));
            }

            var sourcePoint = sourceBounds.Center();
            var targetPoint = realTargetBounds.Center();

            return CalculateFit(sourcePoint, targetPoint, fittingZoom);
        }

        public void ApplyTransform([NotNull] FrameworkElement targetElement, ManipulationAdorner manipulationAdorner)
        {
            _scaleOffsetTransform.X = 0;
            _scaleOffsetTransform.Y = 0;
            targetElement.RenderTransform = _transfrom;

            _adorner = manipulationAdorner;
            if (_adorner != null)
            {
                // After theme change the old translation should be applied to the new Manipulation adorner.
                if (_translation != null)
                {
                    _adorner.ApplyPanTranslation(TranslationX, TranslationY);
                }

                var transformGroup = _adorner.RenderTransform as TransformGroup;
                if (transformGroup != null)
                {
                    var innerGroup = transformGroup.Children[0] as TransformGroup;
                    if (innerGroup != null)
                    {
                        _adornerScaleTranslation = innerGroup.Children[3] as TranslateTransform;
                    }

                    _adornerTranslation = transformGroup.Children[3] as TranslateTransform;
                }
            }
        }

        public Rect TranformToCurrent(Rect bounds)
        {
            return bounds != Rect.Empty ? _transfrom.TransformBounds(bounds) : Rect.Empty;
        }

        public void Zoom(double newZoom, Point newZoomCenter, ZoomType zoomType, Rect adornerBounds)
        {
            newZoom = CoerceZoom(newZoom);
            var deltaZoom = newZoom - _scaleLevelTransform.ScaleX;
            var deltaX = 0d;
            var deltaY = 0d;

            switch (zoomType)
            {
                case ZoomType.Absolute:
                    deltaX = -(newZoomCenter.X*newZoom - newZoomCenter.X) - _scaleOffsetTransform.X;
                    deltaY = -(newZoomCenter.Y*newZoom - newZoomCenter.Y) - _scaleOffsetTransform.Y;
                    break;
                case ZoomType.Incremental:
                    deltaX = -newZoomCenter.X*deltaZoom;
                    deltaY = -newZoomCenter.Y*deltaZoom;
                    break;
            }
            deltaX = Math.Round(deltaX);
            deltaY = Math.Round(deltaY);

            var endBounds = new Rect();
            if (adornerBounds.IsValidBounds())
            {
                endBounds = TransformZoomToCurrent(adornerBounds);
                if (endBounds != Rect.Empty)
                {
                    endBounds.X -= adornerBounds.X;
                    endBounds.Y -= adornerBounds.Y;
                }
            }

            _scaleLevelTransform.ScaleX += deltaZoom;
            _scaleLevelTransform.ScaleY += deltaZoom;
            _scaleOffsetTransform.X += deltaX;
            _scaleOffsetTransform.Y += deltaY;

            _adorner?.ApplyZoomTranslation(endBounds.X, endBounds.Y);
            _adorner?.Resize(endBounds.Width, endBounds.Height);
        }

        public void Pan(double newX, double newY)
        {
            var deltaX = newX - _translation.X;
            var deltaY = newY - _translation.Y;

            if (deltaX != 0 || deltaY != 0)
            {
                _adorner?.ApplyPanTranslation(deltaX, deltaY, true);
                _translation.X += deltaX;
                _translation.Y += deltaY;
            }
        }

        public Rect TranformToOriginal(Rect bounds)
        {
            return bounds != Rect.Empty ? _transfrom.Inverse.TransformBounds(bounds) : Rect.Empty;
        }

        public void Clear()
        {
            _scaleOffsetTransform.X = 0;
            _scaleOffsetTransform.Y = 0;
            _translation.X = 0;
            _translation.Y = 0;
            _scaleLevelTransform.ScaleX = 1;
            _scaleLevelTransform.ScaleY = 1;
        }

        public Rect TransformZoomToCurrent(Rect bounds)
        {
            if (bounds != Rect.Empty)
            {
                var transformBounds = _scaleGroup.TransformBounds(bounds);
                return transformBounds;
            }
            return Rect.Empty;
        }

        internal static double CoerceZoom(double zoom)
        {
            if (zoom < DiagramConstants.MinimumZoom)
            {
                zoom = DiagramConstants.MinimumZoom;
            }

            if (zoom > DiagramConstants.MaximumZoom)
            {
                zoom = DiagramConstants.MaximumZoom;
            }
            return zoom;
        }

        internal Point TranformToOriginal(Point point)
        {
            return _transfrom.Inverse.Transform(point);
        }
    }
}