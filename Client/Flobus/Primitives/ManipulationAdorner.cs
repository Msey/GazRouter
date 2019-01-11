using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Visuals;
using Telerik.Windows;
using Telerik.Windows.Controls.Diagrams.Primitives;
using Telerik.Windows.Diagrams.Core;
using AdornerService = GazRouter.Flobus.Services.AdornerService;
using PropertyMetadata = Telerik.Windows.PropertyMetadata;

namespace GazRouter.Flobus.Primitives
{
    /// <summary>
    ///     Represents a manipulation adorner.
    /// </summary>
    [TemplatePart(Name = PartTopLeftResizeHandle, Type = typeof (FrameworkElement))]
    [TemplatePart(Name = PartTopRightResizeHandle, Type = typeof (FrameworkElement))]
    [TemplatePart(Name = PartBottomLeftResizeHandle, Type = typeof (FrameworkElement))]
    [TemplatePart(Name = PartBottomRightResizeHandle, Type = typeof (FrameworkElement))]
    public class ManipulationAdorner : AdornerBase, IAdornerPartResolver
    {
        /// <summary>
        ///     Identifies the IsResizingEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsResizingEnabledProperty =
            DependencyPropertyExtensions.Register("IsResizingEnabled", typeof (bool), typeof (ManipulationAdorner),
                new PropertyMetadata());

        /// <summary>
        ///     Identifies the IsRotationEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRotationEnabledProperty =
            DependencyPropertyExtensions.Register("IsRotationEnabled", typeof (bool), typeof (ManipulationAdorner),
                new PropertyMetadata());

        /// <summary>
        ///     Identifies the ZoomLevel dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyPropertyExtensions.Register("ZoomLevel", typeof (double), typeof (ManipulationAdorner),
                new PropertyMetadata(1d));

        /// <summary>
        ///     Identifies the ResizeActivationRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty ResizeActivationRadiusProperty =
            DependencyPropertyExtensions.Register("ResizeActivationRadius", typeof (double),
                typeof (ManipulationAdorner), new PropertyMetadata(6.0));

        /// <summary>
        ///     Identifies the RotateHitTestRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateHitTestRadiusProperty =
            DependencyProperty.Register("RotateHitTestRadius", typeof (double), typeof (DiagramAdornerBase),
                new PropertyMetadata(7.0));

        /// <summary>
        ///     Identifies the IsScalingEnabledProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty IsScalingEnabledProperty =
            DependencyProperty.Register("IsScalingEnabled", typeof (bool), typeof (ManipulationAdorner),
                new PropertyMetadata(true));

        private const string PartTopLeftResizeHandle = "TopLeftResizeHandle";
        private const string PartTopRightResizeHandle = "TopRightResizeHandle";
        private const string PartBottomLeftResizeHandle = "BottomLeftResizeHandle";
        private const string PartBottomRightResizeHandle = "BottomRightResizeHandle";
        private const string PartRotationHandle = "RotationPart";

        private TranslateTransform _zoomTranslation;
        private TranslateTransform _panTranslation;
        private FrameworkElement _topLeftResizeHandle;
        private FrameworkElement _topRightResizeHandle;
        private FrameworkElement _bottomLeftResizeHandle;
        private FrameworkElement _bottomRightResizeHandle;
        private FrameworkElement _rotationPart;

