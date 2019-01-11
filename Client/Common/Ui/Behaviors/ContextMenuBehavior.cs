using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using SelectionMode = System.Windows.Controls.SelectionMode;
using ViewModelBase = GazRouter.Common.ViewModel.ViewModelBase;

namespace GazRouter.Common.Ui.Behaviors
{
    public class ContextMenuBehavior : ViewModelBase
    {
        public static readonly DependencyProperty ContextMenuProperty =
            DependencyProperty.RegisterAttached("ContextMenu", typeof(FrameworkElement), typeof(ContextMenuBehavior),
                new PropertyMetadata(ContextMenuChanged));

        private readonly Control _control;

        public ContextMenuBehavior(Control control, FrameworkElement contextMenu)
        {
            _control = control;

            ((RadContextMenu) contextMenu).Opened += RadContextMenu_Opened;
        }

        public static void SetContextMenu(DependencyObject dependencyObject, FrameworkElement contextmenu)
        {
            dependencyObject.SetValue(ContextMenuProperty, contextmenu);
        }

        public static FrameworkElement GetContextMenu(DependencyObject dependencyObject)
        {
            return (FrameworkElement) dependencyObject.GetValue(ContextMenuProperty);
        }

        public static void ContextMenuChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as Control;
            var contextMenu = e.NewValue as FrameworkElement;

            if ((control is GridViewDataControl || control is RadTreeView) && contextMenu != null)
            {
                new ContextMenuBehavior(control, contextMenu);
            }
        }

        private void RadContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var menu = (RadContextMenu) sender;

            var gridViewDataControl = _control as GridViewDataControl;
            if (gridViewDataControl != null)
            {
                switch (gridViewDataControl.SelectionUnit)
                {
                    case GridViewSelectionUnit.FullRow:
                        var row = menu.GetClickedElement<GridViewRow>();

                        if (row != null)
                        {
                            row.IsSelected = row.IsCurrent = true;
                            return;
                        }

                        break;
                    case GridViewSelectionUnit.Cell:
                        var cell = menu.GetClickedElement<GridViewCell>();
                        if (cell != null)
                        {
                            if (gridViewDataControl.CurrentCell != null)
                            {
                                gridViewDataControl.CurrentCell.IsCurrent = false;
                            }
                            if (gridViewDataControl.SelectionMode == SelectionMode.Single)
                            {
                                gridViewDataControl.SelectedCells.Clear();
                                gridViewDataControl.SelectedItems.Clear();
                            }
                            var gridViewCellInfo = new GridViewCellInfo(cell);
                            gridViewDataControl.SelectedCells.Add(gridViewCellInfo);
                            cell.IsCurrent = true;
                            gridViewDataControl.UpdateLayout();
                            return;
                        }
                        break;
                }
                menu.IsOpen = false;
                return;
            }

            var tree = _control as RadTreeView;
            if (tree != null)
            {
                var item = menu.GetClickedElement<RadTreeViewItem>();

                if (item != null)
                {
                    item.IsSelected = true;
                    return;
                }
                menu.IsOpen = false;
            }
        }
    }
}