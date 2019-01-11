using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Misc
{

    public class TinyTrend
    {
        private readonly Path _back;
        private readonly Polyline _line;
        private readonly List<Line> _gridLines;
        private DashboardElementContainer _dashboard;
        

        public double Width { get; set; }
        public double Height { get; set; }


        public TinyTrend(DashboardElementContainer dashboard)
        {
            _dashboard = dashboard;

            // Фон для значения
            _back = new Path {
                Fill = new SolidColorBrush(Color.FromArgb(0xff, 0xdc, 0xdc, 0xdc)),
                Stroke = new SolidColorBrush(Colors.DarkGray),
                Data = new RectangleGeometry(),
                Effect = new DropShadowEffect {BlurRadius = 5, Color = Colors.Black, Direction = 315, Opacity = 0.5, ShadowDepth = 2},
                Visibility = Visibility.Collapsed
            };
            _dashboard.Canvas.Children.Add(_back);
            Canvas.SetZIndex(_back, 333333);

            _gridLines = new List<Line>();
            for (int i = 0; i < 12; i++)
            {
                var ln = new Line {
                                 StrokeThickness = 1,
                                 Stroke = new SolidColorBrush(Colors.Black),
                                 Visibility = Visibility.Collapsed,
                                 StrokeDashArray = {1, 1}
                             };
                _gridLines.Add(ln);
                _dashboard.Canvas.Children.Add(ln);
                Canvas.SetZIndex(ln, 333334);
            }

            _line = new Polyline {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Red),
                Visibility = Visibility.Collapsed,
            };
            _dashboard.Canvas.Children.Add(_line);
            Canvas.SetZIndex(_line, 333335);

            
        }
        private const int _margin = 5;

        public void Show(int x, int y, List<double> values, int selectedIndex)
        {
            _back.Visibility = Visibility.Visible;
            _line.Visibility = Visibility.Visible;

            // фон для элемента значений
            var geom = _back.Data as RectangleGeometry;
            geom.Rect = new Rect(x, y, Width, Height);
            
            var min = values.Min();
            min = min > 0 ? min - 0.1*min : min + min*0.1;
            var max = values.Max();
            max = max > 0 ? max + 0.1*max : max - 0.1*max;
            var xScale = Width / 12;
            var yScale = (Height - 2 * _margin) / (max - min);

            _line.Points.Clear();
            for (var i = 0; i < values.Count; i++)
            {
                var sx = x + i * xScale;
                var sy = y + Height - _margin - yScale * (values[i] - min);
                _gridLines[i].X1 = x + i * xScale;
                _gridLines[i].Y1 = y;
                _gridLines[i].X2 = x + i * xScale;
                _gridLines[i].Y2 = y + Height;
                _gridLines[i].Visibility = Visibility.Visible;
                _line.Points.Add(new Point(sx, sy));

                _gridLines[i].Stroke = new SolidColorBrush(i == selectedIndex ? Colors.Orange : Colors.LightGray);
            }
        }

        public void Hide()
        {
            _back.Visibility = Visibility.Collapsed;
            _line.Visibility = Visibility.Collapsed;
            for (int i = 0; i < 12; i++)
                _gridLines[i].Visibility = Visibility.Collapsed;
            
        }
    }

}