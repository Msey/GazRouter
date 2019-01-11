using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GazRouter.Flobus.Extensions;


namespace GazRouter.Flobus.FloScheme.Misc
{
    /// <summary>
    /// Визуальный компонент для газопровода
    /// </summary>
    public class AnnotationWidget
    {
        private readonly TextBlock _text;
        private readonly Path _box;
        private readonly Line _line;

        private const int TextWidth = 200;
        private const int Margin = 5;

        private Point _pos;
        private Point _anchor;
        public Point Anchor
        {
            get { return _anchor; }
            set 
            { 
                _anchor = value;
                _pos = new Point(value.X + 20, value.Y + 60);
                Update();
            }
        }

        private bool _mousePressed;
        private Size _mouseClickOffset;
        private readonly Schema _schema;



        public AnnotationWidget(Schema scm, Point anchor, string txt)
        {
            _schema = scm;
            _anchor = anchor;
            
            _line = new Line
                        {
                            Stroke = new SolidColorBrush(FloColors.Red),
                            StrokeThickness = 1,
                            StrokeDashArray = {1, 2},
                        };
            _schema.AddItemToCanvas(_line, (int)WidgetZOrder.Max - 1);
           

            _text = new TextBlock
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 11,
                    FontWeight = FontWeights.Bold,
                    Width = TextWidth,
                    TextWrapping = TextWrapping.Wrap,
                    Text = txt
                };
            _schema.AddItemToCanvas(_text,(int)WidgetZOrder.Max + 1);
            _text.MouseLeftButtonDown += BoxMouseLeftButtonDown;
            _text.MouseLeftButtonUp += BoxMouseLeftButtonUp;
            _text.MouseMove += BoxMouseMove;

            _box = new Path
                {
                    Fill = new SolidColorBrush(Colors.White),
                    StrokeThickness = 1,
                    StrokeDashArray = {1, 2},
                    Stroke = new SolidColorBrush(FloColors.Red),
                };
            _schema.AddItemToCanvas(_box, (int)WidgetZOrder.Max);
            _box.MouseLeftButtonDown += BoxMouseLeftButtonDown;
            _box.MouseLeftButtonUp += BoxMouseLeftButtonUp;
            _box.MouseMove += BoxMouseMove;
            

            Update();
        }

        private void Update()
        {
          
            _text.SetLocation(_pos.X + Margin,_pos.Y + Margin);
            var geom = new RectangleGeometry();
            _box.Data = geom;
            geom.Rect = new Rect(
                _pos.X,
                _pos.Y,
                TextWidth + 2 * Margin,
                _text.ActualHeight + 2 * Margin);

            _line.X1 = _pos.X + Margin + TextWidth / 2.0;
            _line.Y1 = _pos.Y + Margin + _text.ActualHeight / 2;
            _line.X2 = _anchor.X;
            _line.Y2 = _anchor.Y;
        }

       
        public Visibility Visibility
        {
            get { return _line.Visibility; }
            set
            {
                _line.Visibility = value;
                _box.Visibility = value;
                _text.Visibility = value;
            }
        }

        public void Destroy()
        {
            _schema.RemoveItemFromCanvas(_line);
            _schema.RemoveItemFromCanvas(_box);
            _schema.RemoveItemFromCanvas(_text);
        }
       
        void BoxMouseMove(object sender, MouseEventArgs e)
        {
            if (!_mousePressed) return;

            var pos = _schema.GetPositonOnCanvas(e);
            // Запретить двигать элемент за пределы схемы
            if (pos.X < 0) pos.X = 0;
            if (pos.Y < 0) pos.Y = 0;

            _pos = new Point(pos.X - _mouseClickOffset.Width, pos.Y - _mouseClickOffset.Height);
            Update();
        }

        void BoxMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _box.ReleaseMouseCapture();
            _mousePressed = false;
        }

        void BoxMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = _pos;
            var position = _schema.GetPositonOnCanvas(e);
            _mouseClickOffset =
                new Size(
                    position.X - pos.X,
                    position.Y - pos.Y);
            _box.CaptureMouse();
            _mousePressed = true;

            //Select();
        }
        
    }

}