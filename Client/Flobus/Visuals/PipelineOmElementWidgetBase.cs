using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using GazRouter.Common.Events;
using GazRouter.Controls.Dialogs.ObjectDetails;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Misc;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.Flobus.Extensions;
using System.Windows.Input;

namespace GazRouter.Flobus.Visuals
{
    /// <summary> Базовый класс для элементов газопрвода из Объектной модели </summary>
    [DebuggerDisplay(nameof(Position) + ": {" + nameof(Position) + "}")]
    [TemplatePart(Name = PartContainer, Type = typeof(Grid))]
    [TemplatePart(Name = PartFigure, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PartLabel, Type = typeof(TextBlock))]
    public abstract class PipelineOmElementWidgetBase : PipelineElementWidget, ISupportContextMenu
    {
        public static readonly DependencyProperty ContainerPositionProperty = DependencyProperty.Register(
            nameof(ContainerPosition), typeof(Point), typeof(WidgetBase),
            new PropertyMetadata(new Point(double.NaN, double.NaN), OnContainerPositionPropertyChanged));

        public static readonly DependencyProperty TextAngleProperty = DependencyProperty.Register(
            nameof(TextAngle), typeof(int), typeof(PipelineOmElementWidgetBase),
            new PropertyMetadata(0, OnTextAnglePropertyChanged));

        public static readonly DependencyProperty ContainerVisibilityProperty = DependencyProperty.Register(
            nameof(ContainerVisibility), typeof(Visibility), typeof(DistributingStationWidget),
            new PropertyMetadata(Visibility.Visible));

        protected const int VerticalAngle = 270;

        private const string PartLabel = "Label";
        private const string PartContainer = "Container";
        private const string PartFigure = "Figure";

        [CanBeNull] private FrameworkElement _label;
        [CanBeNull] private Grid _container;
        [CanBeNull] private FrameworkElement _figure;

        protected PipelineOmElementWidgetBase(PipelineWidget pipelineWidget, IPipelineOmElement el)
            : base(pipelineWidget, el.Km)
        {           
            Id = el.Id;
            DataContext = el;                                   
            if (IsError)
            {
                ServiceLocator.Current.GetInstance<IEventAggregator>()
                     .GetEvent<AddLogEntryEvent>()
                        .Publish(new Tuple<string, string>(el.Km.ToString(), 
                                    $"{ConvertGuidFromNetToOracle(el.Id.ToString())} {GetType()}"));
            }
        }
        private static string ConvertGuidFromNetToOracle(string g)
        {            
            var sGuid = BitConverter.ToString(new Guid(g).ToByteArray());
            return sGuid.Replace("-", "");
        }
        public Guid Id { get; }
        public abstract EntityType EntityType { get; }
        public Point ContainerPosition
        {
            get { return (Point)GetValue(ContainerPositionProperty); }
            set { SetValue(ContainerPositionProperty, value); }
        }
        public int TextAngle
        {
            get { return (int) GetValue(TextAngleProperty); }
            set { SetValue(TextAngleProperty, value); }
        }
        public Visibility ContainerVisibility
        {
            get { return (Visibility) GetValue(ContainerVisibilityProperty); }
            set { SetValue(ContainerVisibilityProperty, value); }
        }

        public virtual void FillMenu(RadContextMenu menu, Point mousePosition, Schema schema)
        {
            if (!schema.IsReadOnly)
            {
                menu.AddCommand("Повернуть текст", new DelegateCommand(RotateLabel));
                menu.AddSeparator();

                menu.AddCommand("Показать в дереве", new DelegateCommand(
                    () => schema.GotoTreeCommand.Execute(Id),
                    () => schema.GotoTreeCommand != null && schema.GotoTreeCommand.CanExecute(Id)));
            }

            menu.AddCommand("Паспорт...",
                new DelegateCommand(
                    () =>
                        new ObjectDetailsView
                        {
                            DataContext = new ObjectDetailsViewModel(Id, EntityType)
                        }.ShowDialog()));

            if (Deleteble())
                menu.AddCommand("Удалить", new DelegateCommand(
                          () => RadWindow.Confirm(new DialogParameters
                          {
                              Header = "Внимание!",
                              Content = @"Вы уверены, что хотите удалить объект со схемы?",
                              Closed = (sender, e1) =>
                              {
                                  if (e1.DialogResult.HasValue && e1.DialogResult.Value)
                                  {
                                      bool? done = false;
                                      if (Schema.SchemaSource != null)
                                      {
                                          done = DeleteCommand();
                                      }
                                      if (done.HasValue && done.Value)
                                      {
                                          Pipeline.DeleteItem(this);
                                          schema.RemovedPipelineChild(this);
                                      }
                                  }
                              }
                          })));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_label != null)
            {
                _label.SizeChanged -= LabelOnSizeChanged;
            }

