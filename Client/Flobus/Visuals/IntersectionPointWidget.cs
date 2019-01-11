using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.UiEntities.FloModel;
using JetBrains.Annotations;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GazRouter.Flobus.Visuals
{
    public class IntersectionPointWidget : PipelineElementWidget, IIntersectionPoint
    {
        public static readonly DependencyProperty TooltipProperty = DependencyProperty.Register(
            nameof(Tooltip), typeof(string), typeof(ValveWidget),
            new PropertyMetadata(string.Empty));

        private bool _isSelected;

        public IntersectionPointWidget([NotNull] PipelineWidget pipelineWidget, double v) : base(pipelineWidget, v)
        {
        }

        public double InnerRadius
        {
            get
            {
                return Pipeline.StrokeThickness;
            }
            
        }

        public double OuterRadius
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public double Rotate
        {
            get
            {
                throw new NotImplementedException();
            }
            
        }

        public string Tooltip
        {
            get { return (string)GetValue(TooltipProperty); }
            set { SetValue(TooltipProperty, value); }
        }

        IPipelineWidget IIntersectionPoint.Pipeline
        {
            get
            {
                return Pipeline;
            }
        }

        IPipelinePoint IIntersectionPoint.PipelinePoint
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void ShowHideKm(bool show)
        {
            // _label.Text = show ? Km.ToString(CultureInfo.InvariantCulture) : Caption;
        }


        protected override void CreateBindings()
        {
            base.CreateBindings();

            SetBinding(IsFoundProperty, new Binding(nameof(ISearchable.IsFound)) { Mode = BindingMode.OneWay });
        }

        protected void Initialize()
        {
        }

        private void UpdateVisualStates()
        {
        }

    }
}