        private Rect _rotationPartBounds = Rect.Empty;
        private Rect _topLeftResizeHandleBounds = Rect.Empty;
        private Rect _topRightResizeHandleBounds = Rect.Empty;
        private Rect _bottomLeftResizeHandleBounds = Rect.Empty;
        private Rect _bottomRightResizeHandleBounds = Rect.Empty;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManipulationAdorner" /> class.
        /// </summary>
        public ManipulationAdorner()
        {
            SizeChanged += OnManipulationAdornerSizeChanged;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether rotation is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if rotation is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsRotationEnabled
        {
            get { return (bool) GetValue(IsRotationEnabledProperty); }
            set { SetValue(IsRotationEnabledProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether resizing is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if resizing is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsResizingEnabled
        {
            get { return (bool) GetValue(IsResizingEnabledProperty); }
            set { SetValue(IsResizingEnabledProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the zoom level.
        /// </summary>
        /// <value>
        ///     The zoom level.
        /// </value>
        public double ZoomLevel
        {
            get { return (double) GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the resize handles activation radius.
        /// </summary>
        public double ResizeActivationRadius
        {
            get { return (double) GetValue(ResizeActivationRadiusProperty); }
            set { SetValue(ResizeActivationRadiusProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the rotate handle activation radius.
        /// </summary>
        public double RotateHitTestRadius
        {
            get { return (double) GetValue(RotateHitTestRadiusProperty); }
            set { SetValue(RotateHitTestRadiusProperty, value); }
        }

        /// <summary>
        ///     Gets or sets whether the scaling is enabled.
        /// </summary>
        /// <value>The is scaling enabled.</value>
        public bool IsScalingEnabled
        {
            get { return (bool) GetValue(IsScalingEnabledProperty); }
            set { SetValue(IsScalingEnabledProperty, value); }
        }

        /// <summary>
        ///     Gets the part resolver.
        /// </summary>
        /// <value>The part resolver.</value>
        internal IAdornerPartResolver PartResolver => this;

        private AdornerService AdornerService => Schema.ServiceLocator.AdornerService;

        /// <summary>
        ///     Gets the resize handles activation radius.
        /// </summary>
        double IAdornerPartResolver.GetResizeActivationRadius()
        {
            if (IsScalingEnabled)
            {
                return ResizeActivationRadius;
            }

            return ResizeActivationRadius/Schema.Zoom;
        }

        /// <summary>
        ///     Gets the rotate handle activation radius.
        /// </summary>
        double IAdornerPartResolver.GetRotateActivationRadius()
        {
            if (IsScalingEnabled)
            {
                return RotateHitTestRadius;
            }

            return RotateHitTestRadius/Schema.Zoom;
        }

        /// <summary>
        ///     Gets the bounds of the rotation handle.
        /// </summary>
        /// <returns></returns>
        Rect IAdornerPartResolver.GetRotationElementBounds(bool forceRecalculation)
        {
            if (!_rotationPartBounds.X.IsNanOrInfinity() && !_rotationPartBounds.Y.IsNanOrInfinity())
            {
                var rotationCenter =
                    AdornerService.AdornerBounds.CenterTop()
                        .Add(ScalePoint(CalculateOffset(_rotationPartBounds, _rotationPart)));
                var rotationBounds = new Rect(rotationCenter.X - GetWidth(_rotationPart)/2,
                    rotationCenter.Y - GetHeight(_rotationPart)/2, GetWidth(_rotationPart), GetHeight(_rotationPart));
                var pivot = AdornerService.AdornerBounds.Pivot(RenderTransformOrigin);
                return rotationBounds.RotateRect(Rotation.Angle, pivot);
            }

            return Rect.Empty;
        }

        /// <summary>
        ///     Gets the offset of the rotation handle relative to top center point.
        /// </summary>
        /// <returns></returns>
        Point IAdornerPartResolver.GetRotationElementOffset(bool forceRecalculation)
        {
            if (_rotationPart == null)
            {
                return new Point(double.NaN, double.NaN);
            }

            if (forceRecalculation || _rotationPartBounds.X.IsNanOrInfinity() || _rotationPartBounds.Y.IsNanOrInfinity())
            {
                // We calculate the ratio between the rotationPart's bounds and the Adorner's bounds.
                var rotationBounds = GetVisualBounds(_rotationPart, this);
                var topLeft = PositionWithoutMargin(rotationBounds.TopLeft(), _rotationPart);

                // The AdornerService expects an offset from the TopCenter point and that's why we use it as pivot.
                _rotationPartBounds = new Rect(new Point((topLeft.X - Width/2)/Width, topLeft.Y/Height),
                    new Size(_rotationPart.ActualWidth/Width, _rotationPart.ActualHeight/Height));
            }

            return ScalePoint(CalculateOffset(_rotationPartBounds, _rotationPart));
        }

        /// <summary>
        ///     Gets the bounds of the top left resize handle.
        /// </summary>
        /// <returns></returns>
        Point IAdornerPartResolver.GetTopLeftResizeHandleOffset(bool forceRecalculation)
        {
            if (_topLeftResizeHandle == null)
            {
                return new Point(double.NaN, double.NaN);
            }

            if (forceRecalculation || _topLeftResizeHandleBounds.X.IsNanOrInfinity() ||
                _topLeftResizeHandleBounds.Y.IsNanOrInfinity())
            {
                var topLeftBounds = GetVisualBounds(_topLeftResizeHandle, this);
                var topLeft = PositionWithoutMargin(topLeftBounds.TopLeft(), _topLeftResizeHandle);

                // The AdornerService expects an offset from the TopLeft point and that's why we use it as pivot.
                _topLeftResizeHandleBounds = new Rect(new Point(topLeft.X/Width, topLeft.Y/Height),
                    new Size(_topLeftResizeHandle.ActualWidth/Width, _topLeftResizeHandle.ActualHeight/Height));
            }

            return ScalePoint(CalculateOffset(_topLeftResizeHandleBounds, _topLeftResizeHandle));
        }

        /// <summary>
        ///     Gets the bounds of the top right resize handle.
        /// </summary>
        /// <returns></returns>
        Point IAdornerPartResolver.GetTopRightResizeHandleOffset(bool forceRecalculation)
        {
            if (_topRightResizeHandle == null)
            {
                return new Point(double.NaN, double.NaN);
            }

            if (forceRecalculation || _topRightResizeHandleBounds.X.IsNanOrInfinity() ||
                _topRightResizeHandleBounds.Y.IsNanOrInfinity())
            {
                var topRightBounds = GetVisualBounds(_topRightResizeHandle, this);
                var topLeft = PositionWithoutMargin(topRightBounds.TopLeft(), _topRightResizeHandle);

                // The AdornerService expects an offset from the TopRight point and that's why we use it as pivot.
                _topRightResizeHandleBounds = new Rect(new Point((topLeft.X - Width)/Width, topLeft.Y/Height),
                    new Size(_topRightResizeHandle.ActualWidth/Width, _topRightResizeHandle.ActualHeight/Height));
            }

            return ScalePoint(CalculateOffset(_topRightResizeHandleBounds, _topRightResizeHandle));
        }

        /// <summary>
        ///     Gets the bounds of the bottom left resize handle.
        /// </summary>
        /// <returns></returns>
        Point IAdornerPartResolver.GetBottomLeftResizeHandleOffset(bool forceRecalculation)
        {
            if (_bottomLeftResizeHandle == null)
            {
                return new Point(double.NaN, double.NaN);
            }

            if (forceRecalculation || _bottomLeftResizeHandleBounds.X.IsNanOrInfinity() ||
                _bottomLeftResizeHandleBounds.Y.IsNanOrInfinity())
            {
                var bottomLeftBounds = GetVisualBounds(_bottomLeftResizeHandle, this);
                var topLeft = PositionWithoutMargin(bottomLeftBounds.TopLeft(), _bottomLeftResizeHandle);

                // The AdornerService expects an offset from the BottomLeft point and that's why we use it as pivot.
                _bottomLeftResizeHandleBounds = new Rect(new Point(topLeft.X/Width, (topLeft.Y - Height)/Height),
                    new Size(_bottomLeftResizeHandle.ActualWidth/Width, _bottomLeftResizeHandle.ActualHeight/Height));
            }

            return ScalePoint(CalculateOffset(_bottomLeftResizeHandleBounds, _bottomLeftResizeHandle));
        }

        /// <summary>
        ///     Gets the bounds of the bottom right resize handle.
        /// </summary>
        /// <returns></returns>
        Point IAdornerPartResolver.GetBottomRightResizeHandleOffset(bool forceRecalculation)
        {
            if (_bottomRightResizeHandle == null)
            {
                return new Point(double.NaN, double.NaN);
            }

            if (forceRecalculation || _bottomRightResizeHandleBounds.X.IsNanOrInfinity() ||
                _bottomRightResizeHandleBounds.Y.IsNanOrInfinity())
            {
                var bottomRightBounds = GetVisualBounds(_bottomRightResizeHandle, this);
                var topLeft = PositionWithoutMargin(bottomRightBounds.TopLeft(), _bottomRightResizeHandle);

                // The AdornerService expects an offset from the BottomRight point and that's why we use it as pivot.
                _bottomRightResizeHandleBounds =
                    new Rect(new Point((topLeft.X - Width)/Width, (topLeft.Y - Height)/Height),
                        new Size(_bottomRightResizeHandle.ActualWidth/Width,
                            _bottomRightResizeHandle.ActualHeight/Height));
            }

            return ScalePoint(CalculateOffset(_bottomRightResizeHandleBounds, _bottomRightResizeHandle));
        }

        /// <summary>
        ///     Updates the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="show">If set to <c>true</c> [show].</param>
        public void Update(IEnumerable<ISchemaItem> items, bool show)
        {
            var newVisibility = show ? Visibility.Visible : Visibility.Collapsed;

            if (items.Count() == 1 && items.FirstOrDefault() as ShapeWidgetBase == null)
            {
                newVisibility = Visibility.Collapsed;
            }

            Visibility = newVisibility;
        }

        /// <summary>
        ///     Gets the bounds of the rotation handle.
        /// </summary>
        /// <returns></returns>
        public virtual Point GetRenderTransformOrigin()
        {
            return RenderTransformOrigin;
        }

        /// <summary>
        ///     When overridden in a derived class, is invoked whenever application code or internal processes call
        ///     <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _topLeftResizeHandle = GetTemplateChild(PartTopLeftResizeHandle) as FrameworkElement;
            _topRightResizeHandle = GetTemplateChild(PartTopRightResizeHandle) as FrameworkElement;
            _bottomLeftResizeHandle = GetTemplateChild(PartBottomLeftResizeHandle) as FrameworkElement;
            _bottomRightResizeHandle = GetTemplateChild(PartBottomRightResizeHandle) as FrameworkElement;
            _rotationPart = GetTemplateChild(PartRotationHandle) as FrameworkElement;

            var groupTransform = RenderTransform as TransformGroup;
            if (groupTransform != null)
            {
                _panTranslation = groupTransform.Children[3] as TranslateTransform;

                var innerTransform = groupTransform.Children[0] as TransformGroup;
                if (innerTransform != null)
                {
                    _zoomTranslation = innerTransform.Children[3] as TranslateTransform;
                }
            }
        }

        internal bool ApplyZoomTranslation(double x, double y, bool isAddition = false)
        {
            return ApplyTranslation(_zoomTranslation, x, y, isAddition);
        }

        internal bool ApplyPanTranslation(double x, double y, bool isAddition = false)
        {
            return ApplyTranslation(_panTranslation, x, y, isAddition);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static Rect GetVisualBounds(FrameworkElement visualElement, UIElement comparedTo)
        {
            var result = Rect.Empty;
            if (visualElement != null && visualElement.ActualWidth > 0d && visualElement.ActualHeight > 0d)
            {
                try
                {
                    result =
                        visualElement.TransformToVisual(comparedTo)
                            .TransformBounds(new Rect(0, 0, visualElement.ActualWidth, visualElement.ActualHeight));
                }
                catch
                {
                }
            }

            return result;
        }

        private static double GetHeight(FrameworkElement element)
        {
            return element.Height.IsNanOrInfinity() ? element.ActualHeight : element.Height;
        }

        private static double GetWidth(FrameworkElement element)
        {
            return element.Width.IsNanOrInfinity() ? element.ActualWidth : element.Width;
        }

        private static bool ApplyTranslation(TranslateTransform transform, double x, double y, bool isAddition)
        {
            if (transform == null || double.IsInfinity(x) || double.IsInfinity(y))
            {
                return false;
            }

            if (isAddition)
            {
                transform.X += x;
                transform.Y += y;
            }
            else
            {
                transform.X = x;
                transform.Y = y;
            }
            return true;
        }

        private static Point PositionWithoutMargin(Point position, FrameworkElement element)
        {
            if (element == null)
            {
                return new Point();
            }

            var margin = element.Margin;
            var result = new Point();

            if (element.HorizontalAlignment == HorizontalAlignment.Right)
            {
                result.X = position.X + margin.Right;
            }
            else if (element.HorizontalAlignment == HorizontalAlignment.Left)
            {
                result.X = position.X - margin.Left;
            }
            else
            {
                result.X = position.X - margin.Left/2 + margin.Right/2;
            }

            if (element.VerticalAlignment == VerticalAlignment.Bottom)
            {
                result.Y = position.Y + margin.Bottom;
            }
            else if (element.VerticalAlignment == VerticalAlignment.Top)
            {
                result.Y = position.Y - margin.Top;
            }
            else
            {
                result.Y = position.Y - margin.Top/2 + margin.Bottom/2;
            }

            return result;
        }

        private void OnManipulationAdornerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizeChanged -= OnManipulationAdornerSizeChanged;

            PartResolver.GetRotationElementOffset(true);
            PartResolver.GetTopLeftResizeHandleOffset(true);
            PartResolver.GetTopRightResizeHandleOffset(true);
            PartResolver.GetBottomLeftResizeHandleOffset(true);
            PartResolver.GetBottomRightResizeHandleOffset(true);
        }

        private Point CalculateOffset(Rect ratio, FrameworkElement element)
        {
            if (element == null)
            {
                return new Point();
            }

            var margin = element.Margin;
            var topLeft = new Point(ratio.X*Width, ratio.Y*Height);

            if (element.HorizontalAlignment == HorizontalAlignment.Right)
            {
                topLeft.X += ratio.Width*Width - element.ActualWidth/2 - margin.Right;
            }
            else if (element.HorizontalAlignment == HorizontalAlignment.Left)
            {
                topLeft.X += element.ActualWidth/2 + margin.Left;
            }
            else
            {
                topLeft.X += ratio.Width*Width/2 + margin.Left/2 - margin.Right/2;
            }

            if (element.VerticalAlignment == VerticalAlignment.Bottom)
            {
                topLeft.Y += ratio.Height*Height - element.ActualHeight/2 - margin.Bottom;
            }
            else if (element.VerticalAlignment == VerticalAlignment.Top)
            {
                topLeft.Y += element.ActualHeight/2 + margin.Top;
            }
            else
            {
                topLeft.Y += ratio.Height*Height/2 + margin.Top/2 - margin.Bottom/2;
            }

            return new Point(topLeft.X, topLeft.Y);
        }

        private Point ScalePoint(Point point)
        {
            if (IsScalingEnabled)
            {
                return point;
            }

            return point.Divide(Schema.Zoom);
        }
    }
}