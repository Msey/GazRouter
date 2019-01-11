using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GazRouter.Flobus.Misc;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Elements
{
    public class LineElementPointView : ElementViewBase
    {
        private readonly Path _circle;
        private bool _mouseButtonPressed;
        private readonly LineElementView _line;


        //public LineElementPointModel Model { get; private set; }

        public override Visibility Visibility
        {
            get { return _circle.Visibility; }
            set { _circle.Visibility = value; }
        }

        //public Point Position
        //{
        //    get { return ElementModel.Position; }
        //    set { ElementModel.Position = value; }
        //}


        public LineElementPointView(DashboardElementContainer dashboard, LineElementPointModel pt, LineElementView line)
            : base(dashboard, pt)
        {
            //Dashboard.AddElement(this);

            _line = line;

            _circle = new Path
            {
                Fill = new SolidColorBrush(Colors.Orange),
                Stroke = Dashboard.Background,
                StrokeThickness = 1
            };
            _circle.MouseLeftButtonDown += OnMouseLeftButtonDown;
            _circle.MouseLeftButtonUp += OnMouseLeftButtonUp;
            _circle.MouseMove += MouseMove;
            _circle.MouseRightButtonUp += OnMouseRightButtonUp;
            Dashboard.Canvas.Children.Add(_circle);

            InitCommands();

            UpdatePosition();
        }

        public override void UpdatePosition()
        {
            _circle.Data = new EllipseGeometry
            {
                Center = ElementModel.Position,
                RadiusX = 5,
                RadiusY = 5
            };
            Canvas.SetZIndex(_circle, 303030);

            if (_mouseButtonPressed)
                _line.UpdatePosition();
            
        }


        public override void Destroy()
        {
            //Dashboard.RemoveElement(this);
            Dashboard.Canvas.Children.Remove(_circle);
            _circle.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            _circle.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            _circle.MouseMove -= MouseMove;
            _circle.MouseRightButtonUp -= OnMouseRightButtonUp;
        }


        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseButtonPressed)
            {
                Position = e.GetPosition(_circle);
                UpdatePosition();
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _circle.ReleaseMouseCapture();
            if (_mouseButtonPressed)
                _line.ElementModel.RecalculatePosition();
            _mouseButtonPressed = false;
            NotifyMouseLeftButtonUp(this, e);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _circle.CaptureMouse();
            _mouseButtonPressed = true;
            
            NotifyMouseLeftButtonDown(this, e);
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Dashboard.IsEditMode)
                NotifyMouseRightButtonUp(this, e);
        }


        #region CONTEXT_MENU

        public override void FillMenu(RadContextMenu menu, MouseButtonEventArgs e)
        {
            base.FillMenu(menu, e);
            menu.AddCommand("Удалить точку", _remove, e);
        }

        private DelegateCommand<MouseButtonEventArgs> _remove;
        
        private void InitCommands()
        {
            //  Удаление точки линии
            _remove = new DelegateCommand<MouseButtonEventArgs>(
                eventArg =>
                {
                    _line.DeletePoint(this);
                    _line.ElementModel.RecalculatePosition();
                }, 
                eventArg => ((LineElementPointModel)ElementModel).DeleteAllowed);

        }

        #endregion
    }
}