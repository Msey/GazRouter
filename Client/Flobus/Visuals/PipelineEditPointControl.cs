using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using GazRouter.Flobus.Dialogs;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Misc;
using GazRouter.Flobus.Model;
using JetBrains.Annotations;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using ISupportMouseOver = Telerik.Windows.Diagrams.Core.ISupportMouseOver;

namespace GazRouter.Flobus.Visuals
{

    /// <summary>
    /// Визуальный компонент для редактирования сегментов газопровода
    /// </summary>
    public class PipelineEditPointControl : Control, ISupportContextMenu, ISupportMouseOver , IPipelineEditPoint
    {
        public static readonly DependencyProperty KmProperty = DependencyProperty.Register(
    nameof(Km), typeof(double), typeof(PipelineEditPointControl), new PropertyMetadata(default(double)));


        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            nameof(Position), typeof(Point), typeof(PipelineEditPointControl), new PropertyMetadata(OnPositionPropertyChanged
                ));

        private readonly IPipelinePoint _point;
        private readonly PipelineWidget _pipeline;

        private bool _isManipulating;



        public PipelineEditPointControl([NotNull] IPipelinePoint point, [NotNull] PipelineWidget pipeline)
            : this()
        {
            if (point == null) throw new ArgumentNullException(nameof(point));
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));
            _point = point;
            _pipeline = pipeline;
     
            Km = point.Km;

            Position = point.Position;
     
            SizeChanged += (sender, args) => UpdateDisplayElement();
        }

        private PipelineEditPointControl()
        {
            DefaultStyleKey = typeof(PipelineEditPointControl);
        }

        public IPipelinePoint Data
        {
            get
            {
                return DataContext as IPipelinePoint;
            }
            set
            {
                DataContext = value;
                CreateBindings();
            }
        }

        public double Km
        {
            get { return (double)GetValue(KmProperty); }
            set { SetValue(KmProperty, value); }
        }

        public IPipelineWidget Pipeline => _pipeline;

        public IPipelinePoint PipelinePoint
        {
            get;
            set;
        }

        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public bool IsMouseOver { get; private set; }

        public bool IsManipulating
        {
            get
            {
                return _isManipulating;
            }
            set
            {
                if (_isManipulating != value)
                {
                    _isManipulating = value;
                    UpdateVisualStates();
                }
            }
        }

        public PointType Type => PipelinePoint.Type;

      

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
           UpdateVisualStates();
        }

        public virtual void FillMenu(RadContextMenu menu, Point mousePosition, Schema schema)
        {
            if (schema.IsReadOnly) return;

            menu.AddCommand("Изменить значение километра...", new DelegateCommand(() =>
            {
                var dlg2 = new PipelinePointManageDialog
                {
                    DataContext = _point
                };

                dlg2.Show();
            }, () => Type == PointType.Intermediate));
            menu.AddCommand("Удалить точку", new DelegateCommand(() =>
            {


                if (_point != null)
                {
                    RadWindow.Confirm(new DialogParameters
                    {
                        Header = "Внимание!",
                        Content = @"Вы уверены, что хотите удалить точку изгиба газопровода?",
                        Closed = (sender, e1) =>
                        {
                            if (e1.DialogResult.HasValue && e1.DialogResult.Value)
                                Pipeline.RemovePoint(_point);
                        }
                    });
                }
            }, () => Type == PointType.Intermediate));
        }

        public void UpdateDisplayElement()
        {
            var top = Position.Y - Height / 2;
            var left = Position.X - Width / 2;
            RenderTransform = new TranslateTransform { X = left, Y = top };
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            _pipeline?.OnManipulationPointActivated(this);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            ReleaseMouseCapture();
            UpdateVisualStates();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            IsMouseOver = true;
            UpdateVisualStates();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            IsMouseOver = false;
            UpdateVisualStates();
        }

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PipelineEditPointControl)d).OnPositionChanged();
        }

        private void UpdateVisualStates()
        {
            VisualStateManager.GoToState(this, IsMouseOver ? "MouseOver" : "Normal", true);
            VisualStateManager.GoToState(this, IsManipulating ? "Drag" : "NoDrag", true);
        }

        private void OnPositionChanged()
        {
            if (IsManipulating)
            {
                UpdateDisplayElement();
            }
        }



     









        private void CreateBindings()
        {
            SetBinding(KmProperty, new Binding(nameof(Data.Km)));
        }


       
    }
}