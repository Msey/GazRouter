using System;
using System.Windows;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus
{
    public partial class Schema
    {
        public event EventHandler<PropertyEventArgs<Rect>> ViewportChanged;

        public event EventHandler VisualChildrenChanged;

        private void OnViewportChanged(Rect oldValue, Rect newValue)
        {
            ViewportChanged?.Invoke(this, new PropertyEventArgs<Rect>(nameof(Viewport), oldValue, newValue));
        }

        private void OnVisualChildrenChanged()
        {
            VisualChildrenChanged?.Invoke(this, System.EventArgs.Empty);
        }
    }
}