using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Diagrams.Core;
using Orientation = System.Windows.Controls.Orientation;

namespace GazRouter.Flobus.Primitives
{
    [TemplatePart(Name = PartLinesPanel, Type = typeof (Panel))]
    public partial class BackgroundGrid : Control
    {
        public static readonly DependencyProperty IsGridVisibleProperty = DependencyProperty.RegisterAttached(
            "IsGridVisible", typeof (bool), typeof (BackgroundGrid),
            new PropertyMetadata(false, OnIsGridVisiblePropertyChanged));

        private const string PartLinesPanel = "LinesPanel";
        private readonly Size _cellSize = new Size(10, 10);
        private readonly SolidColorBrush _lineStroke = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
        private LineDescriptor _verticalDescriptor;
        private LineDescriptor _horizontalDescriptor;
        private LineContainerRecycler _containerRecycler;
        private Panel _panel;
        private Schema _schema;

        public BackgroundGrid()
        {
            DefaultStyleKey = typeof (BackgroundGrid);
        }

        internal Schema Schema
        {
            get { return _schema; }
            set
            {
                if (_schema != null)
                {
                    _schema.ViewportChanged -= SchemaOnViewportChanged;
                }

                _schema = value;

                if (_schema != null)
                {
                    _schema.ViewportChanged += SchemaOnViewportChanged;
                }
                Refresh(true);
            }
        }

        public static void SetIsGridVisible(DependencyObject element, bool value)
        {
            element.SetValue(IsGridVisibleProperty, value);
        }

        public static bool GetIsGridVisible(DependencyObject element)
        {
            return (bool) element.GetValue(IsGridVisibleProperty);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SizeChanged += (sender, args) =>
            {
                Clip = new RectangleGeometry {Rect = new Rect(0, 0, ActualWidth, ActualHeight)};
                Refresh(true);
            };

            _panel = GetTemplateChild(PartLinesPanel) as Panel;
            _containerRecycler = new LineContainerRecycler(_panel, this);

            UpdateVisibility();
        }

        protected void PrepareLine(Line line, LineInfo item)
        {
            if (line == null)
            {
                return;
            }

            var descriptor = item.Orientation == Orientation.Horizontal ? _horizontalDescriptor : _verticalDescriptor;
            line.Tag = item;
            line.Stroke = descriptor.LineStroke;

            var position = item.Position;

            //избегаем блюра
            var precisePosition = position + 0.5;

            if (descriptor.Orientation == Orientation.Horizontal)
            {
                line.X1 = descriptor.LineStart;
                line.Y1 = precisePosition;
                line.X2 = descriptor.LineEnd;
                line.Y2 = precisePosition;
            }
            else
            {
                line.X1 = precisePosition;
                line.Y1 = descriptor.LineStart;
                line.X2 = precisePosition;
                line.Y2 = descriptor.LineEnd;
            }
        }

        private static void OnIsGridVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schema = d as Schema;
            schema?.BackgroundGrid?.UpdateVisibility();
        }

        private void RedrawDynamic()
        {
            var actualWidth = ActualWidth;
            var actualHeight = ActualHeight;

            if (_panel == null || Schema == null || _containerRecycler == null ||
                !IsValidSize(actualWidth) || !IsValidSize(actualHeight) ||
                Visibility == Visibility.Collapsed)
            {
                return;
            }

            var viewport = Schema.Viewport;
            var zoom = Schema.Zoom;
            var cellSize = _cellSize;

            var offsetX = -Math.Round(viewport.Left*zoom, 0);
            var offsetY = -Math.Round(viewport.Top*zoom, 0);

            var translate = new TranslateTransform {X = offsetX, Y = offsetY};
            _panel.RenderTransform = translate;

            _horizontalDescriptor = new LineDescriptor
            {
                Orientation = Orientation.Horizontal,
                Interval = cellSize.Height,
                Zoom = zoom,
                Min = viewport.Top,
                Max = viewport.Bottom,
                LineStart = (viewport.Left - cellSize.Width)*zoom,
                LineEnd = (viewport.Right + cellSize.Width)*zoom,
                LineStroke = _lineStroke
            };

            _verticalDescriptor = new LineDescriptor
            {
                Orientation = Orientation.Vertical,
                Interval = cellSize.Width,
                Zoom = zoom,
                Min = viewport.Left,
                Max = viewport.Right,
                LineStart = (viewport.Top - cellSize.Height)*zoom,
                LineEnd = (viewport.Bottom + cellSize.Height)*zoom,
                LineStroke = _lineStroke
            };

            var items = CreateItems(_horizontalDescriptor).Union(CreateItems(_verticalDescriptor));

            _containerRecycler.Update(items);
        }

        private bool IsValidSize(double size)
        {
            return size > 0 && !double.IsInfinity(size) && !double.IsNaN(size);
        }

        private List<LineInfo> CreateItems(LineDescriptor descriptor)
        {
            var result = new List<LineInfo>();

            for (double itemValue = 0; itemValue <= descriptor.Max; itemValue += descriptor.Interval)
            {
                if (itemValue >= descriptor.Min)
                {
                    var info = new LineInfo
                    {
                        Position = Math.Round(itemValue*descriptor.Zoom, 0),
                        Orientation = descriptor.Orientation
                    };
                    result.Add(info);
                }
            }

            for (var itemValue = 0 - descriptor.Interval; itemValue >= descriptor.Min; itemValue -= descriptor.Interval)
            {
                if (itemValue <= descriptor.Max)
                {
                    var info = new LineInfo
                    {
                        Position = Math.Round(itemValue*descriptor.Zoom, 0),
                        Orientation = descriptor.Orientation
                    };
                    result.Add(info);
                }
            }
            return result;
        }

        private void Refresh(bool forceRefresh = false)
        {
            RedrawDynamic();
        }

        private void RedrawStatic(bool forceRefresh)
        {
            if (!forceRefresh)
            {
//                return;
            }

            var actualWidth = ActualWidth;
            var actualHeight = ActualHeight;

            if (_panel == null || Schema == null || _containerRecycler == null
                || !IsValidSize(actualWidth) || !IsValidSize(actualHeight) || Visibility == Visibility.Collapsed)
            {
                return;
            }

            var viewport = Schema.Viewport;
            var zoom = 1;
            var cellSize = _cellSize;

            var horizontalDescriptor = new LineDescriptor
            {
                Orientation = Orientation.Horizontal,
                Interval = cellSize.Height,
                Zoom = zoom,
                Min = viewport.Top,
                Max = viewport.Bottom,
                LineStart = 0,
                LineEnd = actualWidth,
                LineStroke = _lineStroke
            };
            _horizontalDescriptor = horizontalDescriptor;

            _verticalDescriptor = new LineDescriptor
            {
                Orientation = Orientation.Vertical,
                Interval = cellSize.Width,
                Zoom = zoom,
                Min = viewport.Left,
                Max = viewport.Right,
                LineStart = 0,
                LineEnd = actualHeight,
                LineStroke = _lineStroke
            };

            var items = CreateItems(_horizontalDescriptor).Union(CreateItems(_verticalDescriptor));

            _containerRecycler.Update(items);
        }

        private void UpdateVisibility()
        {
            if (Schema != null)
            {
                Visibility = GetIsGridVisible(Schema) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void SchemaOnViewportChanged(object sender, PropertyEventArgs<Rect> propertyEventArgs)
        {
            Refresh();
        }
    }
}