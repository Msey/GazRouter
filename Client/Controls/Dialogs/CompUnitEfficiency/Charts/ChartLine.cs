using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Telerik.Windows.Controls.ChartView;

namespace GazRouter.Controls.Dialogs.CompUnitEfficiency.Charts
{
    public class ChartPoint
    {
        /// <summary>
        /// Объемная производительность, м3/мин
        /// </summary>
        public double? Pumping { get; set; }

        /// <summary>
        /// Степень сжатия
        /// </summary>
        public double? CompressionRatio { get; set; }

        /// <summary>
        /// Политропный КПД
        /// </summary>
        public double? Efficiency { get; set; }

        /// <summary>
        /// Внутренняя мощность, кВт
        /// </summary>
        public double? Power { get; set; }

        /// <summary>
        /// Относительные обороты
        /// </summary>
        public double? Rpm { get; set; }

    }

    public class ChartLine
    {
        public ChartLine()
        {
            Points = new List<ChartPoint>();
            PointSize = 6;
        }

        /// <summary>
        /// Точки кривой
        /// </summary>
        public List<ChartPoint> Points { get; set; }
        
        /// <summary>
        /// Относительные обороты
        /// </summary>
        public double? Rpm { get; set; }

        /// <summary>
        /// Цвет линии
        /// </summary>
        public Color LineColor { get; set; }
        
        /// <summary>
        /// Цвет точки
        /// </summary>
        public Color? PointColor { get; set; }
        
        /// <summary>
        /// Кривая границы зоны помпажа
        /// </summary>
        public bool IsRedLine { get; set; }

        /// <summary>
        /// Кривая, для которой выводятся значения КПД на графике
        /// </summary>
        public bool IsEfficiencyLabel { get; set; }

        /// <summary>
        /// Кривая, для которой выводятся значения отн оборотов на графике
        /// </summary>
        public bool IsNNomLabel { get; set; }

        /// <summary>
        /// Кривая,  на которой находится раб точка
        /// </summary>
        public bool IsRatedLine { get; set; }

        public string XValue = "Pumping";
        public string YValue = "CompressionRatio";
        
        /// <summary>
        /// Размер точки
        /// </summary>
        public int PointSize { get; set; }

        public ScatterLineSeries Series
        {
            get
            {
                DataTemplate pointTemplate = null;

                if (PointColor.HasValue)
                {
                    var tmpl = new StringBuilder();
                    tmpl.Append(@"<DataTemplate                     
                                    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">");
                    tmpl.AppendFormat("<Ellipse Height=\"{0}\" Width=\"{1}\" Fill=\"{2}\" />", PointSize, PointSize, PointColor);
                    tmpl.Append("</DataTemplate>");

                    pointTemplate = XamlReader.Load(tmpl.ToString()) as DataTemplate;
                    
                }

                if (IsRedLine)
                {
                    return new ScatterLineSeries
                    {
                        ItemsSource = Points,
                        XValueBinding = new PropertyNameDataPointBinding(XValue),
                        YValueBinding = new PropertyNameDataPointBinding(YValue),
                        Stroke = new SolidColorBrush(LineColor),
                        ClipToPlotArea = false,
                        PointTemplate = pointTemplate
                    };
                }

                var t1 = new ScatterSplineSeries
                {
                    ItemsSource = Points,
                    XValueBinding = new PropertyNameDataPointBinding(XValue),
                    YValueBinding = new PropertyNameDataPointBinding(YValue),
                    Stroke = new SolidColorBrush(LineColor),
                    ClipToPlotArea = false,
                    PointTemplate = pointTemplate, 
                    ShowLabels = true
                };

                var margin = new Thickness();
                if (IsEfficiencyLabel)
                    margin = new Thickness(19, 18, 0, 0);
                if (IsNNomLabel)
                    margin = new Thickness(0, 0, 0, 5);
                if (IsRatedLine)
                    margin = new Thickness(0, 0, 0, 20); 

                t1.LabelDefinitions.Add(new ChartSeriesLabelDefinition 
                                            { 
                                                Strategy =
                                                    new LabelStrategy
                                                        {
                                                            IsKpdLabel = 
                                                                IsEfficiencyLabel,
                                                            IsNnomLabel = IsNNomLabel,
                                                            IsRatedLine = IsRatedLine,
                                                            CountOfCollection = Points.Count
                                                        },
                                                Margin = margin
                                            });

                return t1;
            }
        }

    }

    public class LabelStrategy : ChartSeriesLabelStrategy
    {
        public override LabelStrategyOptions Options
        {
            get { return LabelStrategyOptions.DefaultVisual; }
        }

        public bool IsKpdLabel { get; set; }
        public bool IsNnomLabel { get; set; }
        public bool IsRatedLine { get; set; }
        public int CountOfCollection { get; set; }
        
        public override FrameworkElement CreateDefaultVisual(Telerik.Charting.DataPoint point, int labelIndex)
        {
            var t1 = point.DataItem as ChartPoint;
            if (t1 == null) return null;

            var text = "";
            if (IsKpdLabel)
            {
                var subtext = "";
                if (point.CollectionIndex == CountOfCollection/2)
                {
                    subtext = "hпол=";
                }
                text = t1.Efficiency.HasValue ? subtext + t1.Efficiency.Value.ToString("N3") : "";
            }
                

            if (IsNnomLabel)
                text = t1.Rpm.HasValue ? t1.Rpm.Value.ToString("N2") : "";

            if (IsRatedLine)
                text = t1.Rpm.HasValue ? "N/Nnom=" + t1.Rpm.Value.ToString("N2") : "";

            return new Border
            {
                Child = new TextBlock
                {
                    Foreground = new SolidColorBrush(Colors.Black),
                    Text = text
                }
            };
        }
    }
    
}
