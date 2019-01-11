using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Columns
{

    public abstract class ColumnFieldBase : DataElementBase
    {
        private readonly Path _back;
        private readonly Brush _backColor;
        private readonly int _serieIndex;

        protected TextBlock ValueTextBlock { get; set; }
        
        public string Value
        {
            get { return ValueTextBlock.Text; }
            set
            {
                ValueTextBlock.Text = value;
            }
        }

        public TextAlignment TextAlign
        {
            get { return ValueTextBlock.TextAlignment; }
            set { ValueTextBlock.TextAlignment = value; }
        }

        public override Visibility Visibility
        {
            get { return ValueTextBlock.Visibility; }
            set 
            { 
                ValueTextBlock.Visibility = value;
                _back.Visibility = value;
            }
        }

        public int SerieIndex
        {
            get { return _serieIndex; }
        }


        protected ColumnFieldBase(DashboardElementContainer dashboard, int serieIndex, int fontSize = 11)
            : base(dashboard)
        {
            _serieIndex = serieIndex;

            // Фон для значения
            _back = new Path
            {
                Data = new RectangleGeometry(),
                Fill = _backColor = new SolidColorBrush(_serieIndex%2 == 0
                    ? Color.FromArgb(0xff, 0xf8, 0xf8, 0xff)
                    : Color.FromArgb(0xff, 0xdc, 0xdc, 0xdc)),

                StrokeThickness = 0
            };
            Dashboard.Canvas.Children.Add(_back);

            // 1. Текущее значение
            ValueTextBlock = new TextBlock
            {
                TextAlignment = TextAlignment.Center,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = fontSize,
                FontWeight = FontWeights.Light,
                Foreground = new SolidColorBrush(Colors.Black),
                Text = "X"
            };
            Dashboard.Canvas.Children.Add(ValueTextBlock);
            ValueTextBlock.MouseLeftButtonUp += OnMouseLeftButtonUp;
            ValueTextBlock.MouseEnter += OnMouseEnter;
            ValueTextBlock.MouseLeave += OnMouseLeave;
            ValueTextBlock.MouseRightButtonUp += NotifyMouseRightButtonUp;
        }

        public MouseEventHandler MouseLeave;
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (MouseLeave != null)
                MouseLeave(this, e);
        }

        public MouseEventHandler MouseEnter;
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if(MouseEnter != null)
                MouseEnter(this, e);
        }

        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Dashboard.SelectedSerieIndex = _serieIndex;
            Select();
            NotifyMouseLeftButtonUp(this, e);
        }

        public override void Destroy()
        {
            ValueTextBlock.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            ValueTextBlock.MouseEnter -= OnMouseEnter;
            ValueTextBlock.MouseLeave -= OnMouseLeave;

            Dashboard.Canvas.Children.Remove(_back);
            Dashboard.Canvas.Children.Remove(ValueTextBlock);
        }
        
        public override void UpdatePosition()
        {
            // фон для элемента значений
            var geom = (RectangleGeometry)_back.Data;
            geom.Rect = new Rect(Position.X, Position.Y, Width, Height);
            Canvas.SetZIndex(_back, Z);

            // 1. Текущее значение
            ValueTextBlock.Width = Width;
            ValueTextBlock.Height = Height;
            Canvas.SetLeft(ValueTextBlock, Position.X);
            Canvas.SetTop(ValueTextBlock, Position.Y);
            Canvas.SetZIndex(ValueTextBlock, Z + 1);

            if (_serieIndex == Dashboard.SelectedSerieIndex) Select();
            else Deselect();
        }
        
        public override bool Select()
        {
            _back.Fill = new SolidColorBrush(Color.FromArgb(0xff, 0xb0, 0xc4, 0xde));
            ValueTextBlock.FontWeight = FontWeights.Bold;

            return true;
        }

        public override void Deselect()
        {
            _back.Fill = _backColor;
            ValueTextBlock.FontWeight = FontWeights.Light;
        }

        public virtual void UpdateData()
        {
        }

    }

}