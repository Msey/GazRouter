using System.Windows;
using Telerik.Windows.DragDrop.Behaviors;
namespace GazRouter.Controls
{
    public class EntityPickerSearchTreeVisibility
    {
#region dependencyProperty
        public static readonly DependencyProperty CollapseProperty =
            DependencyProperty.RegisterAttached("Collapse",
                                                typeof(bool), typeof(EntityPickerSearchTreeVisibility),
                                                new PropertyMetadata(SearchTreeVisibilityChanged));
        public static bool GetCollapse(DependencyObject obj)
        {
            return (bool) obj.GetValue(CollapseProperty);
        }
        public static void SetCollapse(DependencyObject obj, bool value)
        {
            obj.SetValue(CollapseProperty, value);
        }
        private static void SearchTreeVisibilityChanged(DependencyObject obj, 
                                                        DependencyPropertyChangedEventArgs e)
        {
            var entityPicker = obj as EntityPicker;
            //
            if(entityPicker != null)
                entityPicker.SearchTreeMunuItem.Visibility = Visibility.Collapsed;
        }
#endregion
    }
}
