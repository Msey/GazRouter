using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace DataExchange.ASDU
{
    public class AsduMetadataViewModel : LockableViewModel
    {
        private const string LinkStatesAll = "all";
        private const string LinkStatesLinked = "linked";
        private const string LinkStatesNotlinked = "notlinked";

        private string _fileContent = null;
        private string _fileName = null;

        public DelegateCommand LoadedCommand { get; }
        public DelegateCommand AsduTreeExpandedCommand { get; }
        public DelegateCommand IusTreeExpandedCommand { get; }
        public DelegateCommand ExportToExcelCommand { get; }
        public DelegateCommand LoadMetaXmlCommand { get; }
        public DelegateCommand LinkCommand { get; }
        public DelegateCommand UnLinkCommand { get; }

        public AsduMetadataViewModel()
        {
            LoadedCommand = new DelegateCommand(LoadTrees);
            AsduTreeExpandedCommand = new DelegateCommand(AsduTreeExpanded);
            IusTreeExpandedCommand = new DelegateCommand(IusTreeExpanded);
            ExportToExcelCommand = new DelegateCommand(ExportToExcel);
            LoadMetaXmlCommand = new DelegateCommand(LoadMetaXml);
            LinkCommand = new DelegateCommand(() => ManageLink(true));
            UnLinkCommand = new DelegateCommand(() => ManageLink(false));

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

        private async void ManageLink(bool link)
        {
            Behavior.TryLock();
            try
            {
                LinkAction la;
                if (SelectedIusItem == null || SelectedAsduItem == null)
                {
                    return;
                }
                if (link && SelectedIusItem.IsLinked && SelectedAsduItem.IsLinked)
                {
                    return;
                }
                if (!link && (SelectedIusItem.LinkedId != SelectedAsduItem.Id || SelectedAsduItem.LinkedId != SelectedAsduItem.Id))
                {
                    return;
                }
                if (SelectedIusItem.NodeType == MetadataTreeNodeType.IusObj &&
                    SelectedAsduItem.NodeType == MetadataTreeNodeType.AsduObj)
                {
                    la = link ? LinkAction.LinkMetaEntity : LinkAction.UnLinkMetaEntity;
                }
                else if (SelectedIusItem.NodeType == MetadataTreeNodeType.IusAttr &&
                         SelectedAsduItem.NodeType == MetadataTreeNodeType.AsduAttr ||
                         SelectedIusItem.NodeType == MetadataTreeNodeType.IusAttrLink &&
                         SelectedAsduItem.NodeType == MetadataTreeNodeType.AsduAttrLink)
                {
                    la = link ? LinkAction.LinkMetaAttr : LinkAction.UnLinkMetaAttr;
                }
                else if (SelectedIusItem.NodeType == MetadataTreeNodeType.IusParam &&
                         SelectedAsduItem.NodeType == MetadataTreeNodeType.AsduParam)
                {
                    la = link ? LinkAction.LinkMetaParam : LinkAction.UnLinkMetaParam;
                }
                else
                {
                    return;
                }
                var p = new ASDUServiceProxy();
                
                await p.ManageLinkAsync(new LinkParams
                {
                    IusType = SelectedIusItem.RawType,
                    IusId = SelectedIusItem.RawId,
                    AsduType = SelectedAsduItem.RawType,
                    AsduId = SelectedAsduItem.RawId,
                    LinkAction = la
                });

            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private async void LoadMetaXml()
        {
            _fileContent = null;
            _fileName = null;
            if (!SelectFile())
            {
                return;
            }
            Behavior.TryLock();
            try
            {
                var sp = new ASDUServiceProxy();
                BusyMessage = string.Format("Выполняется загрузка файла");
                var result = await sp.ImportXmlFromMASDUAsync(new XmlFileForImport
                {
                    IsMetadataFile = true,
                    Filename = _fileName,
                    Xml = _fileContent
                });
            }
            finally
            {
                Behavior.TryUnlock();
            }
            LoadTrees();
        }

        private bool SelectFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Filter = "Файлы XML (.xml)|*.xml|All Files (*.*)|*.*",
                FilterIndex = 1,
                Multiselect = false
            };

            bool? userClickedOK = openFileDialog1.ShowDialog();

            if (userClickedOK == true)
            {
                System.IO.Stream fileStream = openFileDialog1.File.OpenRead();

                using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                {
                    _fileContent = reader.ReadToEnd();
                }

                fileStream.Close();
                _fileName = openFileDialog1.File.Name;
            }

            return userClickedOK.GetValueOrDefault();
        }

        private void ExportToExcel()
        {
            var textDialog = new SaveFileDialog
            {
                Filter = "Файлы Excel | *.xlsx",
                DefaultExt = "xlsx",
                DefaultFileName = "MetadataReport"
            };
            var result = textDialog.ShowDialog();
            if (result == true)
            {
                using (var fileStream = textDialog.OpenFile())
                {
                    var workbook = PrepareReport();
                    IWorkbookFormatProvider formatProvider = new XlsxFormatProvider();
                    formatProvider.Export(workbook, fileStream);
                }
            }
        }

        private Workbook PrepareReport()
        {
            Workbook workbook = new Workbook();
            var ws = workbook.Worksheets.Add();
            ws.DefaultColumnWidth = new ColumnWidth(200, false);
            ws.Name = "Объекты ИУС ПТП";
            var iusNodes = _iusTreeRootItemsSource.Source as IEnumerable<MetadataTreeItem>;
            if (iusNodes != null)
            {
                var row = 0;
                var c = ws.Cells[row, 0, row, 3];
                c.Merge();
                c.SetValue("Связанные объекты");
                c.SetBorders(CellBorders.CreateOutline(CellBorder.Default));
                row++;
                foreach (var item in iusNodes.ForAllItems()
                    .Where(i => i.NodeType == MetadataTreeNodeType.IusObj && i.IsLinked))
                {
                    row++;
                    c = ws.Cells[row, 0];
                    c.SetValue("Имя:");
                    c = ws.Cells[row, 1];
                    c.SetValue(item.Name);
                    row++;
                    c = ws.Cells[row, 0];
                    c.SetValue("Родительский объект:");
                    c = ws.Cells[row, 1];
                    c.SetValue(item.Parent == null ? "<нет>" : item.Parent.Name);
                    row++;
                    c = ws.Cells[row, 0];
                    c.SetValue("Связанный класс М АСДУ:");
                    var itemAsdu = AsduTreeRootItems.FindItem(i => i.Id == item.LinkedId);
                    c = ws.Cells[row, 1];
                    c.SetValue(itemAsdu?.Name);
                    row++;

                    c = ws.Cells[row, 0, row, 3];
                    c.Merge();
                    c.SetValue("Атрибуты");
                    row++;
                    c = ws.Cells[row, 0];
                    c.SetValue("Имя");
                    c = ws.Cells[row, 1];
                    c.SetValue("Тип");
                    c = ws.Cells[row, 2];
                    c.SetValue("Связанный атрибут В М АСДУ");
                    c = ws.Cells[row, 3];
                    c.SetValue("GUID в М АСДУ");
                    row++;
                    foreach (var attr in item.Children.Where(child => child.NodeType == MetadataTreeNodeType.IusAttr || child.NodeType == MetadataTreeNodeType.IusAttrLink))
                    {
                        c = ws.Cells[row, 0];
                        c.SetValue(attr.Name);
                        c = ws.Cells[row, 1];
                        c.SetValue(attr.NodeTypeHumanReadable);
                        c = ws.Cells[row, 2];
                        var attrAsdu = AsduTreeRootItems.FindItem(i => i.Id == attr.LinkedId);
                        c.SetValue(attrAsdu?.Name);
                        c = ws.Cells[row, 3];
                        c.SetValue(attrAsdu?.RawId);
                        row++;
                    }

                    c = ws.Cells[row, 0, row, 2];
                    c.Merge();
                    c.SetValue("Параметры");
                    row++;
                    c = ws.Cells[row, 0];
                    c.SetValue("Имя");
                    c = ws.Cells[row, 1];
                    c.SetValue("Связанный параметр В М АСДУ");
                    c = ws.Cells[row, 2];
                    c.SetValue("GUID в М АСДУ");
                    row++;
                    foreach (var param in item.Children.Where(child => child.NodeType == MetadataTreeNodeType.IusParam))
                    {
                        c = ws.Cells[row, 0];
                        c.SetValue(param.Name);
                        c = ws.Cells[row, 1];
                        var paramAsdu = AsduTreeRootItems.FindItem(i => i.Id == param.LinkedId);
                        c.SetValue(paramAsdu?.Name);
                        c = ws.Cells[row, 2];
                        c.SetValue(paramAsdu?.RawId);
                        row++;
                    }
                }
            }

            return workbook;
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
                var res = await p.GetMetadataTreeNodesAsync(new MetadataTreeParams {IsIus = true});
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
                var res = await p.GetMetadataTreeNodesAsync(new MetadataTreeParams {IsIus = false});
                AsduTreeRootItems = res.BuildTreeFromList(this, false);
            }
            finally
            {
                IsAsduTreeBusy = false;
            }
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

        private MetadataTreeItem _selectedIusItem;

        public MetadataTreeItem SelectedIusItem
        {
            get { return _selectedIusItem; }
            set
            {
                if (SetProperty(ref _selectedIusItem, value))
                {
                    SelectLinkedAsduItem();
                }
            }
        }

        private void SelectLinkedAsduItem()
        {
            if (IsInLinkDisplayMode)
            {
                var asduItem = SelectedIusItem == null
                    ? null
                    : AsduTreeRootItems.FindItem(i => i.Id == _selectedIusItem.LinkedId);
                asduItem?.ExpandToRoot();
                SelectedAsduItem = asduItem;
                OnPropertyChanged(() => SelectedAsduItem);
            }
        }


        private MetadataTreeItem _selectedAsduItem;

        public MetadataTreeItem SelectedAsduItem
        {
            get { return _selectedAsduItem; }
            set
            {
                if (SetProperty(ref _selectedAsduItem, value))
                {
                    SelectLinkedIusItem();
                }
            }
        }

        private void SelectLinkedIusItem()
        {
            if (IsInLinkDisplayMode)
            {
                var iusItem = SelectedAsduItem == null
                    ? null
                    : IusTreeRootItems.FindItem(i => i.Id == _selectedAsduItem.LinkedId);
                iusItem?.ExpandToRoot();
                SelectedIusItem = iusItem;
                OnPropertyChanged(() => SelectedIusItem);
            }
        }

        private IEnumerable<MetadataTreeItem> _iusTreeRootItems;

        public IEnumerable<MetadataTreeItem> IusTreeRootItems
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

        private IEnumerable<MetadataTreeItem> _asduTreeRootItems;

        public IEnumerable<MetadataTreeItem> AsduTreeRootItems
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

        public List<DictionaryEntry> LinkStates { get; protected set; }

        private void ApplyTreeFilter(bool isIus)
        {
            var rootItems = isIus ? IusTreeRootItemsView : AsduTreeRootItemsView;
            if (rootItems != null)
            {
                rootItems.Refresh();
                foreach (MetadataTreeItem item in rootItems)
                {
                    item.RefreshFilter();
                }
            }
        }

        internal void TreeFilter(object sender, FilterEventArgs e)
        {
            var item = e.Item as MetadataTreeItem;
            if (item != null)
            {
                var linkState = item.IsIus ? IusLinkState : AsduLinkState;
                e.Accepted = (linkState == LinkStatesAll ||
                              (linkState == LinkStatesLinked && item.IsLinked) ||
                              (linkState == LinkStatesNotlinked &&
                               (!item.IsLinked || item.AllChildren.Any(i => !i.IsLinked))));
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


    public static class MetadataTreeItemExtensions
    {
        public static IEnumerable<MetadataTreeItem> ForAllItems(this IEnumerable<MetadataTreeItem> nodes)
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

        public static MetadataTreeItem FindItem(this IEnumerable<MetadataTreeItem> nodes,
            Func<MetadataTreeItem, bool> predicate)
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

        public static ObservableCollection<MetadataTreeItem> BuildTreeFromList(this IList<MetadataTreeNode> nodes,
            AsduMetadataViewModel parentViewModel,
            bool isIus, MetadataTreeItem parent = null)
        {
            var res = new ObservableCollection<MetadataTreeItem>(nodes.Where(n => string.Equals(n.ParentId, parent?.Id))
                .Select(n =>
                {
                    var i = new MetadataTreeItem(parentViewModel, parent, n, isIus);
                    i.Children = nodes.BuildTreeFromList(parentViewModel, isIus, i);
                    return i;
                }));
            return res;
        }
    }

    public class MetadataTreeItem : PropertyChangedBase
    {
        private readonly AsduMetadataViewModel _parentViewModel;

        public MetadataTreeItem(AsduMetadataViewModel parentViewModel, MetadataTreeItem parent, MetadataTreeNode node,
            bool isIus)
        {
            _parentViewModel = parentViewModel;
            Parent = parent;
            Node = node;
            IsIus = isIus;
            _childrenSource.Filter += _parentViewModel.TreeFilter;
        }

        public MetadataTreeNodeType NodeType => Node.Nodetype;

        public string NodeTypeHumanReadable
        {
            get
            {
                var res = "неизвестно";
                switch (NodeType)
                {
                    case MetadataTreeNodeType.IusObj:
                        return "Сущность";
                    case MetadataTreeNodeType.IusAttr:
                        return "Атрибут";
                    case MetadataTreeNodeType.IusAttrLink:
                        return "Ссылка";
                    case MetadataTreeNodeType.IusParam:
                        return "Параметр";
                    case MetadataTreeNodeType.AsduObj:
                        return "Класс";
                    case MetadataTreeNodeType.AsduAttr:
                        return "Атрибут";
                    case MetadataTreeNodeType.AsduAttrLink:
                        return "Ссылка";
                    case MetadataTreeNodeType.AsduParam:
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

        public bool IsLinked => !string.IsNullOrEmpty(LinkedId);

        private IEnumerable<MetadataTreeItem> _children;

        public IEnumerable<MetadataTreeItem> Children
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

        public MetadataTreeItem Parent { get; }
        public MetadataTreeNode Node { get; }

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
                foreach (MetadataTreeItem item in ChildrenView)
                {
                    item.RefreshFilter();
                }
            }
        }

        public IEnumerable<MetadataTreeItem> AllChildren
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
    }
}