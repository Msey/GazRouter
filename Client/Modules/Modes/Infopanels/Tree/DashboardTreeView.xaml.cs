using GazRouter.DTO.Dashboards;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeView;
using Telerik.Windows.DragDrop;

namespace GazRouter.Modes.Infopanels.Tree
{
    // Drag Source Events:
    // 1. DragInitialize - The DragInitialize event occurs when an object is about to be dragged.
    //    All needed information about the drag should be passed to the event arguments.
    //    Drag start can be stopped by setting Cancel = true. This is a bubbling event.
    // 2. GiveFeedback - This event occurs continuously during a drag-and-drop operation and enables 
    //    the drop source to give feedback information to the user. This feedback is commonly given by 
    //    changing the appearance of the mouse pointer to indicate the effects allowed by the drop target.
    //    This is a bubbling event.
    // 3. QueryContinueDrag - This event occurs when there is a change in the keyboard or mouse button states
    //    during a drag-and-drop operation and enables the drop source to cancel the drag-and-drop operation depending
    //    on the key/button states. This is a bubbling event.
    // 4. PreviewGiveFeedback - Tunneling version of GiveFeedback.
    // 5. PreviewQueryContinueDrag - Tunneling version of QueryContinueDrag.
    // 6. DragDropCompleted - This event occurs when an object is dropped on the drop target and is used to 
    //    notify source for end of the drag operation.This is a bubbling event.
    // =======================================================================================================================
    //    Drop Target Events:
    //DragEnter - This event occurs when an object is dragged into the 
    //        drop target's boundary. This is a bubbling event.
    //DragLeave - This event occurs when an object is dragged out of 
    //        the drop target's boundary. This is a bubbling event.
    //DragOver - This event occurs continuously while an object is 
    //        dragged(moved) within the drop target's boundary. This is a bubbling event.
    //Drop - This event occurs when an object is dropped on the drop target.This is a bubbling event.
    //PreviewDragEnter - Tunneling version of DragEnter.
    //PreviewDragLeave - Tunneling version of DragLeave.
    //PreviewDragOver - Tunneling version of DragOver.
    //PreviewDrop - Tunneling version of Drop.
    /// <summary>
    /// 
    /// операции:
    /// -------- 
    ///                     | Target  |  Items  |
    /// --------------------|---------|---------|
    /// 1. Move.Inside [MI] |   F,    |  F,D,E  |
    /// 2. Move.After  [MA] |  F,D,E  |  F,D,E  | 
    /// 3. Move.Before [MB] |  F,D,E  |  F,D,E  | 
    /// 
    /// </summary>
    public partial class DashboardTreeView
    {
#region constructor
        public DashboardTreeView()
        {
            InitializeComponent();
            //
            Loaded += (sender, args) =>
            {
                var model = DataContext as DashboardTreeViewModel;
                if (model == null || !model.IsDragAndDropEnable()) return;
                //
                RadTreeView1.IsDragDropEnabled = true;
                SubscribeDragDrop();
            };
        }
#endregion
#region variables
        private const string DragOptionsName = "TreeViewDragDropOptions";
#endregion
#region events
        private void OnDragInitialized(object sender, DragInitializeEventArgs e)
        {  
        }
        /// <summary> 
        /// 
        /// правила:
        /// 
        /// cancelationToken = options.DropAction = DropAction.None;
        /// 1. если целевой объект для перемещения не выбран                  - операция отменяется (move inside, after|before)
        /// 2. если целевой объект не является папкой, и операция Move.Inside - операция отменяется (move inside, after|before)
        ///  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnElementDrop(object sender, DragEventArgs e)
        {
            var options = DragDropPayloadManager.GetDataFromObject(e.Data, DragOptionsName) 
                          as TreeViewDragDropOptions;
            if (options == null) return;
            // 1
            if (options.DropTargetItem?.Item == null) // объект target - не выбран
            {
                options.DropAction = DropAction.None;
                return;
            }
            // 2
            var destinationItem = options.DropTargetItem.Item as ItemBase;
            if (options.DropPosition == DropPosition.Inside)
                if (destinationItem?.ContentType != InfopanelItemType.Folder)                
                    options.DropAction = DropAction.None;               
        }
        private void OnDragDropCompleted(object sender, DragDropCompletedEventArgs e)
        {
            var options = DragDropPayloadManager.GetDataFromObject(e.Data, DragOptionsName)
                          as TreeViewDragDropOptions;
            if (options == null || 
                options.DropAction == DropAction.None ||
                options.DropTargetItem == null) return;
            //
            var item   = options.DragSourceItem.Item as ItemBase;
            var target = options.DropTargetItem.Item as ItemBase;// может быть как Folder так и ItemContext
            if (item == null || target == null) return;
            //
            item.Drop(target, options.DropPosition);
        }
#endregion
#region methods
        private void SubscribeDragDrop()
        {
            DragDropManager.AddDragInitializeHandler(RadTreeView1, OnDragInitialized, true);
            DragDropManager.AddDropHandler(RadTreeView1, OnElementDrop, true);
            DragDropManager.AddDragDropCompletedHandler(RadTreeView1, OnDragDropCompleted, true);
        }
#endregion
    }
}
#region trash
//            Predicate<ItemBase> predicate = itemBase =>
//            {
//                return itemBase?.ContentType == InfopanelItemType.Folder;
//            };
//            if (!predicate(destinationItem)) return;
//            // 
//            if (options.DropPosition == DropPosition.Inside)
//                options.DropAction = DropAction.None;


