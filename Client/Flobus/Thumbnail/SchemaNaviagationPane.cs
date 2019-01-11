using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Thumbnail
{
    public class SchemaNaviagationPane : Control
    {
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
           nameof(IsExpanded), typeof (bool), typeof (SchemaNaviagationPane), new PropertyMetadata(false));


        public static readonly DependencyProperty SchemaProperty = DependencyProperty.Register(
           nameof(Schema), typeof (Schema), typeof (SchemaNaviagationPane), new PropertyMetadata(null));

        public Schema Schema
        {
            get { return (Schema) GetValue(SchemaProperty); }
            set { SetValue(SchemaProperty, value); }
        }

        private RadSlider _slider;

        public bool IsExpanded
        {
            get { return (bool) GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }


        public SchemaNaviagationPane()
        {
            DefaultStyleKey = typeof (SchemaNaviagationPane);
        }


        public override void OnApplyTemplate()
        {
            _slider = GetTemplateChild("ZoomSlider") as RadSlider;
            RefreshZoomSlider();
        }

        private void RefreshZoomSlider()
        {
            if (_slider != null)
            {
                _slider.Minimum = DiagramConstants.MinimumZoom;
                _slider.Maximum = DiagramConstants.MaximumZoom;
            }
        }
    }

}