using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GazRouter.Flobus.Extensions;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Thumbnail
{
    [TemplatePart(Name = PartViewportRect, Type = typeof (Rectangle))]
    [TemplatePart(Name = PartMainCanvas, Type = typeof (Canvas))]
    public class SchemaThumbnail : Control
    {
        public static readonly DependencyProperty SchemaProperty = DependencyProperty.Register(
            "Schema", typeof (Schema), typeof (SchemaThumbnail), new PropertyMetadata(null, OnDiagramPropertyChanged));

        public static readonly DependencyProperty ViewportRectProperty = DependencyProperty.Register(
            "ViewportRect", typeof (Rect), typeof (SchemaThumbnail),
            new PropertyMetadata(new Rect(), OnViewportRectPropertyChanged));

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            "ImageSource", typeof (ImageSource), typeof (SchemaThumbnail), new PropertyMetadata(null));

        private const string PartMainCanvas = "PART_mainCanvas";
        private const string PartViewportRect = "PART_viewportRect";
        private readonly bool _shouldRefresh = true;
        private Size _diagramSize;
        private Rect _imageRectangle;
        private Canvas _mainCanvas;
        private Rectangle _viewportRectangle;
        private Point _dragStartPoint;
        private bool _isLeftButtonDown;
        private Point _positionOnDragStart;
        private bool _isDragging;

        public SchemaThumbnail()
        {
            DefaultStyleKey = typeof (SchemaThumbnail);

            SizeChanged += OnSizeChanged;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource) GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public Rect ViewportRect
        {
            get { return (Rect) GetValue(ViewportRectProperty); }
            set { SetValue(ViewportRectProperty, value); }
        }

        public Schema Schema
        {
            get { return (Schema) GetValue(SchemaProperty); }
            set { SetValue(SchemaProperty, value); }
        }

        public bool ShouldCalculate
        {
            get { return ActualWidth != 0 && ActualHeight != 0 && Visibility != Visibility.Collapsed; }
        }

        public double Zoom => Schema?.Zoom ?? 1;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_mainCanvas != null)
            {
                _mainCanvas.Loaded -= OnMainCanvasLoaded;
                _mainCanvas.SizeChanged -= OnMainCanvasSizeChanged;
            }

            _mainCanvas = GetTemplateChild(PartMainCanvas) as Canvas;

            if (_mainCanvas != null)
            {
                _mainCanvas.Loaded += OnMainCanvasLoaded;
                _mainCanvas.SizeChanged += OnMainCanvasSizeChanged;
            }

            _viewportRectangle = GetTemplateChild(PartViewportRect) as Rectangle;

            UpdateViewportBorderSizeAndPosition();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            _dragStartPoint = e.GetPosition(this);
            _isLeftButtonDown = true;
            if (!ViewportRect.Contains(_dragStartPoint))
            {
                ViewportRect =
                    new Rect(
                        new Point(_dragStartPoint.X - ViewportRect.Width/2, _dragStartPoint.Y - ViewportRect.Height/2),
                        ViewportRect.ToSize());
                SetDiagramPosition();
            }
            _positionOnDragStart = ViewportRect.TopLeft();

            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            _isLeftButtonDown = false;
            _isDragging = false;
            ReleaseMouseCapture();

            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_isLeftButtonDown)
            {
                return;
            }

            var currentMousePosition = e.GetPosition(this);
            var offsetX = currentMousePosition.X - _dragStartPoint.X;
            var offsetY = currentMousePosition.Y - _dragStartPoint.Y;

            if (_isDragging)
            {
                var viewportSize = ViewportRect.ToSize();
                var halfViewportWidth = viewportSize.Width/2;
                var halfViewportHeight = viewportSize.Height/2;
                var possiblePosition = new Point(_positionOnDragStart.X + offsetX, _positionOnDragStart.Y + offsetY);
                var positionX = Math.Max(-halfViewportWidth,
                    Math.Min(possiblePosition.X, _mainCanvas.ActualWidth - halfViewportWidth));
                var positionY = Math.Max(-halfViewportHeight,
                    Math.Min(possiblePosition.Y, _mainCanvas.ActualHeight - halfViewportHeight));
                ViewportRect = new Rect(new Point(positionX, positionY), viewportSize);

                SetDiagramPosition();
            }
            else if (Math.Abs(offsetX) > 2 || Math.Abs(offsetY) > 2)
            {
                _isDragging = true;
                CaptureMouse();
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            var zoomPoint = e.GetPosition(_viewportRectangle);
            var diagramZoomPoint = new Point(zoomPoint.X*(_diagramSize.Width/ViewportRect.Width),
                zoomPoint.Y*(_diagramSize.Height/ViewportRect.Height));
            diagramZoomPoint = double.IsNaN(diagramZoomPoint.X) || double.IsNaN(diagramZoomPoint.Y)
                ? Schema.Viewport.TopLeft()
                : diagramZoomPoint;

            Schema.ChangeZoom(diagramZoomPoint, e.Delta > 0);

            e.Handled = true;
        }

        private static void OnViewportRectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thumbnail = d as SchemaThumbnail;
            if (thumbnail != null)
            {
                thumbnail.UpdateViewportBorderSizeAndPosition();
            }
        }

        private static void OnDiagramPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SchemaThumbnail)?.OnDiagramChanged((Schema) e.OldValue, (Schema) e.NewValue);
        }

        private void UpdateViewportSizeAndPosition()
        {
            if (Schema == null || !Schema.IsLoaded || _isLeftButtonDown || _mainCanvas == null || !ShouldCalculate)
            {
                return;
            }

            var viewportWidth = _mainCanvas.ActualWidth*(_diagramSize.Width/_imageRectangle.Width)/Zoom;
            var viewportHeight = _mainCanvas.ActualHeight*(_diagramSize.Height/_imageRectangle.Height)/Zoom;

            var viewportOffsetX = (Schema.Viewport.X - _imageRectangle.X)/
                                  (_imageRectangle.Width/_mainCanvas.ActualWidth);
            var viewportOffsetY = (Schema.Viewport.Y - _imageRectangle.Y)/
                                  (_imageRectangle.Height/_mainCanvas.ActualHeight);

            if (!double.IsInfinity(viewportOffsetX) && !double.IsInfinity(viewportOffsetY) &&
                !double.IsInfinity(viewportWidth) && !double.IsInfinity(viewportHeight) &&
                viewportHeight >= 0 && viewportWidth >= 0)
            {
                ViewportRect = new Rect(new Point(viewportOffsetX, viewportOffsetY),
                    new Size(viewportWidth, viewportHeight));
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            (sender as SchemaThumbnail)?.RefreshThumbnail();
        }

        private void OnVisualChange()
        {
            if (Schema == null || !Schema.IsLoaded || !ShouldCalculate)
            {
                return;
            }

            _imageRectangle = GetImageRectangle();
            CreateBackgroundImage(_imageRectangle);
        }

        private void DetachEvents(Schema diagram)
        {
            if (diagram == null)
            {
                return;
            }

            diagram.SizeChanged -= OnDiagramSizeChanged;
            diagram.Loaded -= OnRefreshThumbnail;
            diagram.ViewportChanged -= OnDiagramViewportChanged;
            diagram.VisualChildrenChanged -= OnRefreshThumbnail;
        }

        private void AttachEvents(Schema diagram)
        {
            if (diagram == null)
            {
                return;
            }

            diagram.SizeChanged += OnDiagramSizeChanged;
            diagram.Loaded += OnRefreshThumbnail;
            diagram.ViewportChanged += OnDiagramViewportChanged;
            diagram.VisualChildrenChanged += OnRefreshThumbnail;
        }

        private void OnRefreshThumbnail(object sender, System.EventArgs e)
        {
            if (!_shouldRefresh)
            {
                return;
            }

            Dispatcher.BeginInvoke(RefreshThumbnail);
        }

        private void OnDiagramViewportChanged(object sender, PropertyEventArgs<Rect> e)
        {
            if (_shouldRefresh)
            {
                UpdateViewportSizeAndPosition();
            }
        }

        private void CreateBackgroundImage(Rect imageRect)
        {
            if (Schema == null || !Schema.IsLoaded || _mainCanvas == null ||
                imageRect.Width.IsNanOrInfinity() || imageRect.Height.IsNanOrInfinity() ||
                imageRect.X.IsNanOrInfinity() || imageRect.Y.IsNanOrInfinity())
            {
                return;
            }

            ImageSource = Schema.CreateDiagramImage(imageRect,
                new Size(_mainCanvas.ActualWidth, _mainCanvas.ActualHeight));
        }

        private void OnDiagramSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_shouldRefresh)
            {
                return;
            }

            _diagramSize = e.NewSize;
            var newImageRect = GetImageRectangle();
            if (_imageRectangle != newImageRect)
            {
                _imageRectangle = newImageRect;
                CreateBackgroundImage(_imageRectangle);
            }
            UpdateViewportSizeAndPosition();
        }

        private void SetDiagramPosition()
        {
            var newPosition = CalculateDiagramPoint(ViewportRect.TopLeft());
            Schema?.BringIntoView(newPosition, Zoom);
        }

        private Point CalculateDiagramPoint(Point thumbnailPoint)
        {
            if (_mainCanvas == null || Schema == null)
            {
                return new Point();
            }

            var offsetX = thumbnailPoint.X*(_imageRectangle.Width/_mainCanvas.ActualWidth) + _imageRectangle.X;
            var offsetY = thumbnailPoint.Y*(_imageRectangle.Height/_mainCanvas.ActualHeight) + _imageRectangle.Y;

            return new Point(offsetX, offsetY);
        }

        private void UpdateViewportBorderSizeAndPosition()
        {
            if (_viewportRectangle == null)
            {
                return;
            }

            var newViewportRect = ViewportRect;
            var strokeThickness = _viewportRectangle.StrokeThickness;
            var doubleStrokeThickness = strokeThickness*2;
            _viewportRectangle.Width = newViewportRect.Width + doubleStrokeThickness;
            _viewportRectangle.Height = newViewportRect.Height + doubleStrokeThickness;
            _viewportRectangle.SetLocation(newViewportRect.X - strokeThickness, newViewportRect.Y - strokeThickness);
        }

        private void OnMainCanvasLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ClipFrameworkElement(sender as FrameworkElement);
        }

        private void OnMainCanvasSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            ClipFrameworkElement(sender as FrameworkElement);
        }

        private double GetDiagramHeight()
        {
            if (_diagramSize.Height != 0)
            {
                return _diagramSize.Height;
            }

            return Schema.ActualHeight;
        }

        private double GetDiagramWidth()
        {
            if (_diagramSize.Width != 0)
            {
                return _diagramSize.Width;
            }

            return Schema.ActualWidth;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            DetachEvents(Schema);
            AttachEvents(Schema);
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            DetachEvents(Schema);
        }

        private void RefreshThumbnail()
        {
            OnVisualChange();
            UpdateViewportSizeAndPosition();
        }

        private void ClipFrameworkElement(FrameworkElement frameworkElement)
        {
            frameworkElement.Clip = new RectangleGeometry
            {
                Rect = new Rect(0, 0, frameworkElement.ActualWidth, frameworkElement.ActualHeight)
            };
        }

        private Rect GetImageRectangle()
        {
            if (_mainCanvas == null)
            {
                return new Rect();
            }

            var itemsBounds = Schema.CalculateEnclosingBounds();

            var thumbnailRatio = _mainCanvas.ActualWidth/_mainCanvas.ActualHeight;
            var maxHeight = Math.Max(GetDiagramHeight(), itemsBounds.Height);
            var maxWidth = Math.Max(GetDiagramWidth(), itemsBounds.Width);
            var imageRatio = maxWidth/maxHeight;
            var minimumImageSize = new Size(maxWidth, maxHeight);
            var imageSize = new Size();
            if (thumbnailRatio >= imageRatio)
            {
                imageSize.Height = minimumImageSize.Height;
                imageSize.Width = minimumImageSize.Height*thumbnailRatio;
            }
            else
            {
                imageSize.Width = minimumImageSize.Width;
                imageSize.Height = minimumImageSize.Width/thumbnailRatio;
            }

            var imagePosition = new Point(itemsBounds.CenterX() - imageSize.Width/2,
                itemsBounds.CenterY() - imageSize.Height/2);
            if (imagePosition.X.IsNanOrInfinity() || imagePosition.Y.IsNanOrInfinity())
            {
                imagePosition = new Point();
            }

            return new Rect(imagePosition, imageSize);
        }

        private void OnDiagramChanged(Schema oldDiagram, Schema newDiagram)
        {
            if (oldDiagram != null)
            {
                DetachEvents(oldDiagram);
            }
            if (newDiagram != null)
            {
                AttachEvents(newDiagram);

                if (_diagramSize.Width != newDiagram.ActualWidth || _diagramSize.Height != newDiagram.ActualHeight)
                {
                    _diagramSize = new Size(newDiagram.ActualWidth, newDiagram.ActualHeight);
                }

                RefreshThumbnail();
            }
        }
    }
}