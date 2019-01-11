using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs.EntityPicker;
using GazRouter.Controls.Tree;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Entities;
using Microsoft.Practices.Prism.Commands;
namespace GazRouter.Controls
{
    public class TreeEntityPickerViewModel : DialogViewModel
    {
#region new
        public ObservableCollection<EntityTypeFilterWrapper> SelectedItems
        {
            get; private set;
            //            get
            //            {
            //                //return _selectedItems ?? (_selectedItems = new ObservableCollection<EntityTypeFilterWrapper>());
            //                return _selectedItems;
            //            }
            //            set
            //            {
            //                _selectedItems = value;
            //                OnPropertyChanged(() => SelectedItems);
            //            }
        }
        public List<EntityType> AllowedTypes { get; private set; }
        protected List<EntityType> FastTypeList;
        private readonly List<EntityTypeFilterWrapper> _fastEntityTypeList;
        public IEnumerable<EntityTypeFilterWrapper> FastEntityTypeList
        {
            get { return _fastEntityTypeList; }
        }
        private EntityTypeFilterWrapper _selectedEntityType;
        public EntityTypeFilterWrapper SelectedEntityType
        {
            get { return _selectedEntityType; }
            set
            {
                if (_selectedEntityType == value) return;
                _selectedEntityType = value;
                OnPropertyChanged(() => SelectedEntityType);
                Refresh();
            }
        }
        private bool _isPointObjectsTabVisible;
        public bool IsPointObjectsTabVisible
        {
            get { return _isPointObjectsTabVisible; }
            set
            {
                if (_isPointObjectsTabVisible == value) return;
                _isPointObjectsTabVisible = value;
                OnPropertyChanged(() => IsPointObjectsTabVisible);
            }
        }
#endregion

#region constructor
        public TreeEntityPickerViewModel(Action<CommonEntityDTO> closeCallback, List<EntityType> allowedTypes) : base(null)
        {
#region
            // todo:  необходимость FastTypeList???
            FastTypeList = new List<EntityType>
            {
                // Enterprises       - отсутствует в фильтрах
                // sites             - установлено по дефолту
                // EntityType.Valve, - отсутствует
                // EntityFilter.CoolingUnits - отсутствует в allowedTypes, но присутствует в исходной версии фильтров???
                // EntityFilter.PowerPlants  - отсутствует в allowedTypes, но присутствует в исходной версии фильтров???
                // 
                EntityType.CompStation,        // 1
                EntityType.CompShop,           // 2
                EntityType.CompUnit,           // 3
                EntityType.DistrStation,       // 4
                EntityType.MeasStation,        // 5
                EntityType.ReducingStation,    // 6
            };
            AllowedTypes = allowedTypes ?? new List<EntityType>();
            // 
            _fastEntityTypeList = (AllowedTypes.Count > 0
                ? ClientCache.DictionaryRepository.EntityTypes.Where(
                    p => FastTypeList.Contains(p.EntityType) && AllowedTypes.Contains(p.EntityType))
                : ClientCache.DictionaryRepository.EntityTypes.Where(p => FastTypeList.Contains(p.EntityType)))
                .Select(et => new EntityTypeFilterWrapper(et)).ToList();
            //
#endregion
            _isLinarTabSelected = true;
#region SelectCommand 
            SelectCommand = new DelegateCommand(async () =>
                                                {        // Ok
                                                    Behavior.TryLock();
                                                    var commonEntityDTO = await new ObjectModelServiceProxy().GetEntityByIdAsync(IsLinarTabSelected ?
                                                                TreeLinarModel.SelectedNode.Entity.Id : 
                                                                TreePipelineModel.SelectedNode.Entity.Id);
                                                    closeCallback(commonEntityDTO);
                                                    Behavior.TryUnlock();
                                                    DialogResult = false;

                                                }, () => // RaiseCanExecuteChanged
                                                {
                                                    if (IsLinarTabSelected)
                                                    {
                                                        if (TreeLinarModel.SelectedNode is EntityNode)
                                                        {
                                                            var entityNode = TreeLinarModel.SelectedNode as EntityNode;
                                                            if (allowedTypes.Contains(entityNode.Entity.EntityType))
                                                            return true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (TreePipelineModel.SelectedNode is EntityNode)
                                                        {
                                                            var entityNode = TreePipelineModel.SelectedNode as EntityNode;
                                                            if (allowedTypes.Contains(entityNode.Entity.EntityType))
                                                                return true;
                                                        }
                                                    }
                                                    return false;
                                                });
#endregion 
            GasTransportList = ClientCache.DictionaryRepository.GasTransportSystems;
            _selectedGasTransport = GasTransportList.First();// _fastEntityTypeList.Insert(0, new EntityTypeFilterWrapper(null));//SelectedEntityType = _fastEntityTypeList.ToList();
            SelectedItems = new ObservableCollection<EntityTypeFilterWrapper>(_fastEntityTypeList);
            SelectedItems.CollectionChanged += (sender, args) => { Refresh(); };// SelectedItems = new ObservableCollection<EntityTypeFilterWrapper>(_fastEntityTypeList);//Refresh();
            Refresh();
        }
#endregion

#region refresh
        private void Refresh()
        {
            RefreshPointObjectTree();
            RefreshLinearObjectTree();
        }
        private async void RefreshLinearObjectTree()
        {
            Behavior.TryLock();
            var filter = new EntityTreeGetParameterSet
            {
                Filter = EntityFilter.Pipelines |
                         EntityFilter.LinearValves |
                         EntityFilter.Boilers |
                         EntityFilter.PowerUnits,
                SystemId = SelectedGasTransport.Id
            };
            if (!UserProfile.Current.Site.IsEnterprise)
                filter.SiteId = UserProfile.Current.Site.Id;
            var pipelineTreeData = await new ObjectModelServiceProxy().GetFullTreeAsync(filter);
            TreePipelineModel.FillTree(pipelineTreeData, null);
            Behavior.TryUnlock();
        }
        private async void RefreshPointObjectTree()
        {
            Behavior.TryLock();
            var treeParams = new EntityTreeGetParameterSet
            {
                SystemId = SelectedGasTransport.Id,
                Filter = EntityFilter.Sites |
                         EntityFilter.CoolingUnits |
                         EntityFilter.PowerPlants |
                         EntityFilter.PowerUnits |
                         EntityFilter.BoilerPlants | 
                         EntityFilter.Boilers
            };            
            foreach (var entityType in SelectedItems) treeParams.Filter |= entityType.Filter;
            var linarData = await new ObjectModelServiceProxy().GetFullTreeAsync(treeParams);
            var linarTreeData = new TreeData
            {
                Enterprises = ClientCache.DictionaryRepository.Enterprises,//1
                Sites = linarData.Sites,//2
                CompStations = linarData.CompStations,//3
                CompShops = linarData.CompShops,//4
                DistrStations = linarData.DistrStations,//5
                MeasStations = linarData.MeasStations,//6
                MeasLines = linarData.MeasLines,//7
                ReducingStations = linarData.ReducingStations,//8
                BoilerPlants = linarData.BoilerPlants,//9
                Boilers = linarData.Boilers,//10
                CompUnits = linarData.CompUnits,//11
                Consumers = linarData.Consumers,//12
                CoolingStations = linarData.CoolingStations,//13
                CoolingUnits = linarData.CoolingUnits,//14
                DistrStationOutlets = linarData.DistrStationOutlets,//15
                MeasPoints = linarData.MeasPoints,//16
                PowerPlants = linarData.PowerPlants,//17
                PowerUnits = linarData.PowerUnits//18
            };
            TreeLinarModel.FillTree(linarTreeData);
            Behavior.TryUnlock();
        }
#endregion

#region properties
        public DelegateCommand SelectCommand { get; set; }
        private bool _isLinarTabSelected;
        public bool IsLinarTabSelected
        {
            get { return _isLinarTabSelected; }
            set
            {
                SetProperty(ref _isLinarTabSelected, value);
                SelectCommand.RaiseCanExecuteChanged();
            }
        }
        private List<GasTransportSystemDTO> _gasTransportList;
        public List<GasTransportSystemDTO> GasTransportList
        {
            get { return _gasTransportList; }
            set
            {
                SetProperty(ref _gasTransportList, value);
                OnPropertyChanged(() => GasTransportList);
            }
        }
        private GasTransportSystemDTO _selectedGasTransport;
        public GasTransportSystemDTO SelectedGasTransport
        {
            get { return _selectedGasTransport; }
            set
            {
                if (value == null) return;
                SetProperty(ref _selectedGasTransport, value);
                OnPropertyChanged(() => SelectedGasTransport);
                Refresh();
            }
        }
        private TreeViewModelPointObjects _treeLinarModel;
        public TreeViewModelPointObjects TreeLinarModel
        {
            get
            {
                if (_treeLinarModel == null)
                {
                    _treeLinarModel = new TreeViewModelPointObjects();
                    _treeLinarModel.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == "SelectedNode")
                            SelectCommand.RaiseCanExecuteChanged();
                    };
                }
                return _treeLinarModel;
            }
        }
        private TreeViewModelPipeline _treePipelineModel;
        public TreeViewModelPipeline TreePipelineModel
        {
            get
            {
                if (_treePipelineModel == null)
                {
                    _treePipelineModel = new TreeViewModelPipeline();
                    _treePipelineModel.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == "SelectedNode")
                            SelectCommand.RaiseCanExecuteChanged();
                    };
                }
                return _treePipelineModel;
            }
        }