//private void UnSubscribeDragDrop()
//{
//    DragDropManager.RemoveDragInitializeHandler(RadTreeView1, OnDragInitialized);
//    DragDropManager.RemoveDropHandler(RadTreeView1, OnElementDrop);
//    DragDropManager.RemoveDragDropCompletedHandler(RadTreeView1, OnDragDropCompleted);
//    //            DragDropManager.RemoveGiveFeedbackHandler(RadTreeView, OnGiveFeedBack);
//    //            DragDropManager.RemoveQueryContinueDragHandler(RadTreeView, OnQueryContinue);
//    //            DragDropManager.RemoveDragOverHandler(RadTreeView, OnElementDragOver);
//    //            DragDropManager.RemoveDragLeaveHandler(RadTreeView, OnElementDragLeave);
//}


//        private void OnGiveFeedBack(object sender, GiveFeedbackEventArgs args)
//        {
//            _sb.AppendLine($"{_i++}.OnGiveFeedBack");
//            args.SetCursor(Cursors.Arrow);
//            args.Handled = true;
//        }
//        private void OnQueryContinue(object sender, QueryContinueDragEventArgs args)
//        {
//            _sb.AppendLine($"{_i++}.OnQueryContinue");
//        }
//        private void OnElementDragOver(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
//        {
//            _sb.AppendLine($"{_i++}.OnElementDragOver");
//        }
//        private void OnElementDragLeave(object sender, Telerik.Windows.DragDrop.DragEventArgs args)
//        {
//            _sb.AppendLine($"{_i++}.OnElementDragLeave");
//        }


//            DragDropManager.AddGiveFeedbackHandler(RadTreeView, OnGiveFeedBack, true);
//            DragDropManager.AddQueryContinueDragHandler(RadTreeView, OnQueryContinue, true);
//            DragDropManager.AddDragOverHandler(RadTreeView, OnElementDragOver, true);
//            DragDropManager.AddDragLeaveHandler(RadTreeView, OnElementDragLeave, true);

//#region finalizer
//~DashboardTreeView()
//{
// UnSubscribeDragDrop();?
//}
//#endregion


// todo: запретить перемещение в ItemContent!
// типы перемещений: - перемещение объекта производится только в папку

