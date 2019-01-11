using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
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
using GazRouter.Common.GoodStyles;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ASDU;
using GazRouter.DTO.ASDU;
using GazRouter.Modes.DispatcherTasks.PDS;
using Microsoft.Practices.Prism.Commands;

namespace DataExchange.ASDU
{
    public class LinkMenuItem : PropertyChangedBase
    {
        private string _text;

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        public ICommand Command { get; set; }
    }

    public class MatchingTreeItem : PropertyChangedBase
    {
        public const string LinkedStateName = "linked";
        public const string UnLinkedStateName = "unlinked";
        public const string NotEqualStateName = "ne";

        private MatchingTreeNode _node;
        private readonly AsduMatchingViewModel _parentViewModel;

        public MatchingTreeItem(MatchingTreeNode node, AsduMatchingViewModel parentViewModel, MatchingTreeItem parentItem, bool isIus)
        {
            _node = node;
            _parentViewModel = parentViewModel;
            _childrenSource.Filter += _parentViewModel.TreeFilter;
            ParentItem = parentItem;
            IsIus = isIus;
        }

        public void Rebind(MatchingTreeNode newNode)
        {
            _node = newNode;
            OnPropertyChanged(() => NodeType);
            OnPropertyChanged(() => NodeTypeHumanReadable);
            OnPropertyChanged(() => Type);
            OnPropertyChanged(() => Id);
            OnPropertyChanged(() => Name);
            OnPropertyChanged(() => IsEndNode);
            OnPropertyChanged(() => CanLink);
            OnPropertyChanged(() => Value);
            OnPropertyChanged(() => LinkedObjectId);
            OnPropertyChanged(() => LinkedRole);
            OnPropertyChanged(() => ChangeState);
            OnPropertyChanged(() => SomeFile);
            OnPropertyChanged(() => LinkedState);
            OnPropertyChanged(() => ChangeStateColor);
            OnPropertyChanged(() => IsExpandable);
            OnPropertyChanged(() => SomeFile);
            OnPropertyChanged(() => SomeFile);
            OnPropertyChanged(() => SomeFile);
            ClearChildren();
            IsExpanded = false;
        }

        public string NodeType => Node.Nodetype;

        public string NodeTypeHumanReadable
        {
            get
            {
                var res = "неизвестно";
                switch (NodeType)
                {
                    case "root_asdu":
                        res = "Корневой";
                        break;
                    case "type_asdu":
                        res = "Ссылка";
                        break;
                    case "asdu_obj":
                        res = "Объект";
                        break;
                    case "atr_asdu":
                        res = "Атрибут";
                        break;
                    case "param_asdu":
                        res = "Параметр";
                        break;
                }
                return res;
            }
        }

        public string Type => Node.Type;

        public string Id => Node.Id;

        public string Name => Node.Name;

        public bool IsEndNode => Node.IsEndNode;
        public bool CanLink => Node.CanLink;

        public string Value => Node.Value ?? " ";

        public string LinkedObjectId
        {
            get { return Node.Linkedobjid; }
            set
            {
                if (Node.Linkedobjid != value)
                {
                    Node.Linkedobjid = value;
                    OnPropertyChanged(() => LinkedObjectId);
                }
            }
        }

        public string LinkedRole
        {
            get { return Node.Linkedrole; }
            set
            {
                if (Node.Linkedrole != value)
                {
                    Node.Linkedrole = value;
                    OnPropertyChanged(() => LinkedRole);
                }
            }
        }

        public string ChangeState => Node.Changestate;

        public string SomeFile
        {
            get { return Node.Somefile; }
            set
            {
                if (Node.Somefile != value)
                {
                    Node.Somefile = value;
                    OnPropertyChanged(() => SomeFile);
                }
            }
        }

        public string LinkedState
        {
            get { return Node.Linkstate; }
            set
            {
                if (Node.Linkstate != value)
                {
                    Node.Linkstate = value;
                    OnPropertyChanged(() => LinkedState);
                    OnPropertyChanged(() => IsLinked);
                    OnPropertyChanged(() => LinkedStateImagePath);
                }
            }
        }

        public bool IsLinked => LinkedState == LinkedStateName;

        /*public string ChangeStateImagePath => ChangeState == NotEqualStateName
            ? @"/Common;component/Images/16x16/flag_orange.png"
            : @"/Common;component/Images/16x16/flag_green.png";*/
        