#endregion
    }
    public class EntityTypeFilterWrapper : EntityTypeWrapper
    {
        public EntityTypeFilterWrapper(EntityTypeDTO dto) : base(dto)
        {
            Filter = GetEntityFilter(dto.EntityType);
        }
        public override string Name => Dto.ShortName;
        public EntityFilter Filter { get; private set; }
        public static EntityFilter GetEntityFilter(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Boiler: return EntityFilter.Boilers;
                case EntityType.Consumer: return EntityFilter.Consumers;
                case EntityType.Site: return EntityFilter.Sites;
                case EntityType.BoilerPlant: return EntityFilter.BoilerPlants;
                case EntityType.CompShop: return EntityFilter.CompShops;
                case EntityType.DistrStation: return EntityFilter.DistrStations;                    
                // case EntityType.Enterprise: 
                // case EntityType.Valve: EntityFilter.LinearValves; ???                    
                case EntityType.MeasLine: return EntityFilter.MeasLines;
                case EntityType.CompStation: return EntityFilter.CompStations;
                case EntityType.CompUnit: return EntityFilter.CompUnits;
                // case EntityType.CngFillingStation:return EntityFilter;
                case EntityType.ReducingStation: return EntityFilter.ReducingStations;                    
                case EntityType.MeasStation: return EntityFilter.MeasStations;
                case EntityType.PowerUnit: return EntityFilter.PowerUnits;
                case EntityType.DistrStationOutlet: return EntityFilter.DistrStationOutlets;
                case EntityType.Pipeline: return EntityFilter.Pipelines;
                // case EntityType.PipelineGroup:return EntityFilter;
                case EntityType.MeasPoint: return EntityFilter.MeasPoints;
                case EntityType.CoolingStation: return EntityFilter.CoolingStations;
                case EntityType.CoolingUnit: return EntityFilter.CoolingUnits;
                case EntityType.PowerPlant: return EntityFilter.PowerPlants;
                // case EntityType.PreparationUnit: return EntityFilter;
                default:
                    throw new ArgumentOutOfRangeException(nameof(entityType));
            }
        }
    }
}
#region trash
//EntityType.MeasPoint,          // 9
//                EntityType.CoolingStation,     // 10
//                EntityType.CoolingUnit,        // 11
//                EntityType.PowerPlant,         // 12
//                EntityType.PowerUnit,          // 13
//                EntityType.Boiler,             // 14
//                EntityType.BoilerPlant,        // 15
//                EntityType.Pipeline,           // 16

