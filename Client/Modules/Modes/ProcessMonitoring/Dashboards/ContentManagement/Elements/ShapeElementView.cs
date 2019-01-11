using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GazRouter.Flobus.Misc;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Dialogs;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Elements
{

    public class ShapeElementView : BoxedElementView<ShapeElementModel>
    {
        private readonly Path _path;

        public ShapeElementView(ShapeElementModel elementModel, DashboardElementContainer dashboard, bool IsAdd)
            : base(elementModel, dashboard, true, true, true, false)
        {
            _path = new Path
            {
                Stroke = new SolidColorBrush(elementModel.StrokeColor),
                StrokeThickness = elementModel.StrokeThickness,
                Fill = new SolidColorBrush(elementModel.FillColor),
            };

            if (IsAdd) ShapeParameters(true);
            else
            {
                Dashboard.Canvas.Children.Add(_path);
                UpdatePosition();
            }

            InitCommands();

        }

        public override void Destroy()
        {
            if (_path != null)
                Dashboard.Canvas.Children.Remove(_path);            
            base.Destroy();
        }


        public void ShapeParameters(bool IsAdd)
        {
            var vm = new ShapeElementSettingsViewModel(ElementModel, IsAdd);
            var dlg = new ShapeElementSettingsView { DataContext = vm };
            dlg.Closed += (sender, args) =>
            {
                if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
                {
                    if (IsAdd) Dashboard.Canvas.Children.Add(_path);
                    UpdatePosition();
                }
            };
            dlg.ShowDialog();
        }

        public override void UpdatePosition()
        {
            base.UpdatePosition();

            if (IsDraging) return;


            _path.Stroke = new SolidColorBrush(ElementModel.StrokeColor);
            _path.StrokeDashArray = new DoubleCollection
            {
                ElementModel.StrokeThickness, 
                ElementModel.IsStrokeDotted ? ElementModel.StrokeThickness : 0
            };
            _path.StrokeThickness = ElementModel.StrokeThickness;
            _path.Fill = new SolidColorBrush(ElementModel.FillColor);

            
            

            switch (ElementModel.Type)
            {
                case ShapeType.Ellipse:
                    _path.Data = new EllipseGeometry
                    {
                        Center = new Point(ElementModel.Width / 2, ElementModel.Height / 2),
                        RadiusX = ElementModel.Width / 2,
                        RadiusY = ElementModel.Height / 2
                    };
                    break;

                case ShapeType.Rectangle:
                    _path.Data = new RectangleGeometry
                    {
                        Rect = new Rect(0, 0, ElementModel.Width, ElementModel.Height)
                    };
                    break;

                case ShapeType.Triangle:
                    var geom = new PathGeometry();
                    var seg = new PathFigure
                    {
                        IsFilled = true,
                        IsClosed = true,
                        StartPoint = new Point(0, Height)
                    };
                    seg.Segments.Add(
                        new PolyLineSegment
                        {
                            Points = new PointCollection
                            {
                                new Point(Width / 2, 0), 
                                new Point(Width, Height),
                            }
                        });
                    geom.Figures.Add(seg);
                    _path.Data = geom;
                    
                    break;
            }

            _path.RenderTransform = new RotateTransform
            {
                Angle = ElementModel.RotateAngle,
                CenterX = ElementModel.Width / 2,
                CenterY = ElementModel.Height / 2
            };

            Canvas.SetLeft(_path, Position.X);
            Canvas.SetTop(_path, Position.Y);
            Canvas.SetZIndex(_path, Z + 2);
        }
        


        public override void EndDrag()
        {
            base.EndDrag();
            //_path.Visibility = Visibility.Visible;
            
            UpdatePosition();
        }

        #region CONTEXT_MENU

        public override void FillMenu(RadContextMenu menu, MouseButtonEventArgs e)
        {
            base.FillMenu(menu, e);
            menu.AddCommand( "Параметры...", _changeViewSettings,e);
            menu.AddSeparator();
            menu.AddCommand( "Поверх всех", _moveForward,e);
            menu.AddCommand( "В самый низ", _moveBackward,e);
            menu.AddSeparator();
            menu.AddCommand("Удалить", _removeDashboardElement,e);
        }

        private DelegateCommand<MouseButtonEventArgs> _removeDashboardElement;
        private DelegateCommand<MouseButtonEventArgs> _moveForward;
        private DelegateCommand<MouseButtonEventArgs> _moveBackward;
        private DelegateCommand<MouseButtonEventArgs> _changeViewSettings;

        private void InitCommands()
        {
            //  Инициализация команды удаления элемента с дашборда
            _removeDashboardElement = new DelegateCommand<MouseButtonEventArgs>(eventArg => Dashboard.RemoveElement(this), eventArg => true);

            //  Инициализация команды перемещения элемента на передний план
            _moveForward = new DelegateCommand<MouseButtonEventArgs>(eventArg => Dashboard.MoveElementForward(this), eventArg => true);

            //  Инициализация команды перемещения элемента на задний план
            _moveBackward = new DelegateCommand<MouseButtonEventArgs>(eventArg => Dashboard.MoveElementBackward(this), eventArg => true);

            //  Инициализация команды изменения настроек отображения элемента
            _changeViewSettings = new DelegateCommand<MouseButtonEventArgs>(eventArg => ShapeParameters(false), eventArg => true);

        }

        #endregion
        
    }
}