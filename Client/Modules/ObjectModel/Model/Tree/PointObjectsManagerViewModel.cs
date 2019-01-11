using System.Collections.Generic;
using System.ComponentModel;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Tree;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.ObjectModel.Model.Tabs;
using GazRouter.ObjectModel.Model.Tabs.CoolingRecomended;
using GazRouter.ObjectModel.Model.Tabs.PropertyValues;
namespace GazRouter.ObjectModel.Model.Tree
{
    public class PointObjectsManagerViewModel : PropertyChangedBase
    {
        private NodeBase _prevSelectedNode;
        private ITabItem _selectedTabItem;
        private List<ITabItem> _tabItems = new List<ITabItem>();

        public PointObjectsManagerViewModel()
        {
            EditableFullTreeVM = new EditableFullTreeViewModel();
            EditableFullTreeVM.TreeModel.PropertyChanged += EditableFullTreeVmPropertyChanged;
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public EditableFullTreeViewModel EditableFullTreeVM { get; private set; }

        public ITabItem SelectedTabItem
        {
            get { return _selectedTabItem; }
            set { SetProperty(ref _selectedTabItem, value); }
        }

        public List<ITabItem> TabItems
        {
            get { return _tabItems; }
            set { SetProperty(ref _tabItems, value); }
        }


        private void EditableFullTreeVmPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedNode")
            {
                RefreshTabsList();
            }
        }


        private void RefreshTabsList()
        {
            // TabItems = new List<ITabItem>();

            NodeBase selectedNode = EditableFullTreeVM.TreeModel.SelectedNode;
            if (selectedNode == null || selectedNode is FolderNode)
            {
                TabItems = new List<ITabItem>();

                _prevSelectedNode = null;
                return;
            }
            var parentEntity = EditableFullTreeVM.TreeModel.SelectedNode.Entity;
            if (_prevSelectedNode != null &&
                _prevSelectedNode.Entity.EntityType == selectedNode.Entity.EntityType)

            {
                foreach (ITabItem tabItem in TabItems)
                {
                    tabItem.ParentEntity = parentEntity;
                    /*  if (tabItem is PropertiesViewModel)
                        continue;*/
                    tabItem.Refresh();
                }
                return;
            }
            _prevSelectedNode = selectedNode;


            SelectedTabItem = null;

            var propertiesValuesItem = new PropertiesValuesViewModel(parentEntity);
            var tabItems = new List<ITabItem> { propertiesValuesItem };
            if (selectedNode.Entity.EntityType == EntityType.CompStation)
                tabItems.Add(new CompStationCoolingRecomendedViewModel {ParentEntity = parentEntity});
            
            TabItems = tabItems;
            TabItems.ForEach(ti => ti.Refresh());

            SelectedTabItem = TabItems[0];
        }
    }
}