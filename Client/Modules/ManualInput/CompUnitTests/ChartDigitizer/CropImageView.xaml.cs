using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GazRouter.ManualInput.CompUnitTests.ChartDigitizer
{
    public partial class CropImageView
    {
        public CropImageView()
        {
            InitializeComponent();
            Img.ImageOpened += (obj, args) => SetCropperPosition(0, 0, Img.ActualWidth - 10, Img.ActualHeight - 10);

        }

        public BitmapImage ImageSource
        {
            get { return (BitmapImage)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSourceProperty", typeof(BitmapImage), typeof(CropImageView),
                                new PropertyMetadata(OnImageSourceChanged));

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CropImageView)d;
            control.Img.Source = (BitmapImage)e.NewValue;
            
            control.SetCropperPosition(100, 100, 300, 200);
        }

        public Rect CropRect
        {
            get { return (Rect)GetValue(CropRectProperty); }
            set { SetValue(CropRectProperty, value); }
        }

        public static readonly DependencyProperty CropRectProperty =
            DependencyProperty.Register("CropRectProperty", typeof(Rect), typeof(CropImageView),
                                new PropertyMetadata(null));

        
        private void SetCropperPosition(double x, double y, double w, double h)
        {
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (w < 0) w = 0;
            if (h < 0) h = 0;
            
            Drager.Data = new RectangleGeometry
            {
                Rect = new Rect(x, y, w, h)
            };

            Resizer.Data = new RectangleGeometry
            {
                Rect = new Rect(x + w, y + h, 10, 10)
            };

            Shader.Points = new PointCollection
            {
                new Point(0, 0),
                new Point(Img.ActualWidth, 0),
                new Point(Img.ActualWidth, Img.ActualHeight),
                new Point(0, Img.ActualHeight),
                new Point(0, y),
                new Point(x, y),
                new Point(x, y + h),
                new Point(x + w, y + h),
                new Point(x + w, y),
                new Point(0, y),
                new Point(0, 0)
            };

            var scale = ImageSource.PixelWidth / Img.ActualWidth;
            CropRect = new Rect(((int)x * scale), ((int)y * scale), ((int)w * scale), ((int)h * scale));
        }
        

        private void MoveCropper(double dx, double dy)
        {
            var rc = ((RectangleGeometry) Drager.Data).Rect;
            if (rc.Right + dx + 10 > Img.ActualWidth)
                dx = Img.ActualWidth - rc.Right - 10;
            if (rc.Bottom + dy + 10 > Img.ActualHeight)
                dy = Img.ActualHeight - rc.Bottom - 10;

            SetCropperPosition(rc.X + dx, rc.Y + dy, rc.Width, rc.Height);
        }

        private void ResizeCropper(double dw, double dh)
        {
            var rc = ((RectangleGeometry)Drager.Data).Rect;
            if (rc.Right + dw + 10 > Img.ActualWidth)
                dw = Img.ActualWidth - rc.Right - 10;
            if (rc.Bottom + dh + 10 > Img.ActualHeight)
                dh = Img.ActualHeight - rc.Bottom - 10;

            SetCropperPosition(rc.X, rc.Y, rc.Width + dw, rc.Height + dh);
        }



        private bool _isMousePressed;
        private Point _prevPos;

        private void OnDragerBtnDown(object sender, MouseButtonEventArgs e)
        {
            _isMousePressed = true;
            Drager.CaptureMouse();
            _prevPos = e.GetPosition(this);
        }

        private void OnDragerBtnUp(object sender, MouseButtonEventArgs e)
        {
            _isMousePressed = false;
            Drager.ReleaseMouseCapture();
        }

        private void OnDragerMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMousePressed)
            {
                var pos = e.GetPosition(this);
                MoveCropper(pos.X - _prevPos.X, pos.Y - _prevPos.Y);
                _prevPos = pos;
            }
        }


        private void OnResizerBtnDown(object sender, MouseButtonEventArgs e)
        {
            _isMousePressed = true;
            Resizer.CaptureMouse();
            _prevPos = e.GetPosition(this);
        }

        private void OnResizerBtnUp(object sender, MouseButtonEventArgs e)
        {
            _isMousePressed = false;
            Resizer.ReleaseMouseCapture();
        }

        private void OnResizerMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMousePressed)
            {
                var pos = e.GetPosition(this);
                ResizeCropper(pos.X - _prevPos.X, pos.Y - _prevPos.Y);
                _prevPos = pos;
            }
        }
        
    }
}
