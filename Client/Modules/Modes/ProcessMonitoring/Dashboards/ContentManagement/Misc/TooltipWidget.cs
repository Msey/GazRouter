using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Misc
{
    /// <summary>
    /// Визуальный компонент для газопровода
    /// </summary>
    public class TooltipWidget
    {
        private readonly TextBlock _text;
        private readonly Path _box;
        private readonly Path _shade;

        private const int TextWidth = 200;

        private const int Margin = 5;

        private const int Offset = 10;

        public TooltipWidget(DashboardElementContainer db)
        {
            _text = new TextBlock
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    //Foreground = new SolidColorBrush(ColorConverter.FromHex(0xfff5f5f5)),
                    FontSize = 11,
                    FontWeight = FontWeights.Bold,
                    Width = TextWidth,
                    TextWrapping = TextWrapping.Wrap
                };
            db.Canvas.Children.Add(_text);
            Canvas.SetZIndex(_text, 282828 + 1);
            _text.Visibility = Visibility.Collapsed;

            _box = new Path
                {
                    Fill = new SolidColorBrush(Color.FromArgb(255, 255, 215, 0)),
                    StrokeThickness = 0,
                };
            db.Canvas.Children.Add(_box);
            Canvas.SetZIndex(_box, 282828);
            _box.Visibility = Visibility.Collapsed;

            _shade = new Path
                {
                    Fill = new SolidColorBrush(Color.FromArgb(255,184,134,11)),
                    StrokeThickness = 0
                };
            db.Canvas.Children.Add(_shade);
            Canvas.SetZIndex(_shade, 282828 - 1);
            _shade.Visibility = Visibility.Collapsed;

        }

       
        public void Show(Point pos, string text)
        {
            _text.Visibility = Visibility.Visible;
            _box.Visibility = Visibility.Visible;
            _shade.Visibility = Visibility.Visible;

            _text.Text = text;

            pos = new Point(pos.X, pos.Y - Offset - (_text.ActualHeight + 2 * Margin));
            if (pos.Y < 0) 
            { 
                pos.Y = 0;
                pos.X += 15;
            }

            Canvas.SetLeft(_text, pos.X + Margin);
            Canvas.SetTop(_text, pos.Y + Margin);
            
            var geom = new RectangleGeometry();
            _box.Data = geom;
            geom.Rect = new Rect(
                pos.X, 
                pos.Y, 
                _text.ActualWidth + 2 * Margin, 
                _text.ActualHeight + 2 * Margin);
            
            var geom2 = new RectangleGeometry();
            _shade.Data = geom2;
            geom2.Rect = new Rect(
                pos.X, 
                pos.Y, 
                _text.ActualWidth + 2*Margin + 2,
                _text.ActualHeight + 2*Margin + 2);


        }
    }

}