//private ObservableCollection<EntityTypeFilterWrapper> _selectedItems;

/*if (collection.has(EntityType) ) */ // var list = new List<EntityFilter> {EntityFilter.Boilers, EntityFilter.MeasLines, EntityFilter.CompStations};

//            var t = new EntityTreeGetParameterSet
//            {
//                Filter = EntityFilter.Sites | 
//                         EntityFilter.CompStations |
//                         EntityFilter.CompShops | 
//                         EntityFilter.CompUnits |
//                         EntityFilter.DistrStations | 
//                         EntityFilter.MeasStations |
//                         EntityFilter.ReducingStations | 
//                         EntityFilter.MeasLines |
//                         EntityFilter.DistrStationOutlets | 
//                         EntityFilter.Consumers |
//                         EntityFilter.MeasPoints | 
//                         EntityFilter.CoolingStations |
//                         EntityFilter.CoolingUnits | 
//                         EntityFilter.PowerPlants |
//                         EntityFilter.PowerUnits | 
//                         EntityFilter.BoilerPlants |
//                         EntityFilter.Boilers,
//                SystemId = SelectedGasTransport.Id
//            };


// throw new Exception("Недопустимый тип!");
//            var t = new EntityTreeGetParameterSet
//            {
//                Filter = SelectedEntityType.Dto.EntityType == EntityType.Site,
//                SystemId = SelectedGasTransport.Id
//            };
//      public EntityTypeDTO Dto { get; private set; }
#endregion
