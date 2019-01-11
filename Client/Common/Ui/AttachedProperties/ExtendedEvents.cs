using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using PropertyMetadata = System.Windows.PropertyMetadata;

namespace GazRouter.Common.Ui.AttachedProperties
{
    public static class ExtendedEvents
    {
        public static ICommand GetMouseLeftDoubleClickEvent(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseLeftDoubleClickEventProperty);
        }

        public static void SetMouseLeftDoubleClickEvent(DependencyObject obj, ICommand value)
        {   
            obj.SetValue(MouseLeftDoubleClickEventProperty, value);
        }

            // Using a DependencyProperty as the backing store for MouseLeftDoubleClickEvent.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MouseLeftDoubleClickEventProperty =
                DependencyProperty.RegisterAttached("MouseLeftDoubleClickEvent", typeof(ICommand), typeof(ExtendedEvents), new PropertyMetadata(MouseLeftDoubleClickEventChangedCallback));

            private static readonly Dictionary<FrameworkElement, bool> Elements = new Dictionary<FrameworkElement, bool>();

        private static void MouseLeftDoubleClickEventChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var radGridView = dependencyObject as RadGridView;

            if (radGridView == null) return;
            
            var command = e.NewValue as ICommand;

            if (command == null) return;
            
            SetMouseLeftDoubleClickEvent(radGridView, command);
            
            if (Elements.ContainsKey(radGridView)) return;
            
            radGridView.CurrentCellChanged += RadGridViewCurrentCellChanged;
            Elements.Add(radGridView, true);
        }

        private static readonly object Lock = new object();

                static void RadGridViewCurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
                {
                    lock (Lock)
                    {
                        if (e.OldCell != null) e.OldCell.CellDoubleClick -= CellDoubleClick;
                        if (e.NewCell != null) e.NewCell.CellDoubleClick += CellDoubleClick;    
                    }
                }

        static void CellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            e.Handled = true;
            GridViewDataControl gridViewDataControl = ((GridViewCell) sender).ParentRow.GridViewDataControl;
            ICommand mouseLeftDoubleClickEvent = GetMouseLeftDoubleClickEvent(gridViewDataControl);
            if (mouseLeftDoubleClickEvent.CanExecute(null)) mouseLeftDoubleClickEvent.Execute(null);
        }

    }

}