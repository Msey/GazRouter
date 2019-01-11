using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GazRouter.ManualInput.CompUnitTests.ChartDigitizer
{
    public partial class InterpolateView
    {
        public InterpolateView()
        {
            InitializeComponent();
            Img.ImageOpened += (obj, args) =>
            {
                
                MoveLimit(XMin, 0, 0);
                MoveLimit(XMax, Img.ActualWidth, 0);
                MoveLimit(YMin, 0, Img.ActualHeight);
                MoveLimit(YMax, 0, 0);
            };
        }

        public BitmapImage ImageSource
        {
            get { return (BitmapImage)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSourceProperty", typeof(BitmapImage), typeof(InterpolateView),
                                new PropertyMetadata(OnImageSourceChanged));

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (InterpolateView)d;
            control.Img.Source = (BitmapImage)e.NewValue;
        }




        private double _xMinPos;
        private double _xMaxPos;
        private double _yMinPos;
        private double _yMaxPos;

        private bool _isMousePressed;
        private Point _prevPos;

        private void OnDragerBtnDown(object sender, MouseButtonEventArgs e)
        {
            _isMousePressed = true;
            _prevPos = e.GetPosition(this);
            var limit = (Polygon) sender;
            limit.CaptureMouse();
        }

        private void OnDragerBtnUp(object sender, MouseButtonEventArgs e)
        {
            _isMousePressed = false;
            var limit = (Polygon)sender;
            limit.ReleaseMouseCapture();
        }

        private void OnDragerMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMousePressed)
            {
                var pos = e.GetPosition(this);
                var limit = (Polygon)sender;
                MoveLimit(limit, pos.X - _prevPos.X, pos.Y - _prevPos.Y);
                _prevPos = pos;
            }
        }


        private void MoveLimit(Polygon limit, double dx, double dy)
        {
            if (limit.Name == "XMin")
            {
                var pos = _xMinPos + dx;
                if (pos > _xMaxPos) pos = _xMaxPos - 10;
                if (pos < 0) pos = 0;
                DrawLimit(limit, pos);
                _xMinPos = pos;
            }

            if (limit.Name == "XMax")
            {
                var pos = _xMaxPos + dx;
                if (pos < _xMinPos) pos = _xMinPos + 10;
                if (pos > Img.ActualWidth) pos = Img.ActualWidth;
                DrawLimit(limit, pos);
                _xMaxPos = pos;
            }

            if (limit.Name == "YMin")
            {
                var pos = _yMinPos + dy;
                if (pos < _yMaxPos) pos = _yMaxPos + 10;
                if (pos > Img.ActualHeight) pos = Img.ActualHeight;
                DrawLimit(limit, pos);
                _yMinPos = pos;
            }

            if (limit.Name == "YMax")
            {
                var pos = _yMaxPos + dy;
                if (pos > _yMinPos) pos = _yMinPos - 10;
                if (pos < 0) pos = 0;
                DrawLimit(limit, pos);
                _yMaxPos = pos;
            }
        }

        private void DrawLimit(Polygon limit, double pos)
        {
            const int size = 10;

            if (limit.Name == "XMin" || limit.Name == "XMax")
            {
                limit.Points = new PointCollection
                {
                    new Point(pos - 0.5 * size, Img.ActualHeight + 1.5 * size),
                    new Point(pos - 0.5 * size, Img.ActualHeight + 0.5 * size),
                    new Point(pos - 0.5, Img.ActualHeight),
                    new Point(pos - 0.5, 0),
                    new Point(pos + 0.5, 0),
                    new Point(pos + 0.5, Img.ActualHeight),
                    new Point(pos + 0.5 * size, Img.ActualHeight + 0.5 * size),
                    new Point(pos + 0.5 * size, Img.ActualHeight + 1.5 * size),
                };
            }

            if (limit.Name == "YMin" || limit.Name == "YMax")
            {
                limit.Points = new PointCollection
                {
                    new Point(Img.ActualWidth + 1.5 * size, pos - 0.5 * size),
                    new Point(Img.ActualWidth + 0.5 * size, pos - 0.5 * size),
                    new Point(Img.ActualWidth, pos - 0.5),
                    new Point(0, pos - 0.5),
                    new Point(0, pos + 0.5),
                    new Point(Img.ActualWidth, pos + 0.5),
                    new Point(Img.ActualWidth + 0.5 * size, pos + 0.5 * size),
                    new Point(Img.ActualWidth + 1.5 * size, pos + 0.5 * size),
                };
            }
        }


        
    }
}
