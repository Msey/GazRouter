using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace GazRouter.Common.Ui.Behaviors
{
    public class ActivateTabBehavior : Behavior<RadTabControl>
    {
        private FrameworkElement _currentContentElement;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewSelectionChanged += TabControlPreviewSelectionChanged;
            AssociatedObject.SelectionChanged += TabControlSelectionChanged;
        }


        protected override void OnDetaching()
        {
            AssociatedObject.PreviewSelectionChanged -= TabControlPreviewSelectionChanged;
            AssociatedObject.SelectionChanged -= TabControlSelectionChanged;
            base.OnDetaching();
        }

        private void TabControlPreviewSelectionChanged(object sender, RadSelectionChangedEventArgs e)
        {
            if (_currentContentElement != null)
            {
                _currentContentElement.DataContextChanged -= SelectedContentDataContextChanged;
                CurrentTabItem?.Deactivate();
            }
        }


        private void TabControlSelectionChanged(object sender, RadSelectionChangedEventArgs e)
        {
            _currentContentElement = AssociatedObject.SelectedContent as FrameworkElement;
            if (_currentContentElement != null)
            {
                _currentContentElement.DataContextChanged += SelectedContentDataContextChanged;
                CurrentTabItem?.Activate();
            }
        }

        private void SelectedContentDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           
            if (_currentContentElement == sender)
            {
                CurrentTabItem?.Activate();
               
            }



        }

        private ITabItem CurrentTabItem
        {
            get
            {
                var currentTabItem = _currentContentElement?.DataContext as ITabItem;
                return currentTabItem;
            }
        }
    }

    public interface ITabItem
    {
        void Activate();
        void Deactivate();
    }
}