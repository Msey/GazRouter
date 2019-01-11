using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.Flobus.Extensions;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.UiEntities.FloModel;
using JetBrains.Annotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace GazRouter.Flobus.Visuals
{
    public class PlugWidget : PipelineElementWidget
    {
        public static readonly DependencyProperty TooltipProperty = DependencyProperty.Register(
            nameof(Tooltip), typeof(string), typeof(ValveWidget),
            new PropertyMetadata(string.Empty, OnTooltipPropertyChanged));

        private bool _isSelected;
       
        public PlugWidget([NotNull] PipelineWidget pipelineWidget, double v) : base(pipelineWidget, v)
        {
        }

        public string Tooltip
        {
            get { return (string)GetValue(TooltipProperty); }
            set { SetValue(TooltipProperty, value); }
        }

        public override void ShowHideKm(bool show)
        {
            // _label.Text = show ? Km.ToString(CultureInfo.InvariantCulture) : Caption;
        }               
        
        protected void Initialize()
        {
        }

        private static void OnTooltipPropertyChanged(DependencyObject o,
            DependencyPropertyChangedEventArgs e)
        {
            ((PlugWidget)o).OnTooltipChanged((string)e.NewValue);
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
