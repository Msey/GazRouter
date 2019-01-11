using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;
using Telerik.Windows;
using PropertyMetadata = System.Windows.PropertyMetadata;

namespace GazRouter.ObjectModel.Model.Tabs.Segments
{
    public partial class SegmentViewer : UserControl
    {
        public SegmentViewer()
        {
            InitializeComponent();
            Scroll.SizeChanged += (obj, args) =>  Update();
            
        }
        

        public static readonly DependencyProperty PipelineProperty =
            DependencyProperty.Register("Pipeline", typeof(PipelineDTO), typeof(SegmentViewer),
                new PropertyMetadata(null, OnPropertyChanged));

        
        public static readonly DependencyProperty SegmentListProperty =
            DependencyProperty.Register("SegmentList", typeof(List<BaseSegmentDTO>), typeof (SegmentViewer),
                new PropertyMetadata(null, OnPropertyChanged));


        public static readonly DependencyProperty SelectedSegmentProperty =
            DependencyProperty.Register("SelectedSegmentList", typeof(BaseSegmentDTO), typeof(SegmentViewer),
                new PropertyMetadata(null, OnPropertyChanged));


        public static readonly DependencyProperty HighlightGapsProperty =
            DependencyProperty.Register("HighlightGaps", typeof(bool), typeof(SegmentViewer),
                new PropertyMetadata(true, OnPropertyChanged));

        public static readonly DependencyProperty HighlightOverlapsProperty =
            DependencyProperty.Register("HighlightOverlaps", typeof(bool), typeof(SegmentViewer),
                new PropertyMetadata(true, OnPropertyChanged));


        public static readonly DependencyProperty EditSegmentProperty =
            DependencyProperty.Register("EditSegment", typeof(ICommand), typeof(SegmentViewer),
                new PropertyMetadata(null, OnPropertyChanged));


