using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ASDU;
using GazRouter.DTO.ASDU;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace DataExchange.ASDU
{
    public class AsduDataViewModel : LockableViewModel
    {
        private const string LinkStatesAll = "all";
        private const string LinkStatesLinked = "linked";
        private const string LinkStatesNotlinked = "notlinked";


        public DelegateCommand LoadedCommand { get; }
        public DelegateCommand RefreshCommand { get; }
        public DelegateCommand AsduTreeExpandedCommand { get; }
        public DelegateCommand IusTreeExpandedCommand { get; }
        public DelegateCommand LinkCommand { get; }
        public DelegateCommand UnLinkCommand { get; }
        public DelegateCommand ShowRequestsCommand { get; }

        public ObservableCollection<DataTreeMenuItem> IusMenuItems { get; } =
            new ObservableCollection<DataTreeMenuItem>();
        public ObservableCollection<DataTreeMenuItem> AsduMenuItems { get; } =
           new ObservableCollection<DataTreeMenuItem>();
        public ObservableCollection<DataTreeMenuItem> SubIusMenuItems { get; } =
            new ObservableCollection<DataTreeMenuItem>();
        public ObservableCollection<DataTreeMenuItem> SubAsduMenuItems { get; } =
    new ObservableCollection<DataTreeMenuItem>();


       

        private List<NodeBinding> _possibleRootTypes = new List<NodeBinding>();

        public List<NodeBinding> PossibleRootTypes
        {
            get { return _possibleRootTypes; }
            set { SetProperty(ref _possibleRootTypes, value); }
        }

        private NodeBinding _selectedRootTypes;

        public NodeBinding SelectedRootTypes
        {
            get { return _selectedRootTypes; }
            set
            {
                if (SetProperty(ref _selectedRootTypes, value) && _selectedRootTypes != null)
                {
                    LoadTrees();
                }
            }
        }


        public AsduDataViewModel()
        {
            LoadedCommand = new DelegateCommand(Init);
            RefreshCommand = new DelegateCommand(LoadTrees);
            AsduTreeExpandedCommand = new DelegateCommand(AsduTreeExpanded);
            IusTreeExpandedCommand = new DelegateCommand(IusTreeExpanded);
            LinkCommand = new DelegateCommand(() => ManageLink(true));
            UnLinkCommand = new DelegateCommand(() => ManageLink(false));
            ShowRequestsCommand = new DelegateCommand(ShowRequests);
           

            _iusTreeRootItemsSource.Filter += TreeFilter;
            _asduTreeRootItemsSource.Filter += TreeFilter;

            LinkStates = new List<DictionaryEntry>
            {
                new DictionaryEntry {Key = LinkStatesAll, Value = "Все"},
                new DictionaryEntry {Key = LinkStatesLinked, Value = "Связанные"},
                new DictionaryEntry {Key = LinkStatesNotlinked, Value = "Несвязанные"},
            };
            IusLinkState = LinkStates[0].Key;
            AsduLinkState = LinkStates[0].Key;
        }

        private void ShowRequests()
        {
            var acrv = new AsduChangeRequestView {DataContext = new AsduChangeRequestViewModel(SelectedRequest)};
            acrv.Closed += async (a, b) =>
            {
                var toSelect = SelectedRequest;
                await LoadRequests();
                SelectedRequest = AsduRequests.FirstOrDefault(r => r.Key == toSelect?.Key);
            };
            acrv.ShowDialog();
        }

        private async Task LoadRequests()
        {
            var p = new ASDUServiceProxy();
            var tRequests = p.GetLoadedFilesAsync(new GetLoadedFilesParam {LoadedFilesType = LoadedFilesType.Output});
            await tRequests;
            AsduRequests = LoadedFileWrapper.FromLoadedFiles(tRequests.Result);
            OnPropertyChanged(() => AsduRequests);
        }

        private async void ManageLink(bool link)
        {
            if (link && !CanLink || !link && !CanUnLink)
            {
                return;
            }

            Behavior.TryLock();
            try
            {
                var p = new ASDUServiceProxy();
                await p.ManageLinkAsync(new LinkParams
                {
                    IusType = SelectedIusItem.RawType,
                    IusId = SelectedIusItem.RawId,
                    AsduType = SelectedAsduItem.RawType,
                    AsduId = SelectedAsduItem.RawId,
                    LinkAction = link ? LinkAction.LinkObject : LinkAction.UnLinkObject
                });

                var updatedTaskIus = p.GetDataTreeNodesAsync(new DataTreeParams
                {
                    IsIus = true,
                    RootId = SelectedRootTypes.IusId,
                    SingleNodeType = SelectedIusItem.RawType,
                    SingleNodeId = SelectedIusItem.RawId
                });

                var updatedTaskAsdu = p.GetDataTreeNodesAsync(new DataTreeParams
                {
                    IsIus = false,
                    RootId = SelectedRootTypes.AsduId,
                    SingleNodeType = SelectedAsduItem.RawType,
                    SingleNodeId = SelectedAsduItem.RawId
                });

                await TaskEx.WhenAll(updatedTaskIus, updatedTaskAsdu);

                SelectedIusItem.UpdateSelfAndChildren(updatedTaskIus.Result);
                SelectedAsduItem.UpdateSelfAndChildren(updatedTaskAsdu.Result);

                if (IsInLinkDisplayMode)
                {
                    OnPropertyChanged(() => IsInLinkDisplayMode);
                }
            }
            finally
            {
                UpdateCanLink();
                UpdateMenus();
                Behavior.TryUnlock();
            }
        }

        private void AddNodeToRequest(DataTreeItem selectedItem, AsduRequestKind asduRequestKind)
        {
            Behavior.TryLock();
            try
            {
                var p = new ASDUServiceProxy();
                p.AddAsduRequestAsync(new AsduRequestParams
                {
                    Id = selectedItem.RawId,
                    Type = selectedItem.RawType,
                    RequestKey = SelectedRequest.Key,
                    RequestKind = asduRequestKind,
                    EntityId = (selectedItem.NodeType == DataTreeNodeType.IusObj ||
                                selectedItem.NodeType == DataTreeNodeType.AsduObj)
                        ? selectedItem.RawId
                        : selectedItem.Parent?.RawId
                });
                // TODO: refresh node
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private void UpdateMenus()
        {
            UpdateIusMenu();
            UpdateAsduMenu();
                }

        private async void Init()
        {
            Behavior.TryLock();
            try
            {
                var p = new ASDUServiceProxy();
                var tRootTypes = p.GetPossibleDataTreeRootsAsync();
                var tRequests = LoadRequests();
                await TaskEx.WhenAll(tRequests, tRootTypes);
                SelectedRequest = AsduRequests.FirstOrDefault();
                PossibleRootTypes = await p.GetPossibleDataTreeRootsAsync();
                SelectedRootTypes = PossibleRootTypes.FirstOrDefault();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }


        private void AsduTreeExpanded()
        {
            SelectLinkedAsduItem();
        }

        private void IusTreeExpanded()
        {
            SelectLinkedIusItem();
        }


        private void LoadTrees()
        {
            LoadIusTree();
            LoadAsduTree();
        }

        private async void LoadIusTree()
        {
            IsIusTreeBusy = true;
            try
            {
                var p = new ASDUServiceProxy();
                var res = await p.GetDataTreeNodesAsync(new DataTreeParams
                {
                    IsIus = true,
                    RootId = SelectedRootTypes?.IusId
                });
                IusTreeRootItems = res.BuildTreeFromList(this, true);
            }
            finally
            {
                IsIusTreeBusy = false;
            }
        }


        private async void LoadAsduTree()
        {
            IsAsduTreeBusy = true;
            try
            {
                var p = new ASDUServiceProxy();
                var res = await p.GetDataTreeNodesAsync(new DataTreeParams
                {
                    IsIus = false,
                    RootId = SelectedRootTypes?.AsduId
                });
                AsduTreeRootItems = res.BuildTreeFromList(this, false);
            }
            finally
            {
                IsAsduTreeBusy = false;
            }
        }

        public bool CanLink => SelectedIusItem != null && SelectedAsduItem != null && !SelectedIusItem.IsLinked &&
                               !SelectedAsduItem.IsLinked && SelectedIusItem.NodeType == DataTreeNodeType.IusObj &&
                               SelectedAsduItem.NodeType == DataTreeNodeType.AsduObj;

        public bool CanUnLink => SelectedIusItem != null && SelectedAsduItem != null && SelectedIusItem.IsLinked &&
                                 SelectedAsduItem.IsLinked && SelectedIusItem.NodeType == DataTreeNodeType.IusObj &&
                                 SelectedAsduItem.NodeType == DataTreeNodeType.AsduObj && SelectedIusItem.LinkedId ==
                                 SelectedAsduItem.Id && SelectedAsduItem.LinkedId == SelectedIusItem.Id;

        private IEnumerable<AsduRequestKind> GetPossibleRequests(DataTreeItem item)
        {
            var res = new List<AsduRequestKind>();
            switch (item?.NodeType)
            {
                case DataTreeNodeType.IusObj:
                    if (!item.IsLinked)
                    {
                        res.Add(AsduRequestKind.IusEntityCreate);
                        res.Add(AsduRequestKind.IusEntityCreateAll);
                    }
                    else
                    {
                        if (item.Equality.HasValue && item.Equality.Value == DataValueEquality.NotEqual)
                        {
                            res.Add(AsduRequestKind.IusEntityUpdateName);
                            res.Add(AsduRequestKind.IusEntityUpdateDesc);
                        }

                        if (item.Children.Any(c => c.Equality.HasValue && c.Equality == DataValueEquality.NotEqual))
                        {
                            res.Add(AsduRequestKind.IusEntityUpdateAll);
                        }
                    }

                    break;
                case DataTreeNodeType.IusAttr:
                case DataTreeNodeType.IusAttrLink:
                    if (item.IsLinked)
                    {
                        if (item.Equality.HasValue && item.Equality.Value == DataValueEquality.NotEqual)
                        {
                            res.Add(AsduRequestKind.IusAttrUpdateValue);
                        }
                    }

                    break;
                case DataTreeNodeType.IusParam:
                    break;
                case DataTreeNodeType.AsduObj:
                    break;
                case DataTreeNodeType.AsduAttr:
                    break;
                case DataTreeNodeType.AsduAttrLink:
                    break;
                case DataTreeNodeType.AsduParam:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return res;
        }

        private bool _isIusTreeBusy;

        public bool IsIusTreeBusy
        {
            get { return _isIusTreeBusy; }
            set { SetProperty(ref _isIusTreeBusy, value); }
        }

        private bool _isAsduTreeBusy;

        public bool IsAsduTreeBusy
        {
            get { return _isAsduTreeBusy; }
            set { SetProperty(ref _isAsduTreeBusy, value); }
        }


        private void UpdateCanLink()
        {
            OnPropertyChanged(() => CanLink);
            OnPropertyChanged(() => CanUnLink);
        }

        private DataTreeItem _selectedIusItem;

        public DataTreeItem SelectedIusItem
        {
            get { return _selectedIusItem; }
            set
            {
                if (SetProperty(ref _selectedIusItem, value))
                {
                    SelectLinkedAsduItem();
                    UpdateCanLink();
                    UpdateMenus();
                }
            }
        }

       
       
        private void UpdateIusMenu()
        {
            IusMenuItems.Clear();
            SubIusMenuItems.Clear();
            if (SelectedIusItem == null)
            {
                return;
            }
            SubIusMenuItems.Add(new DataTreeMenuItem
            {
                Text = "Копировать название",
                Command = new DelegateCommand(() => Clipboard.SetText(SelectedIusItem.Name))

            });
            SubIusMenuItems.Add(new DataTreeMenuItem
            {
                Text = "Копировать значение",
                Command = new DelegateCommand(() => Clipboard.SetText(SelectedIusItem.Value))

            });
            SubIusMenuItems.Add(new DataTreeMenuItem
            {
                Text = "Копировать ID",
                Command = new DelegateCommand(() => Clipboard.SetText(SelectedIusItem.RawId))

            });
            var CopyMenuItem = new DataTreeMenuItem { Text = "Копировать" };
            CopyMenuItem.SubItems = SubIusMenuItems;
            IusMenuItems.Add(CopyMenuItem);
            if (CanLink)
            {
                IusMenuItems.Add(new DataTreeMenuItem
                {
                    Text = "Связать с: " + SelectedAsduItem?.Name,
                    Command = LinkCommand
                });
            }

            if (CanUnLink)
            {
                IusMenuItems.Add(new DataTreeMenuItem
                {
                    Text = "Разорвать связь c: " + SelectedAsduItem?.Name,
                    Command = UnLinkCommand
                });
            }

            if (SelectedRequest == null)
            {
                IusMenuItems.Add(new DataTreeMenuItem {Text = "Выберите заявку для добавления", IsEnabled = false});
            }
            else
            {
                foreach (var asduRequestKind in GetPossibleRequests(SelectedIusItem))
                {
                    IusMenuItems.Add(new DataTreeMenuItem
                    {
                        Text = RequestKindToText(asduRequestKind),
                        Command = new DelegateCommand(() => AddNodeToRequest(SelectedIusItem, asduRequestKind))
                    });
                }
            }
        }


        private string RequestKindToText(AsduRequestKind asduRequestKind)
        {
            switch (asduRequestKind)
            {
                case AsduRequestKind.IusEntityCreate:
                    return "Добавить в заявку: создать";
                case AsduRequestKind.IusEntityCreateAll:
                    return "Добавить в заявку: создать с атрибутами и параметрами";
                case AsduRequestKind.IusEntityUpdateName:
                    return "Добавить в заявку: изменить название";
                case AsduRequestKind.IusEntityUpdateDesc:
                    return "Добавить в заявку: изменить описание";
                case AsduRequestKind.IusEntityUpdateAll:
                    return "Добавить в заявку: изменить все";
                case AsduRequestKind.IusAttrUpdateValue:
                    return "Добавить в заявку: изменить значение";
                case AsduRequestKind.IusParamCreate:
                    return "Добавить в заявку: создать параметр";
                case AsduRequestKind.AsduEntityDelete:
                    return "Добавить в заявку: удалить";
                default:
                    throw new ArgumentOutOfRangeException(nameof(asduRequestKind));
            }
        }

        private void SelectLinkedAsduItem()
        {
            if (IsInLinkDisplayMode)
            {
                var asduItem = SelectedIusItem == null
                    ? null
                    : AsduTreeRootItems.FindItem(i => i.Id == _selectedIusItem.LinkedId);
                SelectedAsduItem = asduItem;
                OnPropertyChanged(() => SelectedAsduItem);
                asduItem?.ExpandToRoot();
                OnPropertyChanged(() => SelectedAsduItem);
            }
        }


        private DataTreeItem _selectedAsduItem;

        public DataTreeItem SelectedAsduItem
        {
            get { return _selectedAsduItem; }
            set
            {
                if (SetProperty(ref _selectedAsduItem, value))
                {
                    SelectLinkedIusItem();
                    UpdateCanLink();
                    UpdateMenus();
                }
            }
        }

      
        private void UpdateAsduMenu()
        {
            AsduMenuItems.Clear();
            SubAsduMenuItems.Clear();
            if (SelectedAsduItem == null)
            {
                return;
            }
           
            if (SelectedIusItem == null)
            {
                return;
            }
            SubAsduMenuItems.Add(new DataTreeMenuItem
            {
                Text = "Копировать название",
                Command = new DelegateCommand(() => Clipboard.SetText(SelectedAsduItem.Name))
            });
            SubAsduMenuItems.Add(new DataTreeMenuItem
            {
                Text = "Копировать значение",
                Command = new DelegateCommand(() => Clipboard.SetText(SelectedAsduItem.Value))
            });
            SubAsduMenuItems.Add(new DataTreeMenuItem
            {
                Text = "Копировать GUID",
                Command = new DelegateCommand(() => Clipboard.SetText(SelectedAsduItem.RawId))
            });
            var CopyMenuItem = new DataTreeMenuItem { Text = "Копировать" };
            CopyMenuItem.SubItems = SubAsduMenuItems;
            AsduMenuItems.Add(CopyMenuItem);
            //Todo
        }

        private void SelectLinkedIusItem()
        {
            if (IsInLinkDisplayMode)
            {
                var iusItem = SelectedAsduItem == null
                    ? null
                    : IusTreeRootItems.FindItem(i => i.Id == _selectedAsduItem.LinkedId);
                SelectedIusItem = iusItem;
                OnPropertyChanged(() => SelectedIusItem);
                iusItem?.ExpandToRoot();
                OnPropertyChanged(() => SelectedIusItem);
            }
        }

        private IEnumerable<DataTreeItem> _iusTreeRootItems;

        public IEnumerable<DataTreeItem> IusTreeRootItems
        {
            get { return _iusTreeRootItems; }
            set
            {
                if (SetProperty(ref _iusTreeRootItems, value))
                {
                    _iusTreeRootItemsSource.Source = _iusTreeRootItems;
                    OnPropertyChanged(() => IusTreeRootItemsView);
                }
            }
        }

        private readonly CollectionViewSource _iusTreeRootItemsSource = new CollectionViewSource();
        public ICollectionView IusTreeRootItemsView => _iusTreeRootItemsSource.View;

        private IEnumerable<DataTreeItem> _asduTreeRootItems;

        public IEnumerable<DataTreeItem> AsduTreeRootItems
        {
            get { return _asduTreeRootItems; }
            set
            {
                if (SetProperty(ref _asduTreeRootItems, value))
                {
                    _asduTreeRootItemsSource.Source = _asduTreeRootItems;
                    OnPropertyChanged(() => AsduTreeRootItemsView);
                }
            }
        }

        private readonly CollectionViewSource _asduTreeRootItemsSource = new CollectionViewSource();
        public ICollectionView AsduTreeRootItemsView => _asduTreeRootItemsSource.View;


        private bool _isInLinkDisplayMode;

        public bool IsInLinkDisplayMode
        {
            get { return _isInLinkDisplayMode; }
            set
            {
                if (SetProperty(ref _isInLinkDisplayMode, value))
                {
                    SelectLinkedAsduItem();
                }
            }
        }

        public IList<LoadedFileWrapper> AsduRequests { get; protected set; }

        private LoadedFileWrapper _selectedRequest;

        public LoadedFileWrapper SelectedRequest
        {
            get { return _selectedRequest; }
            set { SetProperty(ref _selectedRequest, value); }
        }

        public List<DictionaryEntry> LinkStates { get; protected set; }

        private void ApplyTreeFilter(bool isIus)
        {
            var rootItems = isIus ? IusTreeRootItemsView : AsduTreeRootItemsView;
            if (rootItems != null)
            {
                rootItems.Refresh();
                foreach (DataTreeItem item in rootItems)
                {
                    item.RefreshFilter();
                }
            }
        }

        internal void TreeFilter(object sender, FilterEventArgs e)
        {
            var item = e.Item as DataTreeItem;
            if (item != null)
            {
                var linkState = item.IsIus ? IusLinkState : AsduLinkState;
                e.Accepted = (linkState == LinkStatesAll ||
                                (linkState == LinkStatesLinked && item.IsLinked) ||
                                    (linkState == LinkStatesNotlinked &&
                                        (!item.IsLinked ||
                                            (item.AllChildren.Any(i => (!i.IsLinked && 
                                                (i.NodeType == DataTreeNodeType.IusObj || 
                                                i.NodeType == DataTreeNodeType.AsduObj))
                                        )))));
            }
        }

        private string _iusLinkState;

        public string IusLinkState
        {
            get { return _iusLinkState; }
            set
            {
                if (SetProperty(ref _iusLinkState, value))
                {
                    ApplyTreeFilter(true);
                }
            }
        }

        private string _asduLinkState;

        public string AsduLinkState
        {
            get { return _asduLinkState; }
            set
            {
                if (SetProperty(ref _asduLinkState, value))
                {
                    ApplyTreeFilter(false);
                }
            }
        }
    }


    public static class DataTreeItemExtensions
    {
        public static IEnumerable<DataTreeItem> ForAllItems(this IEnumerable<DataTreeItem> nodes)
        {
            foreach (var item in nodes)
            {
                yield return item;
                foreach (var child in item.AllChildren)
                {
                    yield return child;
                }
            }
        }

        public static DataTreeItem FindItem(this IEnumerable<DataTreeItem> nodes,
            Func<DataTreeItem, bool> predicate)
        {
            var metadataTreeItems = nodes.ToList();
            var res = metadataTreeItems.FirstOrDefault(predicate);
            if (res != null)
            {
                return res;
            }

            foreach (var n in metadataTreeItems)
            {
                res = n.Children.FindItem(predicate);
                if (res != null)
                {
                    return res;
                }
            }

            return null;
        }

        public static ObservableCollection<DataTreeItem> BuildTreeFromList(this IList<DataTreeNode> nodes,
            AsduDataViewModel parentViewModel,
            bool isIus, DataTreeItem parent = null)
        {
            var res = new ObservableCollection<DataTreeItem>(nodes.Where(n => string.Equals(n.ParentId, parent?.Id))
                .Select(n =>
                {
                    var i = new DataTreeItem(parentViewModel, parent, n, isIus);
                    i.Children = nodes.BuildTreeFromList(parentViewModel, isIus, i);
                    return i;
                }));
            return res;
        }
    }

    public class DataTreeItem : PropertyChangedBase
    {
        private readonly AsduDataViewModel _parentViewModel;

        public DataTreeItem(AsduDataViewModel parentViewModel, DataTreeItem parent, DataTreeNode node,
            bool isIus)
        {
            _parentViewModel = parentViewModel;
            Parent = parent;
            Node = node;
            IsIus = isIus;
            _childrenSource.Filter += _parentViewModel.TreeFilter;

            IsExpanded = false;
        }

        public DataTreeNodeType NodeType => Node.NodeType;

        public string NodeTypeHumanReadable
        {
            get
            {
                var res = "неизвестно";
                switch (NodeType)
                {
                    case DataTreeNodeType.IusObj:
                        return "Сущность";
                    case DataTreeNodeType.IusAttr:
                        return "Атрибут";
                    case DataTreeNodeType.IusAttrLink:
                        return "Ссылка";
                    case DataTreeNodeType.IusParam:
                        return "Параметр";
                    case DataTreeNodeType.AsduObj:
                        return "Объект";
                    case DataTreeNodeType.AsduAttr:
                        return "Атрибут";
                    case DataTreeNodeType.AsduAttrLink:
                        return "Ссылка";
                    case DataTreeNodeType.AsduParam:
                        return "Параметр";
                }

                return res;
            }
        }

        public string Id => Node.Id;

        public string ParentId => Node.ParentId;

        public string Name => Node.Name;

        public int ErrorCount => Node.ErrorCount;

        public string MiscInfo => Node.MiscInfo;

        public string RawId => Node.RawId;

        public string RawType => Node.RawType;

        public string LinkedId => Node.LinkedId;

        public string Value => Node.Value ?? " ";

        public DataValueEquality? Equality => Node.Equality;

        public bool IsLinked => !string.IsNullOrEmpty(LinkedId);

        private IEnumerable<DataTreeItem> _children;

        public IEnumerable<DataTreeItem> Children
        {
            get { return _children; }
            set
            {
                if (SetProperty(ref _children, value))
                {
                    _childrenSource.Source = _children;
                    OnPropertyChanged(() => ChildrenView);
                }
            }
        }

        private readonly CollectionViewSource _childrenSource = new CollectionViewSource();
        public ICollectionView ChildrenView => _childrenSource.View;

        public bool IsIus { get; }

        public DataTreeItem Parent { get; }
        public DataTreeNode Node { get; private set; }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetProperty(ref _isExpanded, value); }
        }

        public void ExpandToRoot()
        {
            var n = this.Parent;
            while (n != null)
            {
                n.IsExpanded = true;
                n = n.Parent;
            }
        }


        public void RefreshFilter()
        {
            if (ChildrenView != null)
            {
                ChildrenView.Refresh();
                foreach (DataTreeItem item in ChildrenView)
                {
                    item.RefreshFilter();
                }
            }
        }

        public IEnumerable<DataTreeItem> AllChildren
        {
            get
            {
                foreach (var item in _children)
                {
                    yield return item;
                    foreach (var child in item.AllChildren)
                    {
                        yield return child;
                    }
                }
            }
        }

        public void Update(DataTreeNode node)
        {
            this.Node = node;
            foreach (var propertyInfo in GetType().GetProperties())
            {
                OnPropertyChanged(propertyInfo.Name);
            }
        }

        public void UpdateSelfAndChildren(IList<DataTreeNode> updatedNodes)
        {
            foreach (var node in updatedNodes)
            {
                if (this.Id == node.Id)
                {
                    Update(node);
                }

                var childToUpdate = this.Children.FindItem(i => i.Id == node.Id);
                childToUpdate?.Update(node);
            }
        }
    }
}