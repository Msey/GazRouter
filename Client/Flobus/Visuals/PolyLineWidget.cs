using GazRouter.Flobus.Dialogs;
using GazRouter.Flobus.FloScheme;
using JetBrains.Annotations;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Misc;
using System.Windows.Input;
using System.Collections.Generic;
using Telerik.Windows.Diagrams.Core;
using GazRouter.Flobus.UiEntities.FloModel;

namespace GazRouter.Flobus.Visuals
{
    public class PolyLineWidget : ShapeWidgetBase, ISupportContextMenu, IPolyLineWidget
    {
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            nameof(Stroke), typeof(Color), typeof(PolyLineWidget),
            new PropertyMetadata(Colors.Black, OnStrokePropertyChanged));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            nameof(StrokeThickness), typeof(double), typeof(PolyLineWidget), new PropertyMetadata(2d));
                      

        private Polyline _line;
        private Ellipse _startpoint;
        private Ellipse _endpoint;

        IPolyLine model;
        
        public PolyLineWidget(IPolyLine pl,[NotNull] Schema schema) : base(schema)
        {
            IsManipulationAdornerVisible = false;
            model = pl;
            Position = model.Position;
            StartPoint = model.StartPoint;
            EndPoint = model.EndPoint;
            Type = model.Type;                                    
            schema.MouseLeftButtonUp += Schema_MouseLeftButtonUp;

        }

        protected override void OnPositionChanged(Point position)
        {
            base.OnPositionChanged(position);
            model.Position = position;
        }
        public override void Update()
        {
            UpdatePosition();
        }
        private void Schema_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsSelected)
                HideManipulationPoints();
            else
                HideManipulationPoints(true);
        }

        public void FillMenu(RadContextMenu menu, Point mousePosition, Schema schema)
        {
            if (!Schema.IsReadOnly)
            {
                //menu.AddCommand("Добавить точку", _addPoint, e);
                //menu.AddCommand("Выравнить", _straighten, e);
                menu.AddCommand("Параметры...", new Microsoft.Practices.Prism.Commands.DelegateCommand(EditPolyLine));
                //menu.AddSeparator();
                //menu.AddCommand("Поверх всех", _moveForward, e);
                //menu.AddCommand("В самый низ", _moveBackward, e);
                menu.AddSeparator();
                menu.AddCommand("Удалить", new Microsoft.Practices.Prism.Commands.DelegateCommand(
                    () => RadWindow.Confirm(new DialogParameters
                    {
                        Header = "Внимание!",
                        Content = @"Вы уверены, что хотите удалить линию со схемы?",
                        Closed = (sender, e1) =>
                        {
                            if (e1.DialogResult.HasValue && e1.DialogResult.Value)
                            {
                                Schema.RemovePolyLineWidget(this);
                            }
                        }
                    })));
            }       
        }
                       
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public Color Stroke
        {
            get { return (Color)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        
        public Point StartPoint
        {
            get { return model.StartPoint; }
            set { model.StartPoint = value; }
        }

        public Point EndPoint
        {
            get { return model.EndPoint; }
            set { model.EndPoint = value; }
        }


        public LineType Type
        {
            get { return model.Type; }
            set { model.Type = value; }
        }
        
        public string Name
        {
            get { return model.Name; }
            set { model.Name = value; }
        }

        public string Description
        {
            get { return model.Description; }
            set { model.Description = value; }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _line = GetTemplateChild("PolyLine") as Polyline;
            _startpoint = GetTemplateChild("StartPoint") as Ellipse;
            _endpoint = GetTemplateChild("EndPoint") as Ellipse;
                       

            _startpoint.MouseLeftButtonDown += ManipulationPointMouseLeftButtonDown;
            _endpoint.MouseLeftButtonDown += ManipulationPointMouseLeftButtonDown;
            _startpoint.MouseLeftButtonUp += ManipulationPointMouseLeftButtonUp;
            _endpoint.MouseLeftButtonUp += ManipulationPointMouseLeftButtonUp;
            _startpoint.MouseMove += ManipulationPointMouseMove;
            _endpoint.MouseMove += ManipulationPointMouseMove;

            BindColors();
            Update();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }
        
        public void HideManipulationPoints(bool hide = false)
        {
            if(_startpoint != null && _endpoint != null)
            _startpoint.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
            _endpoint.Visibility   = hide ? Visibility.Collapsed : Visibility.Visible; ;
        }        
        private void ManipulationPointMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDraging)
                return;
            var el = sender as Ellipse;
            var position = e.GetPosition(this);
            if (el.Name == "StartPoint")
                StartPoint = position;
            if (el.Name == "EndPoint")
                EndPoint = position;
            UpdatePosition();
        }
        
        private void ManipulationPointMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsDraging)
                return;
            var el = sender as Ellipse;
            el.Fill = new SolidColorBrush(Stroke);
            IsDraging = false;
            Schema.IsPanEnabled = true;                        
            el.ReleaseMouseCapture();
        }
        
        private void ManipulationPointMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {  
            Schema.IsPanEnabled = false;
            IsDraging = true;
            var el = sender as Ellipse;
            el.CaptureMouse();
            el.Fill = new SolidColorBrush(Colors.Red);
        }

        private void BindColors()
        {
            if (_line == null)
            {
                return;
            }
            _line.SetBinding(Shape.StrokeProperty, new Binding("Stroke")
            {
                Mode = BindingMode.OneWay,
                RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
                Converter = new ColorToBrushConverter()
            });
        }
                
        private static void OnStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        public IEnumerable<Point> Points => new List<Point>() { StartPoint, EndPoint };

        public bool IsDraging { get; set; }
        
        private void EditPolyLine()
        {
            var vm = new PolyLineStyleEditDialogViewModel(this);
            var dlg = new PolyLineStyleEditDialog { DataContext = vm };
            dlg.Closed += (sender, args) =>
            {
                if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
                {
                    UpdatePosition();
                }
            };
            dlg.ShowDialog();
        }

        public void UpdatePosition()
        {
            _startpoint.Margin = new Thickness(StartPoint.X - (_startpoint.Width / 2), StartPoint.Y - (_startpoint.Width / 2), 0, 0);
            _endpoint.Margin = new Thickness(EndPoint.X - (_endpoint.Width / 2), EndPoint.Y - (_endpoint.Width / 2), 0, 0);

            _line.Points.Clear();
            _line.Points.AddRange(Points);

            _line.StrokeEndLineCap = PenLineCap.Round;
            _line.StrokeStartLineCap = PenLineCap.Round;
            _line.StrokeLineJoin = PenLineJoin.Round;//  PenLineJoin.Bevel;
            switch(Type)
            {
                case LineType.Solid:
                    _line.StrokeDashArray = new DoubleCollection();
                    break;
                case LineType.Dash:
                    _line.StrokeDashArray = new DoubleCollection { 2, 2 };
                    break;
                case LineType.DashDot:
                    _line.StrokeDashArray = new DoubleCollection { 2, 2, 0, 2 };
                    break;
                case LineType.DashDotDot:
                    _line.StrokeDashArray = new DoubleCollection { 2, 2, 0, 2, 0, 2 };
                    break;
                case LineType.Dot:
                    _line.StrokeDashArray = new DoubleCollection { 0, 2 };
                    break;

            }
        }

    }
}


