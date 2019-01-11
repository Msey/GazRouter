using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Elements
{
    public abstract class BoxedElementView<T> : ElementViewBase where T : BoxedElementModel
    {
        // Перетаскивание
        private readonly Polygon _drager;

        // Изменение размера
        private readonly Polygon _resizer;

        // Выделение
        private readonly Polyline _selection;
        // Фоновый элемент, сам прямоугольник дашборда
        private readonly Path _box;
        private bool _mouseButtonPressed;
        private Point _captureOffset;

        protected BoxedElementView(T elementModel, DashboardElementContainer dashboard,
            bool bDragEnable = false, bool bResizeEnable = false, bool bSelectEnable = false, bool bHasBox = false)
            : base(dashboard, elementModel)
        {
            Dashboard.AddElement(this);

            if (bHasBox)
            {
                _box = new Path
                {
                    StrokeThickness = 0,
                    Fill = new SolidColorBrush(Color.FromArgb(0xff, 0xdc, 0xdc, 0xdc)),
                    Effect = new DropShadowEffect
                    {
                        BlurRadius = 5,
                        Direction = 315,
                        ShadowDepth = 2,
                        Opacity = 0.5,
                        Color = Colors.Black
                    }
                };
                Dashboard.Canvas.Children.Add(_box);

                //_titleBox = new Path
                //{
                //    StrokeThickness = 0,
                //    Fill = new SolidColorBrush(Color.FromArgb(0xff, 0x2d, 0x9c, 0xd0)),
                //};
                //_dashboard.Canvas.Children.Add(_titleBox);
            }

            if (bDragEnable)
            {
                _drager = new Polygon
                {
                    Opacity = 0,
                    StrokeThickness = 0,
                    Fill = new SolidColorBrush(Colors.Black)
                };
                Dashboard.Canvas.Children.Add(_drager);
                _drager.MouseLeftButtonDown += m_drager_MouseLeftButtonDown;
                _drager.MouseLeftButtonUp += m_drager_MouseLeftButtonUp;
                _drager.MouseMove += m_drager_MouseMove;
                _drager.MouseRightButtonUp += OnBoxOnMouseRightButtonUp;
                Canvas.SetZIndex(_drager, 11111);
            }

            if (bResizeEnable)
            {
                _resizer = new Polygon
                {
                    StrokeThickness = 0,
                    Fill = new SolidColorBrush(Color.FromArgb(0xff, 0xdc, 0x14, 0x3c)),
                    Visibility = Visibility.Collapsed,
                    Points = new PointCollection
                    {
                        new Point(0, 0),
                        new Point(10, 0),
                        new Point(10, 10),
                        new Point(0, 10)
                    }
                };
                Dashboard.Canvas.Children.Add(_resizer);
                _resizer.MouseLeftButtonDown += m_resizer_MouseLeftButtonDown;
                _resizer.MouseLeftButtonUp += m_resizer_MouseLeftButtonUp;
                _resizer.MouseMove += m_resizer_MouseMove;
                Canvas.SetZIndex(_resizer, 11112);
            }

            if (bSelectEnable)
            {
                _selection = new Polyline
                {
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Color.FromArgb(0xff, 0xdc, 0x14, 0x3c)),
                    StrokeDashArray = new DoubleCollection {1, 1},
                    Visibility = Visibility.Collapsed
                };
                Dashboard.Canvas.Children.Add(_selection);
                Canvas.SetZIndex(_selection, 11113);
            }
        }

        /// <summary>
        ///     Объект модели элемента
        /// </summary>
        public new T ElementModel => (T) base.ElementModel;

        public override void UpdatePosition()
        {
            if (_box != null)
            {
                _box.Data = new RectangleGeometry {Rect = new Rect(Position.X, Position.Y, Width, Height)};
                Canvas.SetZIndex(_box, Z);

                _box.Visibility = ElementModel.IsBoxVisible ? Visibility.Visible : Visibility.Collapsed;

                //_titleBox.Layout = new RectangleGeometry { Rect = new Rect(X, Y, Width, 30) };
                //Canvas.SetZIndex(_titleBox, Z + 1);
            }

            if (_drager != null)
            {
                _drager.Points = new PointCollection
                {
                    new Point(0, 0),
                    new Point(Width, 0),
                    new Point(Width, Height),
                    new Point(0, Height)
                };
                Canvas.SetLeft(_drager, Position.X);
                Canvas.SetTop(_drager, Position.Y);
                _drager.Visibility = Dashboard.IsEditMode ? Visibility.Visible : Visibility.Collapsed;
            }

            if (_resizer != null)
            {
                Canvas.SetLeft(_resizer, Position.X + Width);
                Canvas.SetTop(_resizer, Position.Y + Height);
            }

            if (_selection != null)
            {
                _selection.Points = new PointCollection
                {
                    new Point(0, 0),
                    new Point(Width, 0),
                    new Point(Width, Height),
                    new Point(0, Height),
                    new Point(0, 0)
                };
                Canvas.SetLeft(_selection, Position.X);
                Canvas.SetTop(_selection, Position.Y);
            }

            if (!Dashboard.IsEditMode)
            {
                Deselect();
            }
        }

        public override void Move(double xOffset, double yOffset)
        {
            Position = new Point(Position.X + xOffset, Position.Y + yOffset);
        }

        public override bool Select()
        {
            if (_selection == null)
            {
                return false;
            }
            if (_resizer != null)
            {
                _resizer.Visibility = Visibility.Visible;
            }

            _selection.Visibility = Visibility.Visible;

            return true;
        }

        public override void Deselect()
        {
            if (_resizer != null)
            {
                _resizer.Visibility = Visibility.Collapsed;
            }

            if (_selection != null)
            {
                _selection.Visibility = Visibility.Collapsed;
            }
        }

        public override void Destroy()
        {
            if (_resizer != null)
            {
                _resizer.MouseLeftButtonDown -= m_resizer_MouseLeftButtonDown;
                _resizer.MouseLeftButtonUp -= m_resizer_MouseLeftButtonUp;
                _resizer.MouseMove -= m_resizer_MouseMove;
                Dashboard.Canvas.Children.Remove(_resizer);
            }

            if (_selection != null)
            {
                Dashboard.Canvas.Children.Remove(_selection);
            }

            if (_drager != null)
            {
                _drager.MouseLeftButtonDown -= m_drager_MouseLeftButtonDown;
                _drager.MouseLeftButtonUp -= m_drager_MouseLeftButtonUp;
                _drager.MouseMove -= m_drager_MouseMove;
                _drager.MouseRightButtonUp -= OnBoxOnMouseRightButtonUp;
                Dashboard.Canvas.Children.Remove(_drager);
            }

            if (_box != null)
            {
                Dashboard.Canvas.Children.Remove(_box);
            }
        }

        private void m_drager_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseButtonPressed)
            {
                var offset = new Point(
                    e.GetPosition(Dashboard.Canvas).X - Position.X - _captureOffset.X,
                    e.GetPosition(Dashboard.Canvas).Y - Position.Y - _captureOffset.Y);

                NotifyElementMove(this, offset);
            }
        }

        private void m_drager_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _drager.ReleaseMouseCapture();
            _mouseButtonPressed = false;
            Dashboard.EndDragElements();

            NotifyMouseLeftButtonUp(this, e);
        }

        private void m_drager_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _drager.CaptureMouse();
            _mouseButtonPressed = true;

            _captureOffset = new Point(
                e.GetPosition(_drager).X,
                e.GetPosition(_drager).Y);

            NotifyMouseLeftButtonDown(this, e);

            Dashboard.StartDragElements();
        }

        private void m_resizer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseButtonPressed)
            {
                var pos = e.GetPosition(null);
                Width += pos.X - _captureOffset.X;
                Height += pos.Y - _captureOffset.Y;
                _captureOffset = pos;
            }
        }

        private void m_resizer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _resizer.ReleaseMouseCapture();
            _mouseButtonPressed = false;
        }

        private void m_resizer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _resizer.CaptureMouse();
            _mouseButtonPressed = true;

            _captureOffset = e.GetPosition(null);
        }

        private void OnBoxOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            NotifyMouseRightButtonUp(this, e);
        }
    }
}