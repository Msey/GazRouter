using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
namespace GazRouter.Modes.GasCosts2.Behaviors
{
    public class GasCostCellSelectedBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached(); 
            var treeList = AssociatedObject as RadTreeListView;
            if (treeList == null) return;
            //  
            treeList.CurrentCellChanged += TreeListOnCurrentCellChanged;
            treeList.BeginningEdit      += TreeListOnBeginningEdit;
            treeList.CellEditEnded      += TreeListOnCellEditEnded;
        }
        private void TreeListOnBeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
        {
            var tree = sender as RadTreeListView;
            if (tree == null) return;
            var dc = tree.DataContext as ConsumptionsViewModel;
            dc.CaptureSelectedRow();
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            var treeList = AssociatedObject as RadTreeListView;
            if (treeList == null) return;
            //
            treeList.CurrentCellChanged -= TreeListOnCurrentCellChanged;
            treeList.BeginningEdit      -= TreeListOnBeginningEdit;
            treeList.CellEditEnded      -= TreeListOnCellEditEnded;
        }
        private void TreeListOnCellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.EditAction == GridViewEditAction.Cancel) return;
            //
            var tree = sender as RadTreeListView;
            if (tree == null) return;
            var dc = tree.DataContext as ConsumptionsViewModel;
            var editingElement = (RadMaskedNumericInput)e.EditingElement;
            // 
            dc.OnInputMeasuredValue(editingElement.Value ?? 0);
        }
        /// <summary> get cell </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TreeListOnCurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            var newCell = e.NewCell;
            if (newCell == null) return;
            //
            var tree = sender as RadTreeListView;
            if (tree == null) return;
            var dc = tree.DataContext as ConsumptionsViewModel;
            // 
            dc.OnTargetChanged(newCell.Column.DisplayIndex);
            dc.UpdateSelectedItem();
        }
    }
}
#region trash

//treeList.AddingNewDataItem  += TreeListOnAddingNewDataItem;
//private void TreeListOnAddingNewDataItem(object sender,
//    GridViewAddingNewEventArgs gridViewAddingNewEventArgs)
//{
//}

//        private void TreeListOnBeginningEdit(object sender, 
//            GridViewBeginningEditRoutedEventArgs gridViewBeginningEditRoutedEventArgs)
//        {
//        }

//            treeList.BeginningEdit      += TreeListOnBeginningEdit;


//            gridViewCellEditEndedEventArgs.Handled = true;
//
//            tree.ed
//
//            tree.CommitEdit();
// gridViewCellEditEndedEventArgs.NewData = d.;



//            dc.OnInputMeasuredValue((double)gridViewCellEditEndedEventArgs.NewData);

/// <summary> get row </summary>
/// <param name="sender"></param>
/// <param name="gridViewSelectedCellsChangedEventArgs"></param>
//private static void TreeListOnSelectedCellsChanged(object sender,
//     GridViewSelectedCellsChangedEventArgs gridViewSelectedCellsChangedEventArgs)
//{
//}
//
//            var messageOld = oldCell == null ? "" : $"oldCell:  column = {oldCell.Column.Header}, value = {oldCell.Value}";
//            var messageNew = newCell == null ? "" : $"newCell:  column = {newCell.Column.Header}, value = {newCell.Value}";
//            MessageBox.Show($"{messageOld} \n {messageNew}");
#endregion