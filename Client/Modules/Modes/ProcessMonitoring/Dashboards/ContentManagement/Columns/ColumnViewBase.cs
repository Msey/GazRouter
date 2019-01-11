using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Columns
{

    public abstract class ColumnViewBase : DataElementBase
    {
        private readonly TextBlock _title;
        protected double HeaderHeight;

        protected int RowCount { get; private set; }

        public override Visibility Visibility
        {
            get
            {
                return base.Visibility;
            }
            set
            {
                _title.Visibility = value;
                base.Visibility = value;
            }
        }


        protected ColumnViewBase(DashboardElementContainer dashboard, string title, int rowCount = 12, int fontSize = 11)
            :base (dashboard)
        {
            RowCount = rowCount;
            HeaderHeight = fontSize*3;

            // Заголовок столбца
            _title = new TextBlock
            {
                TextAlignment = TextAlignment.Center,
                FontFamily = new FontFamily("Segoe UI"),
                TextWrapping = TextWrapping.Wrap,
                FontSize = fontSize,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Black),
                Text = title
            };
            Dashboard.Canvas.Children.Add(_title);
        }

        public override void Destroy()
        {
            Dashboard.Canvas.Children.Remove(_title);
        }

        
        public override void UpdatePosition()
        {
            //Заголовок столбца
            _title.Width = Width;
            _title.Height = Height;
            //_title.Height = _title.FontSize * 3.5;
            Canvas.SetLeft(_title, Position.X);
            Canvas.SetTop(_title, Position.Y);
            Canvas.SetZIndex(_title, Z);
        }

        public virtual void UpdateData()
        {
        }
        
    }

}