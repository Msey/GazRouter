using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.Flobus.Extensions;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.UiEntities.FloModel;
using JetBrains.Annotations;
namespace GazRouter.Flobus.Visuals
{
    /// <summary>
    ///     Визуальный компонент для крана
    /// </summary>
    public class ValveWidget : PipelineOmElementWidgetBase
    {
        public static readonly DependencyProperty ValveBrushProperty = DependencyProperty.Register(
            nameof(ValveBrush), typeof(Brush), typeof(ValveWidget), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty TooltipProperty = DependencyProperty.Register(
            nameof(Tooltip), typeof(string), typeof(ValveWidget),
            new PropertyMetadata(string.Empty, OnTooltipPropertyChanged));

        private bool _isSelected;
//        private TextBlock _label;
//        private FrameworkElement _labelPanel;

        public ValveWidget([NotNull] PipelineWidget pipelineWidget, IValve v) : base(pipelineWidget, v)
        {
        }

        public override EntityType EntityType => EntityType.Valve;

        public Brush ValveBrush
        {
            get { return (Brush) GetValue(ValveBrushProperty); }
            set { SetValue(ValveBrushProperty, value); }
        }

        public string Tooltip
        {
            get { return (string) GetValue(TooltipProperty); }
            set { SetValue(TooltipProperty, value); }
        }

        private bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                UpdateVisualStates();
            }
        }

        public override void Select()
        {
            IsSelected = true;
        }

        public override void ShowHideKm(bool show)
        {
            // _label.Text = show ? Km.ToString(CultureInfo.InvariantCulture) : Caption;
        }

        protected override void UpdateLabelLocation(FrameworkElement label)
        {
            if (label == null)
            {
                return;
            }

            if (Orientation == Orientation.Vertical)
            {
                if (TextAngle == VerticalAngle)
                {
                    label.SetLocation(ActualWidth, label.ActualWidth/2 - Height/2);
                }
                else
                {
                    label.SetLocation(ActualWidth, -(label.ActualHeight/2 - Height/2));
                }
                label.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                label.SetLocation(-label.ActualWidth/2 + Width/2, Height - 1);
                label.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

/*
        public override void Update()
        {
            Visibility = Visibility.Visible;

            UpdateVisualStates();
            if (_figure != null)
            {
                UpdateContentLayout();
            }

            //            SetToolTip(this, $"{Data.Name}\r\n{Data.PurposeName}\r\nКм. установки: {Data.Km:0.###}\r\n\r\n{stateString}\r\n{Data.ValveMeasuring.StateChangingTimestamp.DisplayValue}");
        }
*/

        protected override void CreateBindings()
        {
            base.CreateBindings();
            SetBinding(TooltipProperty, new Binding(nameof(IValve.Tooltip)) {Mode = BindingMode.OneWay});
        }

        protected override void CaptionChanged()
        {
            /*      if (_label != null)
            {
                _label.Text = Caption;
            }*/
        }

        protected override void UpdateContainerLocation(Grid container)
        {
        }

/*
        protected override void UpdateDisplayElement()
        {
            if (ActualHeight > 0 && ActualWidth > 0)
            {
                var figureBasePoint =
                    _figure.TransformToVisual(this).Transform(new Point(ActualWidth/2, ActualHeight/2));

                RenderTransform = new TranslateTransform {X = -figureBasePoint.X, Y = -figureBasePoint.Y};
            }
        }
*/

        protected void Initialize()
        {
        }

        private static void OnTooltipPropertyChanged(DependencyObject o,
            DependencyPropertyChangedEventArgs e)
        {
            ((ValveWidget) o).OnTooltipChanged((string) e.NewValue);
        }

        private void UpdateVisualStates()
        {
        }

        private void OnTooltipChanged(string newValue)
        {
//            SetToolTip(this, newValue);
        }
    }
}