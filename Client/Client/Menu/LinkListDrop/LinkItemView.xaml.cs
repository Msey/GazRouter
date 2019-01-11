using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GazRouter.Common.GoodStyles;

namespace GazRouter.Client.Menu.LinkListDrop
{
    public partial class LinkItemView : UserControl
    {
        private readonly Brush _normalBrush = Brushes.Transparent;
        private readonly Brush _highlightBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xb0, 0xe0, 0xe6));

        public LinkItemView()
        {
            InitializeComponent();
        }


        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Border.Background = _highlightBrush;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Border.Background = _normalBrush;
        }
    }
}