// перемещение в папку - чистое
// 1. перемещение папки в папку
// 2. 
// 3.  
//            var options = DragDropPayloadManager.GetDataFromObject(e.Data, DraOptionsName) 
//                as TreeViewDragDropOptions;
//            var isHandled = e.Handled;
//            if (options == null) {
//                e.Handled = true;
//                return;
//            }
//            //
//            var destinationItem = options.DropTargetItem.Item;
//            var relocatableItem = options.DragSourceItem.Item;
//            if (destinationItem is FolderItem)
//            {
//                var item = options.DragSourceItem.Item;
//                var iBase = (ItemBase)item;
//                iBase.Delete();
//            }
//            else
//            {
//                e.Handled = true;
//            }
#region folder_drop
//        public override async void Drop(object obj)
//        {
//            var e = (RadTreeViewDragEndedEventArgs)obj;
//            if (e.TargetDropItem.Item is ItemContent) return;
//            //
//            var targetFolder = e.TargetDropItem.Item as FolderItem;
//            if (targetFolder?.Id == Dto.ParentId) return;
//            // 
//            await new DashboardServiceProxy()
//                .EditFolderAsync(new EditFolderParameterSet
//                {
//                    FolderId = Id,
//                    Name = Name,
//                    ParentId = targetFolder?.Id
//                });
//        }
#endregion
#region dash
//        public override async void Drop(object obj)
//        {
//            var e = (RadTreeViewDragEndedEventArgs)obj;
//            if (e.TargetDropItem.Item is ItemContent) return;
//            //
//            var targetFolder = e.TargetDropItem.Item as FolderItem;
//            if (targetFolder?.Id == Dto.FolderId) return;
//            // 
//            await new DashboardServiceProxy()
//                .EditDashboardAsync(new EditDashboardParameterSet
//                {
//                    DashboardId = Id,
//                    FolderId = targetFolder?.Id,
//                    DashboardName = Dto.DashboardName,
//                    PeriodTypeId = Dto.PeriodTypeId
//                });
//        }
#endregion
#region excel
//        public override async void Drop(object obj)
//        {
//            var e = (RadTreeViewDragEndedEventArgs)obj;
//            if (e.TargetDropItem.Item is ItemContent) return;
//            //
//            var targetFolder = e.TargetDropItem.Item as FolderItem;
//            if (targetFolder?.Id == Dto.FolderId) return;
//            // 
//            await new ExcelReportServiceProxy()
//                .EditDashboardAsync(new EditDashboardParameterSet
//                {
//                    DashboardId = Id,
//                    FolderId = targetFolder?.Id,
//                    DashboardName = Dto.DashboardName,
//                    PeriodTypeId = Dto.PeriodTypeId
//                });
//        }
#endregion
//            var options = DragDropPayloadManager.GetDataFromObject(e.Data, DraOptionsName)
//                          as TreeViewDragDropOptions;
//            var destinationItem = options.DropTargetItem.Item;
//            if (destinationItem is FolderItem)
//            {
//
//            }
//            else
//            {
//                e.Handled = true;
//            }
//            #region trash
//            _sb.AppendLine($"{_i++}.OnElementDrop");
//            var destinationItem = (e.OriginalSource as FrameworkElement).ParentOfType<RadTreeViewItem>();
//            var options = DragDropPayloadManager.GetDataFromObject(e.Data, DraOptionsName) as TreeViewDragDropOptions;
//            if (options == null)
//            {
//                e.Handled = true;
//                return;
//            }
//            //
//            var dropPosition = options.DropPosition;
//            var data = e.Data;
//            var dropEffects = e.Effects;
//            var dragDropEffects = dropEffects;
//            var originalSource = e.OriginalSource;
//            var source = e.Source;
//            var handled = e.Handled;
//            #endregion

//_sb.AppendLine($"{_i++}.OnDragDropCompleted");
// todo: сделать асинхронный вызов сервиса
//_sb.Clear();
//            _sb.AppendLine($"{_i++}.OnDragInitialized");
//            var selectedItem = (sender as RadTreeView).SelectedItem;
//
//var source = e.Source;


//            var payload = DragDropPayloadManager.GeneratePayload(null);
//            e.Data = ((FrameworkElement)e.OriginalSource).DataContext;
//            e.DragVisual = new ContentControl {
//                Content = e.Data,
//                ContentTemplate = this.Resources["DraggedItemTemplate"] as DataTemplate
//            };

//        private void RadTreeView_OnPreviewDragStarted(object sender, RadTreeViewDragEventArgs e)
//        {
//            var f = sender;
//            var p = e;
//        }
//        private void RadTreeView_OnDragStarted(object sender, RadTreeViewDragEventArgs e)
//        {
//            throw new System.NotImplementedException();
//        }
//        private void RadTreeView_OnPreviewDragEnded(object sender, RadTreeViewDragEndedEventArgs e)
//        {
//            throw new System.NotImplementedException();
//        }
//        private void RadTreeView_OnDragEnded(object sender, RadTreeViewDragEndedEventArgs e)
//        {
//            throw new System.NotImplementedException();
//        }

// using Telerik.Windows.Controls;
// private void RadTreeView_PreviewDragStarted(object sender, RadTreeViewDragEventArgs e)
// {
//            if (e.DraggedItems.Count == 1 && e.DraggedItems[0] is FolderItem)
//                e.Handled = true;
// }
#endregion