        public static readonly DependencyProperty DeleteSegmentProperty =
            DependencyProperty.Register("DeleteSegment", typeof(ICommand), typeof(SegmentViewer),
                new PropertyMetadata(null, OnPropertyChanged));


        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SegmentViewer)d;
            control.Update();
        }
        
        /// <summary>
        /// Газопровод
        /// </summary>
        public PipelineDTO Pipeline
        {
            get { return (PipelineDTO)GetValue(PipelineProperty); }
            set {SetValue(PipelineProperty, value);}
        }

        /// <summary>
        /// Список сегментов
        /// </summary>
        public List<BaseSegmentDTO> SegmentList
        {
            get { return (List<BaseSegmentDTO>)GetValue(SegmentListProperty); }
            set { SetValue(SegmentListProperty, value); }
        }

        /// <summary>
        /// Выбранный сегмент
        /// </summary>
        public BaseSegmentDTO SelectedSegment
        {
            get { return (BaseSegmentDTO)GetValue(SelectedSegmentProperty); }
            set { SetValue(SelectedSegmentProperty, value); }
        }

        /// <summary>
        /// Искать и подсвечивать дырки между сегментами
        /// </summary>
        public bool HighlightGaps
        {
            get { return (bool)GetValue(HighlightGapsProperty); }
            set { SetValue(HighlightGapsProperty, value); }
        }

        /// <summary>
        /// Искать и подсвечивать пересечение сегментов
        /// </summary>
        public bool HighlightOverlaps
        {
            get { return (bool)GetValue(HighlightOverlapsProperty); }
            set { SetValue(HighlightOverlapsProperty, value); }
        }

        /// <summary>
        /// Изменить выбранный сегмент
        /// </summary>
        public ICommand EditSegment
        {
            get { return (ICommand)GetValue(EditSegmentProperty); }
            set { SetValue(EditSegmentProperty, value); }
        }

        /// <summary>
        /// Изменить выбранный сегмент
        /// </summary>
        public ICommand DeleteSegment
        {
            get { return (ICommand)GetValue(DeleteSegmentProperty); }
            set { SetValue(DeleteSegmentProperty, value); }
        }


        private void Update()
        {
            if (Pipeline == null || SegmentList == null) return;
            if (SegmentList.Count > 0 && SegmentList.Any(s => s.PipelineId != Pipeline.Id)) return;

            Cnv.Children.Clear();
            
            var chartWidth = Scroll.ViewportWidth;
            var chartHeight = Scroll.ViewportHeight;
            if (chartWidth == 0 || chartHeight == 0) return;

            var margin = 40;

            var kmLine = new Rectangle
            {
                Fill = new SolidColorBrush(Colors.DarkGray),
                StrokeThickness = 0,
                Width = chartWidth,
                Height = 20
            };
            Canvas.SetLeft(kmLine, 0);
            Canvas.SetTop(kmLine, 0);
            Cnv.Children.Add(kmLine);

            var xScale = (chartWidth - 2*margin) / Pipeline.Length;


            // отрисовка сегментов
            var yPos = 60;
            foreach (var segment in SegmentList)
            {
                var bar = new Rectangle
                {
                    Fill = new SolidColorBrush(segment == SelectedSegment ? Colors.DarkGray : Color.FromArgb(0xff, 0x25, 0xa0, 0xda)),
                    StrokeThickness = 0,
                    Width = (segment.KilometerOfEndPoint - segment.KilometerOfStartPoint) * xScale,
                    Height = 20
                };
                bar.MouseLeftButtonUp += (sender, args) => { SelectedSegment = segment; };
                bar.MouseRightButtonDown += (sender, args) =>
                {
                    SelectedSegment = segment;
                };
                Canvas.SetLeft(bar, (segment.KilometerOfStartPoint - Pipeline.KilometerOfStartPoint) * xScale + margin);
                Canvas.SetTop(bar, yPos);
                Canvas.SetZIndex(bar, 100);
                Cnv.Children.Add(bar);
                

                // Значение км. начала сегмента
                var kmBeginingLbl = new TextBlock
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 10,
                    Text = segment.KilometerOfStartPoint.ToString("#.###"),
                    Width = 30,
                    TextAlignment = TextAlignment.Right

                };
                Cnv.Children.Add(kmBeginingLbl);
                Canvas.SetLeft(kmBeginingLbl, (segment.KilometerOfStartPoint - Pipeline.KilometerOfStartPoint) * xScale + margin - 33);
                Canvas.SetTop(kmBeginingLbl, yPos + 5);
                Canvas.SetZIndex(kmBeginingLbl, 100);

                // Значение км. окончания сегмента
                var kmEndLbl = new TextBlock
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 10,
                    Text = segment.KilometerOfEndPoint.ToString("#.###"),
                    Width = 30,
                    TextAlignment = TextAlignment.Left
                };
                Cnv.Children.Add(kmEndLbl);
                Canvas.SetLeft(kmEndLbl, (segment.KilometerOfEndPoint - Pipeline.KilometerOfStartPoint) * xScale + margin + 3);
                Canvas.SetTop(kmEndLbl, yPos + 5);
                Canvas.SetZIndex(kmEndLbl, 100);


                // Описание сегмента
                var descriptionLbl = new TextBlock
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 10,
                    Text = new SegmentWrapper(segment).Description,
                    TextAlignment = TextAlignment.Center
                };
                Cnv.Children.Add(descriptionLbl);
                Canvas.SetLeft(descriptionLbl, (segment.KilometerOfStartPoint - Pipeline.KilometerOfStartPoint) * xScale + margin);
                Canvas.SetTop(descriptionLbl, yPos - 15);
                Canvas.SetZIndex(descriptionLbl, 100);

                yPos += 50;
            }

            chartHeight = chartHeight > yPos ? chartHeight : yPos + 30;
            Cnv.Height = chartHeight;


            // границы газопровода (пунктирные линии)
            var pipelineStartLine = new Line
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 0.5,
                StrokeDashArray = { 1, 2 },
                X1 = margin,
                Y1 = 0,
                X2 = margin,
                Y2 = chartHeight
            };
            Cnv.Children.Add(pipelineStartLine);

            var pipelineEndLine = new Line
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 0.5,
                StrokeDashArray = { 1, 2 },
                X1 = chartWidth - margin,
                Y1 = 0,
                X2 = chartWidth - margin,
                Y2 = chartHeight
            };
            Cnv.Children.Add(pipelineEndLine);


            
            // Определить внутренние сегменты
            var normalSegments = new List<BaseSegmentDTO>();
            foreach (var segment in SegmentList)
            {
                if (SegmentList.Any(s => s != segment 
                    && s.KilometerOfStartPoint <= segment.KilometerOfStartPoint
                    && s.KilometerOfEndPoint >= segment.KilometerOfEndPoint))
                {
                    var x = (segment.KilometerOfStartPoint - Pipeline.KilometerOfStartPoint) * xScale + margin;
                    var width = (segment.KilometerOfEndPoint - segment.KilometerOfStartPoint) * xScale;

                    var markerRect = new Rectangle
                    {
                        Fill = new SolidColorBrush(Colors.Red),
                        StrokeThickness = 0,
                        Width = width,
                        Height = 7
                    };
                    Cnv.Children.Add(markerRect);
                    Canvas.SetLeft(markerRect, x);
                    Canvas.SetTop(markerRect, 13);


                    var highlightRect = new Rectangle
                    {
                        Fill = new SolidColorBrush(Colors.Red),
                        Opacity = 0.15,
                        StrokeThickness = 0,
                        Width = width,
                        Height = chartHeight - 20
                    };
                    Cnv.Children.Add(highlightRect);
                    Canvas.SetLeft(highlightRect, x);
                    Canvas.SetTop(highlightRect, 20);
                    
                }
                else
                {
                    normalSegments.Add(segment);
                }
            }
            


            for (var i = 0; i < normalSegments.Count; i++)
            {
                // Проверка есть ли дырка между текущим и следующим сегментом
                // если есть то подсвечиваем
                if (HighlightGaps)
                {
                    var x = 0.0;
                    var width = 0.0;

                    if (i == 0 && Pipeline.KilometerOfStartPoint < normalSegments[i].KilometerOfStartPoint)
                    {
                        x = margin;
                        width = (normalSegments[i].KilometerOfStartPoint - Pipeline.KilometerOfStartPoint) * xScale;
                    }

                    if (i < normalSegments.Count - 1 && normalSegments[i].KilometerOfEndPoint < normalSegments[i + 1].KilometerOfStartPoint)
                    {
                        x = (normalSegments[i].KilometerOfEndPoint - Pipeline.KilometerOfStartPoint) * xScale + margin;
                        width = (normalSegments[i + 1].KilometerOfStartPoint - normalSegments[i].KilometerOfEndPoint) * xScale;
                    }

                    if (i == normalSegments.Count - 1)
                    {
                        x = (normalSegments[i].KilometerOfEndPoint - Pipeline.KilometerOfStartPoint) * xScale + margin;
                        width = chartWidth - margin - x;
                    }

                    if (width > 0)
                    {

                        var markerRect = new Rectangle
                        {
                            Fill = new SolidColorBrush(Colors.Orange),
                            StrokeThickness = 0,
                            Width = width,
                            Height = 7
                        };
                        Cnv.Children.Add(markerRect);
                        Canvas.SetLeft(markerRect, x);
                        Canvas.SetTop(markerRect, 13);


                        var highlightRect = new Rectangle
                        {
                            Fill = new SolidColorBrush(Colors.Orange),
                            Opacity = 0.15,
                            StrokeThickness = 0,
                            Width = width,
                            Height = chartHeight - 20
                        };
                        Cnv.Children.Add(highlightRect);
                        Canvas.SetLeft(highlightRect, x);
                        Canvas.SetTop(highlightRect, 20);
                    }

                }


                // Проверка есть ли пересечение текущего и следующего сегмента
                // если есть то подсвечиваем
                if (HighlightOverlaps)
                {
                    var x = 0.0;
                    var width = 0.0;

                    if (i < normalSegments.Count - 1
                        && normalSegments[i].KilometerOfEndPoint > normalSegments[i + 1].KilometerOfStartPoint)
                    {
                        x = (normalSegments[i + 1].KilometerOfStartPoint - Pipeline.KilometerOfStartPoint) * xScale + margin;
                        width = (normalSegments[i].KilometerOfEndPoint - normalSegments[i + 1].KilometerOfStartPoint) * xScale;
                    }
                    
                    if (width > 0)
                    {

                        var markerRect = new Rectangle
                        {
                            Fill = new SolidColorBrush(Colors.Red),
                            StrokeThickness = 0,
                            Width = width,
                            Height = 7
                        };
                        Cnv.Children.Add(markerRect);
                        Canvas.SetLeft(markerRect, x);
                        Canvas.SetTop(markerRect, 13);


                        var highlightRect = new Rectangle
                        {
                            Fill = new SolidColorBrush(Colors.Red),
                            Opacity = 0.15,
                            StrokeThickness = 0,
                            Width = width,
                            Height = chartHeight - 20
                        };
                        Cnv.Children.Add(highlightRect);
                        Canvas.SetLeft(highlightRect, x);
                        Canvas.SetTop(highlightRect, 20);
                    }

                }
                
            }



            

        }


        private void OnEdit(object sender, RadRoutedEventArgs e)
        {
            if (EditSegment != null)
                EditSegment.Execute(this);
            
        }

        private void OnDelete(object sender, RadRoutedEventArgs e)
        {
            if(DeleteSegment != null)
                DeleteSegment.Execute(this);
        }
    }
    
    
}
