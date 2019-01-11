using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GazRouter.Client.Menu
{
    public partial class DropMenuView : UserControl
    {
        private readonly Brush _hlb = new SolidColorBrush(Color.FromArgb(0xff, 0xf5, 0xf5, 0xf5));
        private readonly Brush _nb = new SolidColorBrush(Color.FromArgb(0xff, 0xf0, 0xef, 0xf1));
        private bool _isOnBorder;
        private bool _isOnPopup;

        public DropMenuView()
        {
            InitializeComponent();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            _isOnBorder = true;

            Update();
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _isOnBorder = false;

            Update();
        }

        private void Update()
        {
            if (_isOnBorder || _isOnPopup)
            {
                Border.Background = _hlb;
                Popup.IsOpen = true;
            }
            else
            {
                Border.Background = _nb;
                Popup.IsOpen = false;
            }
        }

        //public static readonly DependencyProperty ImageSourceProperty = 
        //    DependencyProperty.Register(
        //        "ImageSource", 
        //        typeof(ImageSource), 
        //        typeof(DropMenuView),
        //        new PropertyMetadata(null, OnImageSourcePropertyChanged));

        //public ImageSource ImageSource
        //{
        //    get { return GetValue(ImageSourceProperty) as ImageSource; }
        //    set { SetValue(ImageSourceProperty, value); }
        //}

        //private static void OnImageSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var ctrl = d as DropMenuView;
        //    if (ctrl?.ImageSource != null)
        //    {
        //        ctrl.Img.Source = ctrl.ImageSource;
        //    }
        //}

        //public static readonly DependencyProperty DropContentProperty =
        //    DependencyProperty.Register(
        //        "DropContent",
        //        typeof(ViewModelBase),
        //        typeof(DropMenuView),
        //        new PropertyMetadata(null, OnDropContentPropertyChanged));

        //public ViewModelBase DropContent
        //{
        //    get { return GetValue(DropContentProperty) as ViewModelBase; }
        //    set { SetValue(DropContentProperty, value); }
        //}

        //private static void OnDropContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var ctrl = d as DropMenuView;
        //    if (ctrl?.DropContent != null)
        //    {
        //        ctrl.CntCtrl.Content = ctrl.DropContent;
        //    }
        //}
    }
}