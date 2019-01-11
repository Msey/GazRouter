using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Tree;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.ObjectModel.Model.Tabs.PropertyValues;
using GazRouter.ObjectModel.Model.Tabs.Segments.Diameter;
using GazRouter.ObjectModel.Model.Tabs.Segments.Group;
using GazRouter.ObjectModel.Model.Tabs.Segments.Pressure;
using GazRouter.ObjectModel.Model.Tabs.Segments.Site;
using GazRouter.ObjectModel.Model.Tabs.Segments.Regions;

namespace GazRouter.ObjectModel.Model.Pipelines
{
    public class PipeLineManagerViewModel : PropertyChangedBase
    {
        private ITabItem _selectedTabItem;
        private List<ITabItem> _tabItems;

        public PipeLineManagerViewModel()
        {
            _tabItems = new List<ITabItem>();
            EditableFullTreeVM = new EditableFullTreeViewModel();
            EditableFullTreeVM.TreeModel.PropertyChanged += TreeModelPropertyChanged;
        }

		#region IsSelected

		private bool _isSelected;

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				OnPropertyChanged(() => IsSelected);
			}
		}


		#endregion

        public EditableFullTreeViewModel EditableFullTreeVM { get; }

        public ITabItem SelectedTabItem
        {
            get { return _selectedTabItem; }
            set
            {
                _selectedTabItem = value;

                OnPropertyChanged(() => SelectedTabItem);
            }
        }

        public List<ITabItem> TabItems
        {
            get { return _tabItems; }
            set
            {
                _tabItems = value;
                OnPropertyChanged(() => TabItems);
            }
        }

        private void TreeModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedNode")
            {
                RefreshTabsList();
            }
        }

        private void RefreshTabsList()
        {

	        var temptab = SelectedTabItem == null ? string.Empty : SelectedTabItem.Header;
            var selectedNode = EditableFullTreeVM.TreeModel.SelectedNode;
            if (selectedNode == null || selectedNode is FolderNode)
            {
                TabItems = new List<ITabItem>();
                return;
            }

            var entityNode = EditableFullTreeVM.TreeModel.SelectedNode as EntityNode;
            if (entityNode != null)
            {
                var parentEntity = entityNode.Entity;
                switch (selectedNode.Entity.EntityType)
                {
                    case EntityType.Pipeline:
                        var segment = new SiteSegmentViewModel {ParentEntity = parentEntity};
                        var pipeSegment = new PressureSegmentViewModel {ParentEntity = parentEntity};
                        var pipediameterSegment = new DiameterSegmentViewModel { ParentEntity = parentEntity };
                        var regionSegment = new RegionSegmentViewModel { ParentEntity = parentEntity };
                        var groupSegment = new GroupSegmentViewModel { ParentEntity = parentEntity };
                        TabItems = new List<ITabItem> {
                            segment,
                            regionSegment,
                            pipeSegment,
                            pipediameterSegment,
                            groupSegment };
                        break;
                    default:
                        TabItems =
                            new List<ITabItem>
                                {
                                    new PropertiesValuesViewModel(parentEntity)
                                };
                        break;
                }
            }
            foreach (var tabItem in TabItems)
            {
                tabItem.Refresh();
            }
            SelectedTabItem = TabItems.ToArray().FirstOrDefault(p => p.Header == temptab) ?? TabItems.FirstOrDefault();
        }
    }
}