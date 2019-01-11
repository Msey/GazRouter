using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Model;
using JetBrains.Annotations;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Visuals
{
    /// <summary>
    ///     Визуальный компонент для отображения подсказки по соединению газопроводов
    /// </summary>
    [TemplatePart(Name = PartFigure, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PartLine, Type = typeof(FrameworkElement))]
    public class PipelineConnectionHintWidget : PipelineElementWidget
    {
        private const string PartFigure = "Figure";
        private const string PartLine = "Line";
        private Ellipse _figure;
        private Line _line;
        private PipelineWidget _destPipelineWidget;

        public PipelineConnectionHintWidget([NotNull] PipelineWidget pipelineWidget, double km)
            : base(pipelineWidget, km)
        {
            MinHeight = 6;
            MinWidth = 6;
            Width = pipelineWidget.StrokeThickness;
            Height = pipelineWidget.StrokeThickness;
        }

        public IPipelinePoint DestionationPoint { get; set; }

        public IPipelineConnectionHint PipelineConnection => DataContext as IPipelineConnectionHint;

        protected override bool MakeInfra => false;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _figure = GetTemplateChild(PartFigure) as Ellipse;
            _line = GetTemplateChild(PartLine) as Line;

            UpdateDisplayElement();
            SizeChanged += (sender, args) => UpdateDisplayElement();
        }

        public override void ShowHideKm(bool show)
        {
        }

        protected override void OnPositionChanged(Point position)
        {
            base.OnPositionChanged(position);
            UpdateDisplayElement();
        }

        private void UpdateDisplayElement()
        {
            if (_figure == null)
            {
                return;
            }
            Height = Width = Math.Max(Pipeline.StrokeThickness, 8);

            var x = ActualWidth/2;
            var y = ActualHeight/2;

            _figure.Width = Width;
            _figure.Height = Height;

            if (_line != null)
            {
                if (Pipeline.Schema.IsReadOnly)
                {
                    _line.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _line.Visibility = Visibility.Visible;
                    _line.X1 = x;
                    _line.Y1 = y;

                    if (PipelineConnection != null)
                    {
                        var destPipeline = Schema.Pipelines[PipelineConnection.DestinationPipileneId];
                        if (_destPipelineWidget != destPipeline)
                        {
                            _destPipelineWidget = destPipeline;
                            _destPipelineWidget.PropertyChanged += (s, e) => UpdateDisplayElement();
                        }

                        _figure.Fill = new SolidColorBrush(_destPipelineWidget.Stroke);
                        var destPoint = PipelineConnection.ConnectToStart
                            ? _destPipelineWidget.StartPoint
                            : _destPipelineWidget.EndPoint;

                        var destPos = destPoint.Substract(Position );
                        _line.X2 = !destPos.X.IsNanOrInfinity() ? destPos.X + x : 0 + x;
                        _line.Y2 = !destPos.Y.IsNanOrInfinity() ? destPos.Y + y : 0 + y;

                        _line.Visibility = new Point(_line.X1, _line.Y1).Distance(new Point(_line.X2, _line.Y2)) < 10
                            ? Visibility.Collapsed
                            : Visibility.Visible;
                    }
                }
            }
            RenderTransform = new TranslateTransform {X = -x, Y = -y};
        }

        /*
                private Line _line;
        //        private Path _point;
                protected static readonly Style ToolTipStyle = System.Windows.Application.Current.Resources["WidgetToolTip"] as Style;
              


              

                private void PipeOnPropertyChanged()
                {
                    Position = Data.Position.Value;
                    _line.Y1 = Position.Y;
                    _line.X1 = Position.X;
                }

                private void ConnectedPipeLinePropertyChanged()
                {


                    _line.X2 = Data.PiplineEndPosition.Value.X;
                    _line.Y2 = Data.PiplineEndPosition.Value.Y;
                }

                private PipelineConnectionHintWidget()
                {
                    DefaultStyleKey = typeof(PipelineConnectionHintWidget);
                }

        /*
                void PointMouseRightButtonUp(object sender, MouseButtonEventArgs e)
                {
                    NotifyMouseRightButtonUp(this, e);
                }
        #1#





                public virtual void Update()
                {
                    if (!Data.Position.HasValue)
                    {
                        Visibility = Visibility.Collapsed;
                        return;
                    }

                    var pt1 = Data.PiplineEndPosition.Value;
                    var pt2 = Data.Position.Value;


                    Visibility = Visibility.Visible;


                    _line.X2 = pt2.X;
                    _line.Y2 = pt2.Y;

                 //   var geom = (EllipseGeometry)_point.Data;
           //         geom.Center = pt2;
               //     geom.RadiusX = geom.RadiusY = Data.ConnectionPointRadius;

                //    _point.Fill = new SolidColorBrush(Schema.StandardColorsMode ? Data.StandardConnectionPointColor : Data.ConnectionPointColor);
              //      _point.Stroke = Schema.GetCanvasBackground();
                   // if (Data.Km.HasValue) SetToolTip(_point, Data.Km.Value.ToString("0.##"));

                    UpdateDisplayElement();

                }

                public virtual void Destroy()
                {
                    Schema.RemoveItemFromCanvas(_line);
        //            Schema.RemoveItemFromCanvas(_point);

                }

                /// <summary>
                /// Слой схемы, на котором находится данный виджет
                /// </summary>
                public virtual SchemeLayers Layer
                {
                    get { return SchemeLayers.Base; }
                }

                public PipelineConnection Data { get; private set; }
                protected Schema Schema { get; set; }

                protected virtual void Initialize()
                {
                    _line = new Line
                    {
                        StrokeThickness = 0.5,r
                        Stroke = new SolidColorBrush(FloColors.Red),
                        StrokeDashArray = new DoubleCollection { 3, 4 }
                    };
                    Schema.AddItemToCanvas(_line, (int)WidgetZOrder.PipelineConnectionHint);

        //            _point = new Path { Data = new EllipseGeometry(), StrokeThickness = 1 };

        //            _point.MouseRightButtonUp += PointMouseRightButtonUp;

        //            Schema.AddItemToCanvas(_point,(int)WidgetZOrder.PipelineConnectionHint + 1 );

                }


             

             

     
                public virtual void Select()
                {

                }

                public virtual void Deselect()
                {
                }

                SchemeObject IWidget.GetData()
                {
                    return Data;
                }

                public bool IsSelected { get; set; }


                protected void SetToolTip(UIElement element, string tooltip)
                {
                    ToolTipService.SetToolTip(element, new ToolTip { Content = tooltip, Style = ToolTipStyle });
                }

                /*
                        /// <summary>
                        /// Формируется при клике правой кнопки
                        /// </summary>
                //        public event MouseButtonEventHandler MouseRightButtonUp;

                        public void NotifyMouseRightButtonUp(MouseButtonEventArgs args)
                        {
                            if (MouseRightButtonUp != null)
                            {
                                MouseRightButtonUp(this, args);
                            }
                        }
                #1#

                /*
                        public void NotifyMouseRightButtonUp(object sender, MouseButtonEventArgs args)
                        {
                            if (MouseRightButtonUp != null)
                            {
                                MouseRightButtonUp(sender, args);
                            }
                        }
                #1#
                /*
               /// <summary>
               /// Формируется при клике левой кнопки
               /// </summary>
               //        public event MouseButtonEventHandler MouseLeftButtonUp;

                   protected void NotifyMouseLeftButtonUp(MouseButtonEventArgs args)
                    {
                        if (MouseLeftButtonUp != null)
                        {
                            MouseLeftButtonUp(this, args);
                        }
                    }

                    public void NotifyMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
                    {
                        if (MouseLeftButtonUp != null)
                        {
                            MouseLeftButtonUp(sender, args);
                        }
                    }#1#
        */
    }
}