            _label = GetTemplateChild(PartLabel) as FrameworkElement;

            if (_label != null)
            {
                _label.SizeChanged += LabelOnSizeChanged;
            }                       
            
            if (_container != null)
            {
                _container.SizeChanged -= OnContainerSizeChanged;
            }

            _container = GetTemplateChild(PartContainer) as Grid;

            if (_container != null)
            {
                _container.SizeChanged += OnContainerSizeChanged;
            }

            _container = GetTemplateChild("Container") as Grid;

            if (_container != null)
            {
                _container.MouseLeftButtonDown += Container_MouseLeftButtonDown;
                _container.MouseMove += Container_MouseMove;
                _container.MouseLeftButtonUp += Container_MouseLeftButtonUp;
            }

            if (_figure != null)
            {
                _figure.SizeChanged -= OnFigureSizeChanged;
            }

            _figure = GetTemplateChild(PartFigure) as FrameworkElement;

            if (_figure != null)
            {
                _figure.SizeChanged += OnFigureSizeChanged;
            }
            UpdateContentLayout();
        }

        protected abstract void UpdateLabelLocation([CanBeNull] FrameworkElement label);

        protected override void CreateBindings()
        {
            base.CreateBindings();
            SetBinding(ContainerPositionProperty, new Binding("ContainerPosition") { Mode = BindingMode.TwoWay });
            SetBinding(TextAngleProperty, new Binding("TextAngle") {Mode = BindingMode.TwoWay});
        }

/*
        protected void SetToolTip(UIElement element, string tooltip)
        {
            ToolTipService.SetToolTip(element, new ToolTip {Content = tooltip, Style = ToolTipStyle});
        }
*/

        protected void RotateLabel()
        {
            TextAngle = TextAngle == 270 ? 0 : 270;
        }

        protected sealed override void UpdateContentLayout()
        {
            OnTextAngleChanged();
            if (ContainerPosition != new Point(0, 0))
                _container.SetLocation(ContainerPosition);
            else
                UpdateContainerLocation(_container);
        }

        protected abstract void UpdateContainerLocation([CanBeNull] Grid container);

        private static void OnTextAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var omElementWidgetBase = (PipelineOmElementWidgetBase) d;
            omElementWidgetBase.OnTextAngleChanged();
        }

        private void LabelOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            UpdateLabelLocation(_label);
        }

        public bool IsDraging { get; set; }
        private void Container_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsDraging)
                return;
            var cont = sender as Grid;
            IsDraging = false;
            Schema.IsPanEnabled = true;
            cont.ReleaseMouseCapture();
        }

        private void Container_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDraging)
                return;
            var cont = sender as Grid;
            ContainerPosition = e.GetPosition(this);
            _container.SetLocation(ContainerPosition);
        }

        private void Container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((Schema.IsReadOnly) || !(Keyboard.Modifiers ==  ModifierKeys.Control))
                return;
            Schema.IsPanEnabled = false;
            IsDraging = true;
            var cont = sender as Grid;
            cont.CaptureMouse();
        }

        private void OnContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateContentLayout();
        }

        private void OnTextAngleChanged()
        {
            if (_label == null)
            {
                return;
            }

            var transform = _label.RenderTransform as RotateTransform;
            if (transform == null)
            {
                transform = new RotateTransform
                {
                    CenterX = Width/2,
                    CenterY = Height/2
                };

                _label.RenderTransform = transform;
            }
            transform.Angle = TextAngle;
            UpdateLabelLocation(_label);
        }

        private void OnFigureSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateContentLayout();
        }
        
        private static void OnContainerPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        //        protected abstract void UpdateDisplayElement();
    }
}