        public string LinkedStateImagePath
        {
            get
            {
                if (!IsLinked)
                {
                   return @"/DataExchange;component/ASDU/img/broken_link_16px.png";
                }
                else
                {
                    return @"/DataExchange;component/ASDU/img/link_16px.png";
                }
            }
        }

        public Brush ChangeStateColor
        {
            get
            {
                if (!IsLinked)
                {
                    return Brushes.Transparent;
                }
                if (ChangeState == NotEqualStateName)
                {
                    return Brushes.Orange;
                }
                else
                {
                    return Brushes.NiceGreen;
                }
            }
        }
            


        private bool _isExpandable = true;

        private bool _isExpanded;

        private CollectionViewSource _childrenSource { get; set; } = new CollectionViewSource();
        public ICollectionView Children => _childrenSource.View;

        public void ClearChildren()
        {
            _childrenSource.Source = null;
            OnPropertyChanged(() => Children);
        }

        public bool IsExpandable
        {
            get { return _isExpandable && !IsEndNode; }
            set { SetProperty(ref _isExpandable, value); }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    if (_isExpanded && Children == null)
                    {
                        var t = _parentViewModel.LoadChildItems(this.Node, this, IsIus);
                        Action loadChildren = async () =>
                        {
                            var src = new ObservableCollection<MatchingTreeItem>(await t);
                            _childrenSource.Source = src;
                            OnPropertyChanged(() => Children);
                            IsExpandable = src.Count > 0;
                            _isExpanded = src.Count > 0;
                            OnPropertyChanged(() => IsExpanded);
                        };
                        loadChildren();
                    }
                    else
                    {
                        OnPropertyChanged(() => IsExpanded);
                    }
                }
            }
        }

        public MatchingTreeItem ParentItem { get; }
        public bool IsIus { get; }

        public MatchingTreeNode Node
        {
            get { return _node; }
        }

        public void RefreshFilter()
        {
            if (Children != null)
            {
                Children.Refresh();
                foreach (MatchingTreeItem item in Children)
                {
                    item.RefreshFilter();
                }
            }
        }
    }


    public class AsduMatchingViewModel : LockableViewModel
    {

        private readonly ASDUServiceProxy _serviceProxy;

        public DelegateCommand LoadedCommand { get; }
        public DelegateCommand MatchingMenuOpeningCommand { get; }
        public DelegateCommand UnLinkCommand { get; }
        public DelegateCommand<string> AddToOutboundCommand { get; }
        public DelegateCommand ManageRequestsCommand { get; }
        public DelegateCommand ApplyChangeToIusCommand { get; }
        

        public ObservableCollection<LinkMenuItem> LinkMenuItems { get; } = new ObservableCollection<LinkMenuItem>();

        public AsduMatchingViewModel()
        {
            LoadedCommand = new DelegateCommand(InitData);
            MatchingMenuOpeningCommand = new DelegateCommand(MatchingMenuOpening);
            UnLinkCommand = new DelegateCommand(UnLink);
            AddToOutboundCommand = new DelegateCommand<string>(AddToOutbound);
            ManageRequestsCommand = new DelegateCommand(ManageRequests);
            ApplyChangeToIusCommand = new DelegateCommand(ApplyChangeToIus);

            _serviceProxy = new ASDUServiceProxy();
            _iusTreeRootItemSource.Filter += TreeFilter;
            _asduTreeRootItemsSource.Filter += TreeFilter;
        }

        private async void ApplyChangeToIus()
        {
            if (SelectedAsduItem != null)
            {
                Behavior.TryLock();
                try
                {
                    await _serviceProxy.ApplyChangeToIusAsync(SelectedAsduItem.Node);
                    LoadRootTree(true);
                    RefreshTreeItem(SelectedAsduItem);
                }
                finally
                {
                    Behavior.TryUnlock();
                }
            }
        }

        private void ManageRequests()
        {
//            var acrv = new AsduChangeRequestView {DataContext = new AsduChangeRequestViewModel(AsduOutbound)};
//            acrv.Closed += async (a, b) =>
//            {
//                LoadRootTree(true);
//                LoadRootTree(false);
//                var toSelect = AsduOutbound;
//                var tAsduOutbounds = _serviceProxy.GetFilterParamsAsync(FilterType.AsduOutbounds);
//                await tAsduOutbounds;
//                AsduOutbounds = tAsduOutbounds.Result;
//                OnPropertyChanged(() => AsduOutbounds);
//                AsduOutbound = toSelect;
//            };
//            acrv.ShowDialog();
        }

        private async void AddToOutbound(string s)
        {
            var isAsdu = s == "Asdu";
            var treeItem = isAsdu ? SelectedAsduItem : SelectedIusItem;
            if (treeItem != null &&
                AsduOutbound != null)
            {
                Behavior.TryLock();
                try
                {
                    var iusItem = treeItem;
                    var outbound = AsduOutbound;
                    await _serviceProxy.AddAsduRequestAsync(
                        new AsduRequestParams
                        {
//                            RequestKey = outbound,
//                            NodeType = iusItem.NodeType,
//                            Type = iusItem.Type,
//                            Id = iusItem.Id
                        });
                    if (!isAsdu)
                    {
                        iusItem.SomeFile = AsduOutbounds.FirstOrDefault(a => a.Key == outbound)?.Value;
                    }
                }
                finally
                {
                    Behavior.TryUnlock();
                }
            }
        }


        //private long _dummy;
        public bool CanLink => SelectedIusItem != null && SelectedAsduItem != null && !SelectedIusItem.IsLinked &&
                               !SelectedAsduItem.IsLinked && SelectedIusItem.CanLink && SelectedAsduItem.CanLink;
                               /*!(SelectedIusItem.Id?.Contains(":")).GetValueOrDefault() && long.TryParse(SelectedIusItem.Type, out _dummy) &&
                               !(SelectedAsduItem.Id?.Contains(":")).GetValueOrDefault();*/

        public bool CanUnLink => SelectedIusItem != null && SelectedAsduItem != null && SelectedIusItem.IsLinked &&
                                 SelectedAsduItem.IsLinked && SelectedIusItem.LinkedObjectId == SelectedAsduItem.Id &&
                                 SelectedAsduItem.LinkedObjectId == SelectedIusItem.Id &&
                                 SelectedIusItem.LinkedRole == SelectedAsduItem.LinkedRole;

        public bool CanAddToOutbound => SelectedIusItem != null && AsduOutbound != null;

        public bool CanAddAsduToOutbound => SelectedAsduItem?.IsLinked != true && SelectedAsduItem?.NodeType == "asdu_obj";

        private async void RefreshTreeItem(MatchingTreeItem item)
        {
            var parentNode = item.ParentItem?.Node ?? GetDummyRootNode(item.IsIus);
            var res = await _serviceProxy.GetTreeNodesAsync(new MatchingTreeNodeParams(parentNode, item.Node));
            if (res.Any())
            {
                item.Rebind(res.First());
            }
            LinkStateChanged();
        }

        private async void MatchingMenuOpening()
        {
            LinkMenuItems.Clear();
            var possibleLinkRoles = await _serviceProxy.GetPossibleLinkRolesAsync(new LinkParams
            {
                IusType = SelectedIusItem?.Type,
                IusId = SelectedIusItem?.Id,
                AsduId = SelectedAsduItem?.Id
            });
            if (possibleLinkRoles.Any())
            {
                foreach (var linkRole in possibleLinkRoles)
                {
                    LinkMenuItems.Add(new LinkMenuItem
                    {
                        Text = $"С ролью \"{linkRole.Value}\"",
                        Command = new DelegateCommand<LinkCommandParams>(a => MakeLink(new LinkCommandParams
                        {
                            AsduItem = SelectedAsduItem,
                            IusItem = SelectedIusItem,
                            LinkRole = linkRole
                        }))
                    });
                }
            }
        }

        private async void UnLink()
        {
           /* if (!CanUnLink)
            {
                return;
            }
            var linkParams = new LinkCommandParams
            {
                AsduItem = SelectedAsduItem,
                IusItem = SelectedIusItem,
                LinkRole = new DictionaryEntry
                {
                    Key = SelectedIusItem.LinkedRole,
                    Value = SelectedIusItem.LinkedRole
                }
            };
            Behavior.TryLock();
            try
            {
                await _serviceProxy.ManageLinkAsync(
                    new LinkParams
                    {
                        IusId = linkParams.IusItem.Id,
                        AsduId = linkParams.AsduItem.Id,
                        LinkRole = linkParams.LinkRole.Key,
                        IsUnlink = true
                    });
                RefreshTreeItem(linkParams.IusItem);
                RefreshTreeItem(linkParams.AsduItem);
            }
            finally
            {
                Behavior.TryUnlock();
            }*/
        }

        private async void MakeLink(LinkCommandParams linkParams)
        {
           /* if (linkParams.IusItem != null &&
                linkParams.AsduItem != null &&
                linkParams.LinkRole != null)
            {
                LinkMenuItems.Clear();
                Behavior.TryLock();
                try
                {
                    await _serviceProxy.ManageLinkAsync(
                        new LinkParams
                        {
                            IusType = linkParams.IusItem.Type,
                            IusId = linkParams.IusItem.Id,
                            AsduId = linkParams.AsduItem.Id,
                            LinkRole = linkParams.LinkRole.Key
                        });
                    RefreshTreeItem(linkParams.IusItem);
                    RefreshTreeItem(linkParams.AsduItem);
                }
                finally
                {
                    Behavior.TryUnlock();
                }
            }*/
        }


        internal void TreeFilter(object sender, FilterEventArgs e)
        {
            var item = e.Item as MatchingTreeItem;
            if (item != null)
            {
                var linkState = item.IsIus ? IusLinkState : AsduLinkState;
                var changeState = item.IsIus ? IusChangeState : AsduChangeState;
                e.Accepted = (linkState == AllTypes || item.LinkedState == linkState) &&
                             (changeState == AllTypes || item.ChangeState == changeState);
            }
        }

        private const string RootIusNodeType = "root_ius";
        private const string RootAsduNodeType = "root_asdu";
        private const string AllTypes = "_all_";
        public List<DictionaryEntry> IusObjectTypes { get; protected set; }
        public List<DictionaryEntry> AsduObjectTypes { get; protected set; }
        public List<DictionaryEntry> LinkStates { get; protected set; }
        public List<DictionaryEntry> ChangeStates { get; protected set; }

        public List<DictionaryEntry> AsduOutbounds { get; protected set; }

        private void ApplyTreeFilter(bool isIus)
        {
            var rootItems = isIus ? IusTreeRootItems : AsduTreeRootItems;
            if (rootItems != null)
            {
                rootItems.Refresh();
                foreach (MatchingTreeItem item in rootItems)
                {
                    item.RefreshFilter();
                }
            }
        }


        private MatchingTreeItem _selectedIusItem;

        public MatchingTreeItem SelectedIusItem
        {
            get { return _selectedIusItem; }
            set
            {
                if (SetProperty(ref _selectedIusItem, value))
                {
                    LinkStateChanged();
                    OnPropertyChanged(() => CanAddToOutbound);
                }
            }
        }

        private MatchingTreeItem _selectedAsduItem;

        public MatchingTreeItem SelectedAsduItem
        {
            get { return _selectedAsduItem; }
            set
            {
                if (SetProperty(ref _selectedAsduItem, value))
                {
                    LinkStateChanged();
                    OnPropertyChanged(() => CanAddAsduToOutbound);
                }
            }
        }

        private void LinkStateChanged()
        {
            OnPropertyChanged(() => CanLink);
            OnPropertyChanged(() => CanUnLink);
        }

        private string _iusObjectType;
        public string IusObjectType
        {
            get { return _iusObjectType; }
            set
            {
                if (SetProperty(ref _iusObjectType, value))
                {
                    LoadRootTree(true);
                }

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

        private string _iusChangeState;
        public string IusChangeState
        {
            get { return _iusChangeState; }
            set
            {
                if (SetProperty(ref _iusChangeState, value))
                {
                    ApplyTreeFilter(true);
                }

            }
        }

        private string _asduObjectType;
        public string AsduObjectType
        {
            get { return _asduObjectType; }
            set
            {
                if (SetProperty(ref _asduObjectType, value))
                {
                    LoadRootTree(false);
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

        private string _asduChangeState;
        public string AsduChangeState
        {
            get { return _asduChangeState; }
            set
            {
                if (SetProperty(ref _asduChangeState, value))
                {
                    ApplyTreeFilter(false);
                }

            }
        }

        private string _asduOutbound;
        public string AsduOutbound
        {
            get { return _asduOutbound; }
            set { SetProperty(ref _asduOutbound, value); }
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

        private CollectionViewSource _iusTreeRootItemSource { get; set; } = new CollectionViewSource();
        public ICollectionView IusTreeRootItems => _iusTreeRootItemSource.View;

        private CollectionViewSource _asduTreeRootItemsSource { get; set; } = new CollectionViewSource();
        public ICollectionView AsduTreeRootItems => _asduTreeRootItemsSource.View;


        internal async Task<List<MatchingTreeItem>> LoadChildItems(MatchingTreeNode parentNode, MatchingTreeItem parentItem, bool iusTree)
        {
            try
            {
                if (iusTree)
                {
                    IsIusTreeBusy = true;
                }
                else
                {
                    IsAsduTreeBusy = true;
                }
                var res = await _serviceProxy.GetTreeNodesAsync(new MatchingTreeNodeParams(parentNode));
                return res.Select(n => new MatchingTreeItem(n, this, parentItem, iusTree)).ToList();
            }
            finally
            {
                if (iusTree)
                {
                    IsIusTreeBusy = false;
                }
                else
                {
                    IsAsduTreeBusy = false;
                }
            }
        }

        private async void LoadRootTree(bool isIus)
        {
            var node = GetDummyRootNode(isIus);
            var res = new ObservableCollection<MatchingTreeItem>(await LoadChildItems(node, null, isIus));
            if (isIus)
            {
                _iusTreeRootItemSource.Source = res;
                OnPropertyChanged(() => IusTreeRootItems);
                SelectedIusItem = null;
            }
            else
            {
                _asduTreeRootItemsSource.Source = res;
                OnPropertyChanged(() => AsduTreeRootItems);
                SelectedAsduItem = null;
            }

        }

        private MatchingTreeNode GetDummyRootNode(bool isIus)
        {
            return MatchingTreeNode.AsParam(isIus ? RootIusNodeType : RootAsduNodeType, isIus ? IusObjectType : AsduObjectType, "");
        }

        private async void InitData()
        {
            Behavior.TryLock();
            try
            {
                var tIusObjectTypes = _serviceProxy.GetFilterParamsAsync(FilterType.IusObjectTypes);
                var tAsduObjectTypes = _serviceProxy.GetFilterParamsAsync(FilterType.AsduObjectTypes);
                var tLinkStates = _serviceProxy.GetFilterParamsAsync(FilterType.LinkStates);
                var tChangeStates = _serviceProxy.GetFilterParamsAsync(FilterType.ChangeStates);
                var tAsduOutbounds = _serviceProxy.GetFilterParamsAsync(FilterType.AsduOutbounds);
                await TaskEx.WhenAll(tIusObjectTypes, tAsduObjectTypes, tLinkStates, tChangeStates, tAsduOutbounds);

                IusObjectTypes = tIusObjectTypes.Result;
                OnPropertyChanged(() => IusObjectTypes);

                AsduObjectTypes = tAsduObjectTypes.Result;
                OnPropertyChanged(() => AsduObjectTypes);

                LinkStates = tLinkStates.Result;
                LinkStates.Insert(0, new DictionaryEntry { Key = AllTypes, Value = "Все" });
                OnPropertyChanged(() => LinkStates);

                ChangeStates = tChangeStates.Result;
                ChangeStates.Insert(0, new DictionaryEntry { Key = AllTypes, Value = "Все" });
                OnPropertyChanged(() => ChangeStates);

                AsduOutbounds = tAsduOutbounds.Result;
                OnPropertyChanged(() => AsduOutbounds);
            }
            finally
            {
                Behavior.TryUnlock();
            }

            IusLinkState = LinkStates.First().Key;
            AsduLinkState = LinkStates.First().Key;
            IusChangeState = ChangeStates.First().Key;
            AsduChangeState = ChangeStates.First().Key;
            if (AsduOutbounds.Any())
            {
                AsduOutbound = AsduOutbounds.First().Key;
            }


            if (IusObjectTypes.Any())
            {
                IusObjectType = IsDebug ? IusObjectTypes.FirstOrDefault(a => a.Value == "Компрессорная станция")?.Key : IusObjectTypes.First().Key;
            }
            if (IusObjectTypes.Any())
            {
                AsduObjectType = IsDebug ? AsduObjectTypes.FirstOrDefault(a => a.Value == "Компрессорная станция")?.Key : AsduObjectTypes.First().Key;
            }
        }
    }

    internal class LinkCommandParams
    {
        public DictionaryEntry LinkRole { get; set; }
        public MatchingTreeItem IusItem { get; set; }
        public MatchingTreeItem AsduItem { get; set; }
    }
}
