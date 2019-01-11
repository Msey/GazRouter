using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Columns
{

    public class ColumnValueField : ColumnFieldBase
    {
        private Path _badDataBox;
        private readonly int _precision;
        
        private BasePropertyValueDTO _measuring;
        public BasePropertyValueDTO Measuring
        {
            get { return _measuring; }
            set
            {
                _measuring = value;
                UpdateData();
            }
        }

        public ColumnValueField(DashboardElementContainer dashboard, int serieIndex, int fontSize, int precision)
            : base(dashboard, serieIndex, fontSize)
        {
            _precision = precision;
        }

        public override void UpdateData()
        {
            if (Measuring != null)
            {
                if (Measuring is PropertyValueDoubleDTO)
                {
                    var dbl = Measuring as PropertyValueDoubleDTO;
                    Value = dbl.Value.ToString("n" + _precision);

                    if (dbl.QualityCode != QualityCode.Good)
                    {
                        _badDataBox = new Path
                        {
                            Fill = new SolidColorBrush(Colors.Orange),
                            Data = new RectangleGeometry
                            {
                                Rect = new Rect(Position.X, Position.Y, ValueTextBlock.Width, ValueTextBlock.Height)
                            }
                        };
                        Dashboard.Canvas.Children.Add(_badDataBox);
                        Canvas.SetZIndex(_badDataBox, Z);
                    }
                    else
                    {
                        if (_badDataBox != null)
                            Dashboard.Canvas.Children.Remove(_badDataBox);
                    }
                }
                else if (Measuring is PropertyValueStringDTO)
                {
                    var v = Measuring as PropertyValueStringDTO;
                    Value = v.Value;
                }
                else
                {
                    Value = "x";
                }
            }

            //base.UpdateData();
        }

        
    }

}