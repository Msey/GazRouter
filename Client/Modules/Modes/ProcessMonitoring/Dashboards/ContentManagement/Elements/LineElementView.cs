using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GazRouter.Flobus.Misc;
using GazRouter.Flobus.UiEntities;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Dialogs;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Elements
{

    public class LineElementView : BoxedElementView<LineElementModel>
    {
        private readonly Polyline _line;
        private readonly Polyline _selector;
        private List<LineElementPointView> _points; 


        
        public LineElementView(LineElementModel elementModel, DashboardElementContainer dashboard)
            : base(elementModel, dashboard, false, false, false, false)
        {
            Dashboard.AddElement(this);

            _line = new Polyline();
            Dashboard.Canvas.Children.Add(_line);

            _selector = new Polyline
            {
                Stroke = new SolidColorBrush(Colors.Transparent),
                StrokeThickness = 6
            };
            Dashboard.Canvas.Children.Add(_selector);
            _selector.MouseRightButtonUp += LineOnMouseRightButtonUp;
            _selector.MouseLeftButtonDown += LineOnMouseLeftButtonDown;
            _selector.MouseLeftButtonUp += LineOnMouseLeftButtonUp;
            _selector.MouseMove += LineOnMouseMove;
            
            _points = new List<LineElementPointView>();
            foreach (var pt in ElementModel.PointList)
            {
                var pv = new LineElementPointView(dashboard, pt, this);
                pv.MouseRightButtonUp += NotifyMouseRightButtonUp;
                _points.Add(pv);
            }
            
            Deselect();

            UpdatePosition();

            InitCommands();
        }

        void LineOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDraging) return;

            var pos = e.GetPosition(_selector);
            Move(pos.X - _dragBuffPosition.X, pos.Y - _dragBuffPosition.Y);
            _dragBuffPosition = pos;

            NotifyElementMove(this, new Point(pos.X - _dragBuffPosition.X, pos.Y - _dragBuffPosition.Y));
        }

        private void LineOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Dashboard.IsEditMode)
            {
                NotifyMouseLeftButtonUp(this, e);

                ElementModel.RecalculatePosition();
                Dashboard.EndDragElements();
            }
        }

        private void LineOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Dashboard.IsEditMode)
            {
                _dragBuffPosition = e.GetPosition(_selector);
                NotifyMouseLeftButtonDown(this, e);

                Dashboard.StartDragElements();
            }
        }

        private void LineOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Dashboard.IsEditMode)
                NotifyMouseRightButtonUp(this, e);
        }

        public override void Destroy()
        {
            if (_line != null)
                Dashboard.Canvas.Children.Remove(_line);

            if (_selector != null)
            {
                Dashboard.Canvas.Children.Remove(_selector);
                _selector.MouseRightButtonUp -= LineOnMouseRightButtonUp;
                _selector.MouseLeftButtonDown -= LineOnMouseLeftButtonDown;
                _selector.MouseLeftButtonUp -= LineOnMouseLeftButtonUp;
            }

            if (_points != null)
                _points.ForEach(p => p.Destroy());
            
        }

        public override void UpdatePosition()
        {
            _line.Points.Clear();
            _line.Points.AddRange(ElementModel.PointList.Select(p => p.Position));
            Canvas.SetZIndex(_line, Z);

            _selector.Points.Clear();
            _selector.Points.AddRange(ElementModel.PointList.Select(p => p.Position));
            Canvas.SetZIndex(_selector, Z);

            

            if (IsDraging) return;
            _line.StrokeEndLineCap = PenLineCap.Round;
            _line.StrokeStartLineCap = PenLineCap.Round;
            _line.StrokeLineJoin = ElementModel.IsJoinRounded ? PenLineJoin.Round : PenLineJoin.Bevel;
            _line.StrokeThickness = ElementModel.Thickness;
            _line.Stroke = new SolidColorBrush(ElementModel.Color);
            _line.StrokeDashArray = ElementModel.IsDotted ?
                new DoubleCollection { 1, 2 }
                : new DoubleCollection { 1, 0 };

            
        }

        private Point _dragBuffPosition;

        public override void StartDrag()
        {
            base.StartDrag();
            _selector.CaptureMouse();
        }


        public override void EndDrag()
        {
            base.EndDrag();
            _selector.ReleaseMouseCapture();
            
            UpdatePosition();
        }


        public override bool Select()
        {
            if (_points != null)
                _points.ForEach(p => p.Visibility = Visibility.Visible);
            return true;
        }

        public override void Deselect()
        {
            if (_points != null)
                _points.ForEach(p => p.Visibility = Visibility.Collapsed);
        }

        public override void Move(double xOffset, double yOffset)
        {
            foreach (var pt in _points)
                pt.Move(xOffset, yOffset);
            
            UpdatePosition();
        }

        public void DeletePoint(LineElementPointView pt)
        {
            _points.Remove(pt);
            ElementModel.PointList.Remove((LineElementPointModel)pt.ElementModel);
            pt.Destroy();
            UpdatePosition();
        }

        #region CONTEXT_MENU

        public override void FillMenu(RadContextMenu menu, MouseButtonEventArgs e)
        {
            base.FillMenu(menu, e);

            menu.AddCommand("Добавить точку", _addPoint,e );
            menu.AddCommand("Выравнить", _straighten, e);
            menu.AddCommand( "Параметры...", _changeViewSettings,e);
            menu.AddSeparator();
            menu.AddCommand( "Поверх всех", _moveForward,e);
            menu.AddCommand("В самый низ", _moveBackward, e);
            menu.AddSeparator();
            menu.AddCommand( "Удалить", _removeDashboardElement,e);
            


        }

        private DelegateCommand<MouseButtonEventArgs> _removeDashboardElement;
        private DelegateCommand<MouseButtonEventArgs> _moveForward;
        private DelegateCommand<MouseButtonEventArgs> _moveBackward;
        private DelegateCommand<MouseButtonEventArgs> _changeViewSettings;
        private DelegateCommand<MouseButtonEventArgs> _addPoint;
        private DelegateCommand<MouseButtonEventArgs> _straighten;
        //private DelegateCommand<MouseButtonEventArgs> _deletePoint;

        private void InitCommands()
        {
            //  Инициализация команды удаления элемента с дашборда
            _removeDashboardElement = new DelegateCommand<MouseButtonEventArgs>(eventArg => Dashboard.RemoveElement(this), eventArg => true);

            //  Инициализация команды перемещения элемента на передний план
            _moveForward = new DelegateCommand<MouseButtonEventArgs>(eventArg => Dashboard.MoveElementForward(this), eventArg => true);

            //  Инициализация команды перемещения элемента на задний план
            _moveBackward = new DelegateCommand<MouseButtonEventArgs>(eventArg => Dashboard.MoveElementBackward(this), eventArg => true);

            //  Инициализация команды изменения настроек отображения элемента
            _changeViewSettings = new DelegateCommand<MouseButtonEventArgs>(eventArg =>
            {
                var vm = new LineElementSettingsViewModel(ElementModel);
                var dlg = new LineElementSettingsView { DataContext = vm };
                dlg.Closed += (sender, args) =>
                {
                    if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
                    {
                        UpdatePosition();
                    }

                };
                dlg.ShowDialog();


            }, eventArg => true);


            _addPoint = new DelegateCommand<MouseButtonEventArgs>(eventArg =>
            {
                Point pt = eventArg.GetPosition(Dashboard.Canvas);
                for (int i = 0; i < _points.Count - 1; i++)
                {
                    if (Support2D.PointAtLine(ElementModel.PointList[i].Position, ElementModel.PointList[i + 1].Position, pt))
                    {
                        var p = new LineElementPointModel
                        {
                            DeleteAllowed = true,
                            Position = pt,
                        };
                        ElementModel.PointList.Insert(i + 1, p);
                        ElementModel.RecalculatePosition();
                        var pv = new LineElementPointView(Dashboard, p, this);
                        pv.MouseRightButtonUp += NotifyMouseRightButtonUp;
                        _points.Add(pv);
                        UpdatePosition();
                        break;
                    }
                }


            }, 
            eventArg => true);

            _straighten = new DelegateCommand<MouseButtonEventArgs>(eventArgs =>
            {
                ElementModel.Straighten();
                UpdatePosition();
                _points.ForEach(p => p.UpdatePosition());
            });


        }

        #endregion
        
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}