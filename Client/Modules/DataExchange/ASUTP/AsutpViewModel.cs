using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.DataExchange.ASUTPImport;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using Telerik.Windows.Controls;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DataExchange.Dialogs;
using System.Threading.Tasks;

namespace GazRouter.DataExchange.ASUTP
{
    public class AsutpViewModel : LockableViewModel
    {
        private ExchangeTaskDTO _selectedTask;

        private SiteDTO _selectedSite;

        private EntityType _selectedType;

        private List<ExchangeEntityDTO> _bindings;
        private List<ExchangePropertyDTO> _propertyBindings;

        private ItemBase _selectedItem;

        public bool IsEnabled { get; set; }
        public bool IsReadOnly { get; set; }

        public AsutpViewModel()
        {
            IsEnabled = Authorization2.Inst.IsEditable(LinkType.Asutp);
            IsReadOnly = !Authorization2.Inst.IsEditable(LinkType.Asutp);

            FindByExtIdCommand = new DelegateCommand(FindByExtId);

            EntityByExtId = new Dictionary<string, ExchangeEntityDTO>();

            EntityByPropertyExtId = new Dictionary<string, ExchangeEntityDTO>();

            ItemByEntityId = new Dictionary<Guid, ItemBase>();

            AncestorsOfItem = new Dictionary<ItemBase, List<ItemBase>>();

            SpecMode = false;

            SpecItem = null;

            SpecHierarchy = new List<ItemBase>();

            RefreshCommand = new DelegateCommand(Refresh);
            RunCommand = new DelegateCommand(Run, () => IsEnabled);

            Init();
        }

        public DelegateCommand RefreshCommand { get; private set; }

        /// <summary>
        ///     Список заданий обмена (Exchange_Task)
        /// </summary>
        public List<ExchangeTaskDTO> TaskList { get; set; }

        /// <summary>
        ///     Выбранное задание
        /// </summary>
        public ExchangeTaskDTO SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                if (SetProperty(ref _selectedTask, value))
                {
                    Refresh();
                }
            }
        }

        /// <summary>
        ///     Список ЛПУ
        /// </summary>
        public List<SiteDTO> SiteList { get; set; }

        /// <summary>
        ///     Выбранное ЛПУ
        /// </summary>
        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if (SetProperty(ref _selectedSite, value))
                {
                    if (!SpecMode)
                        Refresh();
                }
            }
        }

        public IEnumerable<EntityType> TypeList
        {
            get
            {
                yield return EntityType.CompStation;
                yield return EntityType.DistrStation;
                yield return EntityType.MeasStation;
                yield return EntityType.ReducingStation;
                yield return EntityType.Valve;
                yield return EntityType.OperConsumer;
            }
        }

        public EntityType SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (SetProperty(ref _selectedType, value))
                {
                    if (!SpecMode)
                        Refresh();
                }
            }
        }

        public List<ItemBase> Items { get; set; }

        public ItemBase SpecItem; // узел дерева, определенный с помощью идентификатора внешней системы, заданного пользователем

        public List<ItemBase> SpecHierarchy; // и его иерархия

        public ItemBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    LoadPropertyList();
                }
            }
        }

        public List<PropertyItem> PropertyList { get; set; }

        private PropertyItem _selectedProperty;

        public PropertyItem SelectedProperty
        {
            get { return _selectedProperty; }
            set { _selectedProperty = value; OnPropertyChanged(() => SelectedProperty); }
        }

        public async void Init()
        {
            // получить список заданий обмена
            TaskList = await new DataExchangeServiceProxy().GetExchangeTaskListAsync(
                new GetExchangeTaskListParameterSet
                {
                    SourceId = 8
                });
            OnPropertyChanged(() => TaskList);

            _selectedTask = TaskList.FirstOrDefault();
            OnPropertyChanged(() => SelectedTask);

            // получить список ЛПУ
            if (UserProfile.Current.Site.IsEnterprise)
            {
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                    new GetSiteListParameterSet
                    {
                        //EnterpriseId = UserProfile.Current.Site.Id
                    });
            }
            else
            {
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                    new GetSiteListParameterSet
                    {
                        SiteId = UserProfile.Current.Site.Id
                    });
            }

            OnPropertyChanged(() => SiteList);

            _selectedSite = SiteList.First();
            OnPropertyChanged(() => SelectedSite);

            _selectedType = EntityType.CompStation;
            OnPropertyChanged(() => SelectedType);

            Refresh();
        }

        public bool UpdateEntityBinding(ExchangeEntityDTO dto, string newId)
        {
            if (SelectedTask == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(newId) &&
                _bindings.Any(b => b.ExtId == newId && b.EntityId != dto.EntityId))
            {
                var bndng = _bindings.Single(b => b.ExtId == newId && b.EntityId != dto.EntityId);
                MessageBoxProvider.Alert(
                    $"Невозможно присвоить идентификатор, т.к. он уже присвоен другому объекту: \n\r\n\r{bndng.EntityPath}",
                    "Недопустимый идентификатор");

                return false;
            }

            new DataExchangeServiceProxy().SetExchangeEntityAsync(
                new AddEditExchangeEntityParameterSet
                {
                    EntityId = dto.EntityId,
                    ExchangeTaskId = SelectedTask.Id,
                    ExtId = string.IsNullOrEmpty(newId) ? null : newId,
                    IsActive = dto.IsActive
                });

            return true;
        }

        public bool UpdatePropertyBinding(ExchangePropertyDTO dto, string newId)
        {
            if (!string.IsNullOrEmpty(newId) && _propertyBindings.Any(b => b.ExtId == newId))
            {
                var bndng = _propertyBindings.Single(b => b.ExtId == newId);
                if (bndng.EntityId != dto.EntityId || bndng.PropertyTypeId != dto.PropertyTypeId)
                {
                    MessageBoxProvider.Alert(
                        string.Format(
                            "Невозможно присвоить идентификатор, т.к. он уже присвоен другому свойству: {0}{0}{1}{0}{2}",
                            Environment.NewLine,
                            _bindings.Single(e => e.EntityId == bndng.EntityId).EntityPath,
                            ClientCache.DictionaryRepository.PropertyTypes.Single(
                                pt => pt.PropertyType == bndng.PropertyTypeId).Name),
                        "Недопустимый идентификатор");
                }
                return false;
            }

            new DataExchangeServiceProxy().SetExchangePropertyAsync(
                new SetExchangePropertyParameterSet
                {
                    ExchangeTaskId = dto.ExchangeTaskId,
                    EntityId = dto.EntityId,
                    PropertyTypeId = dto.PropertyTypeId,
                    Coeff = dto.Coeff,
                    ExtId = newId
                });

            // Обновить ExtId в массиве _propertyBindings чтобы не загружать каждый раз массив с севрвера
            var oldBndng =
                _propertyBindings.SingleOrDefault(
                    b => b.EntityId == dto.EntityId && b.PropertyTypeId == dto.PropertyTypeId);
            if (oldBndng != null)
            {
                oldBndng.ExtId = newId;
                oldBndng.Coeff = dto.Coeff;
            }
            else
            {
                dto.ExtId = newId;
                _propertyBindings.Add(dto);
            }

            return true;
        }

        private async void Refresh()
        {
            if (SelectedSite == null || SelectedTask == null)
            {
                return;
            }

            try
            {
                Behavior.TryLock();

                // Получить список объектов для обмена
                _bindings = await new DataExchangeServiceProxy().GetExchangeEntityListAsync(
                    new GetExchangeEntityListParameterSet
                    {
                        ExchangeTaskIdList = { SelectedTask.Id }
                    });

                _propertyBindings = await new DataExchangeServiceProxy().GetExchangePropertyListAsync(
                    new GetExchangeEntityListParameterSet
                    {
                        ExchangeTaskIdList = { SelectedTask.Id }
                    });
                // Заполнить словари для поиска по идентификатору внешней системы
                EntityByExtId.Clear();
                foreach (ExchangeEntityDTO _binding in _bindings)
                    if (_binding.ExtId + "" != "")
                        if (!EntityByExtId.ContainsKey(_binding.ExtId)) EntityByExtId.Add(_binding.ExtId, _binding);
                EntityByPropertyExtId.Clear();
                foreach (ExchangePropertyDTO _property in _propertyBindings)
                {
                    if (_property.ExtId + "" != "")
                    {
                        foreach (ExchangeEntityDTO binding in _bindings)
                            if (binding.EntityId == _property.EntityId)
                                EntityByPropertyExtId.Add(_property.ExtId, binding);
                    }
                }
                #region refreshitems
                /*
                if (_selectedType == EntityType.CompStation)
                {
                    // Получить список ГПА
                    var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(SelectedSite.Id);

                    Items = new List<ItemBase>();

                    foreach (var station in stationTree.CompStations)
                    {
                        var stationItem = CreateBindableItem(station, _bindings);
                        Items.Add(stationItem);

                        foreach (var shop in stationTree.CompShops.Where(cs => cs.ParentId == station.Id))
                        {
                            var shopItem = CreateBindableItem(shop, _bindings);
                            stationItem.Children.Add(shopItem);


                            var unitFolder = new GroupItem("ГПА");
                            shopItem.Children.Add(unitFolder);
                            // ГПА
                            foreach (var unit in stationTree.CompUnits.Where(u => u.ParentId == shop.Id))
                                unitFolder.Children.Add(CreateBindableItem(unit, _bindings));


                            // Точки измерения параметров газа (физ-хим. показатели)
                            foreach (var mp in stationTree.MeasPoints.Where(mp => mp.ParentId == shop.Id))
                                shopItem.Children.Add(CreateBindableItem(mp, _bindings));

                        }

                        // СОГи
                        foreach (var cs in stationTree.CoolingStations)
                        {
                            var csItem = CreateBindableItem(cs, _bindings);
                            stationItem.Children.Add(csItem);

                            foreach (var cu in stationTree.CoolingUnits.Where(cu => cu.ParentId == cs.Id))
                                csItem.Children.Add(CreateBindableItem(cu, _bindings));

                        }
                    }
                }

                if (_selectedType == EntityType.DistrStation)
                {
                    var distrTree = await new ObjectModelServiceProxy().GetDistrStationTreeAsync(
                        new GetDistrStationListParameterSet
                        {
                            SiteId = SelectedSite.Id
                        });

                    Items = new List<ItemBase>();

                    foreach (var ds in distrTree.DistrStations)
                    {
                        var dsItem = CreateBindableItem(ds, _bindings);
                        Items.Add(dsItem);

                        var outFolderItem = new GroupItem("Выходы");
                        dsItem.Children.Add(outFolderItem);

                        // Выходы
                        foreach (var dso in distrTree.DistrStationOutlets.Where(o => o.ParentId == ds.Id))
                        {
                            var dsoItem = CreateBindableItem(dso, _bindings);
                            outFolderItem.Children.Add(dsoItem);
                        }

                        var consumerFolderItem = new GroupItem("Потребители");
                        dsItem.Children.Add(consumerFolderItem);

                        // Потребители
                        foreach (var cons in distrTree.Consumers.Where(o => o.DistrStationId == ds.Id))
                        {
                            var consoItem = CreateBindableItem(cons, _bindings);
                            consumerFolderItem.Children.Add(consoItem);
                        }

                        // Точки измерения параметров газа (физ-хим. показатели)
                        foreach (var mp in distrTree.MeasPoints.Where(mp => mp.ParentId == ds.Id))
                        {
                            var mpItem = CreateBindableItem(mp, _bindings);
                            dsItem.Children.Add(mpItem);
                        }
                    }
                }

                if (_selectedType == EntityType.MeasStation)
                {
                    var measTree = await new ObjectModelServiceProxy().GetMeasStationTreeAsync(
                        new GetMeasStationListParameterSet { SiteId = SelectedSite.Id });

                    Items = new List<ItemBase>();

                    foreach (var ms in measTree.MeasStations)
                    {
                        var msItem = new GroupItem(ms.Name);
                        Items.Add(msItem);
                        foreach (var ml in measTree.MeasLines.Where(m => m.ParentId == ms.Id))
                        {
                            var mlItem = CreateBindableItem(ml, _bindings);
                            msItem.Children.Add(mlItem);

                            // Точки измерения параметров газа (физ-хим. показатели)
                            foreach (var mp in measTree.MeasPoints.Where(mp => mp.ParentId == ml.Id))
                                mlItem.Children.Add(CreateBindableItem(mp, _bindings));
                        }
                    }
                }

                if (_selectedType == EntityType.ReducingStation)
                {
                    var rsList = await new ObjectModelServiceProxy().GetReducingStationListAsync(
                        new GetReducingStationListParameterSet
                        {
                            SiteId = SelectedSite.Id
                        });

                    Items = new List<ItemBase>();

                    foreach (var rs in rsList)
                        Items.Add(CreateBindableItem(rs, _bindings));
                }



                if (_selectedType == EntityType.Valve)
                {
                    var pipeTree = await new ObjectModelServiceProxy().GetPipelineTreeAsync(SelectedSite.Id);

                    Items = new List<ItemBase>();

                    foreach (var pipeType in ClientCache.DictionaryRepository.PipelineTypes.Values)
                    {
                        var typeItem = new GroupItem(pipeType.Name) { IsExpanded = false };
                        foreach (var pipe in pipeTree.Pipelines.Where(p => p.Type == pipeType.PipelineType))
                        {
                            var pipeItem = CreateBindableItem(pipe, _bindings);
                            typeItem.Children.Add(pipeItem);

                            foreach (var valve in pipeTree.LinearValves.Where(v => v.ParentId == pipe.Id))
                                pipeItem.Children.Add(CreateBindableItem(valve, _bindings));

                        }
                        if (typeItem.Children.Any())
                        {
                            Items.Add(typeItem);
                        }
                    }
                }


                if (_selectedType == EntityType.OperConsumer)
                {
                    var ocList = await new ObjectModelServiceProxy().GetOperConsumersAsync(null);

                    Items = new List<ItemBase>();

                    foreach (var cons in ocList.Where(c => c.ParentId == SelectedSite.Id))
                    {
                        Items.Add(CreateBindableItem(cons, _bindings));
                    }
                }

                OnPropertyChanged(() => Items);
                */
                #endregion
                await RefreshItems();
            }
            finally
            {
                Behavior.TryUnlock();
            }

            var root = new ItemBase() {Children = Items};
            Traversal(root, item => item.IsEditable = !IsReadOnly);
        }
        public void Traversal<T>(T data, Action<T> action) where T : ItemBase
        {
            action.Invoke(data);
            data.Children?.ForEach(item => Traversal((T)item, action));
        }

        private void addToDictItemWithHierarchy(Guid id, ItemBase item, List<ItemBase> hierarchi = null)
        {
            try
            {
                ItemByEntityId.Add(id, item);
                if (hierarchi != null)
                    AncestorsOfItem.Add(item, hierarchi);
            }
            catch { }
        }

        private async Task RefreshItems()
        {
            Items = new List<ItemBase>();
            ItemByEntityId.Clear();
            AncestorsOfItem.Clear();
            #region КС
            if (_selectedType == EntityType.CompStation)
            {
                var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(SelectedSite.Id);
                // КС
                foreach (var station in stationTree.CompStations)
                {
                    var stationItem = CreateBindableItem(station, _bindings);

                    addToDictItemWithHierarchy(stationItem.EntityId, stationItem);

                    //  КЦ
                    foreach (var shop in stationTree.CompShops.Where(cs => cs.ParentId == station.Id))
                    {
                        var shopItem = CreateBindableItem(shop, _bindings);

                        stationItem.Children.Add(shopItem);

                        addToDictItemWithHierarchy(shopItem.EntityId, shopItem, new List<ItemBase> { stationItem });

                        var unitFolder = new GroupItem("ГПА") { IsExpanded = false };
                        shopItem.Children.Add(unitFolder);

                        // ГПА
                        foreach (var unit in stationTree.CompUnits.Where(u => u.ParentId == shop.Id))
                        {
                            var unitItem = CreateBindableItem(unit, _bindings);

                            unitFolder.Children.Add(unitItem);

                            addToDictItemWithHierarchy(unitItem.EntityId, unitItem, new List<ItemBase> { stationItem, shopItem, unitFolder });
                        }


                        // Точки измерения параметров газа (физ-хим. показатели)

                        foreach (var mp in stationTree.MeasPoints.Where(mp => mp.ParentId == shop.Id))
                        {
                            var mpItem = CreateBindableItem(mp, _bindings);

                            shopItem.Children.Add(mpItem);

                            addToDictItemWithHierarchy(mpItem.EntityId, mpItem, new List<ItemBase> { stationItem, shopItem });
                        }
                    }

                    // СОГи

                    foreach (var cs in stationTree.CoolingStations.Where(cs => cs.ParentId == station.Id))
                    {
                        var csItem = CreateBindableItem(cs, _bindings);

                        stationItem.Children.Add(csItem);

                        addToDictItemWithHierarchy(csItem.EntityId, csItem, new List<ItemBase> { stationItem });

                        foreach (var cu in stationTree.CoolingUnits.Where(cu => cu.ParentId == cs.Id))
                        {
                            var cuItem = CreateBindableItem(cu, _bindings);

                            csItem.Children.Add(cuItem);

                            addToDictItemWithHierarchy(cuItem.EntityId, cuItem, new List<ItemBase> { stationItem, csItem });
                        }
                    }
                    Items.Add(stationItem);
                }
            }
            #endregion
            #region ГРС
            if (_selectedType == EntityType.DistrStation)
            {
                var distrTree = await new ObjectModelServiceProxy().GetDistrStationTreeAsync(
                    new GetDistrStationListParameterSet
                    {
                        SiteId = SelectedSite.Id
                    });

                foreach (var ds in distrTree.DistrStations)
                {
                    var dsItem = CreateBindableItem(ds, _bindings);

                    addToDictItemWithHierarchy(dsItem.EntityId, dsItem);

                    var outFolderItem = new GroupItem("Выходы") { IsExpanded = false };

                    dsItem.Children.Add(outFolderItem);

                    // Выходы
                    foreach (var dso in distrTree.DistrStationOutlets.Where(o => o.ParentId == ds.Id))
                    {
                        var dsoItem = CreateBindableItem(dso, _bindings);

                        outFolderItem.Children.Add(dsoItem);

                        addToDictItemWithHierarchy(dsoItem.EntityId, dsoItem, new List<ItemBase> { dsItem, outFolderItem });
                    }

                    var consumerFolderItem = new GroupItem("Потребители") { IsExpanded = false };

                    dsItem.Children.Add(consumerFolderItem);

                    // Потребители
                    foreach (var cons in distrTree.Consumers.Where(o => o.DistrStationId == ds.Id))
                    {
                        var consoItem = CreateBindableItem(cons, _bindings);

                        consumerFolderItem.Children.Add(consoItem);

                        addToDictItemWithHierarchy(consoItem.EntityId, consoItem, new List<ItemBase> { dsItem, consumerFolderItem });
                    }

                    // Точки измерения параметров газа (физ-хим. показатели)

                    foreach (var mp in distrTree.MeasPoints.Where(mp => mp.ParentId == ds.Id))
                    {
                        var mpItem = CreateBindableItem(mp, _bindings);

                        dsItem.Children.Add(mpItem);

                        addToDictItemWithHierarchy(mpItem.EntityId, mpItem, new List<ItemBase> { dsItem });
                    }
                    Items.Add(dsItem);
                }
            }
            #endregion
            #region ГИС
            if (_selectedType == EntityType.MeasStation)
            {
                var measTree = await new ObjectModelServiceProxy().GetMeasStationTreeAsync(
                    new GetMeasStationListParameterSet { SiteId = SelectedSite.Id });

                foreach (var ms in measTree.MeasStations)
                {
                    var msItem = new GroupItem(ms.Name) { IsExpanded = false };

                    foreach (var ml in measTree.MeasLines.Where(m => m.ParentId == ms.Id))
                    {
                        var mlItem = CreateBindableItem(ml, _bindings);

                        msItem.Children.Add(mlItem);

                        addToDictItemWithHierarchy(mlItem.EntityId, mlItem, new List<ItemBase> { msItem });

                        // Точки измерения параметров газа (физ-хим. показатели)

                        foreach (var mp in measTree.MeasPoints.Where(mp => mp.ParentId == ml.Id))
                        {
                            var mpItem = CreateBindableItem(mp, _bindings);

                            mlItem.Children.Add(mpItem);

                            addToDictItemWithHierarchy(mpItem.EntityId, mpItem, new List<ItemBase> { msItem, mlItem });
                        }
                    }

                    Items.Add(msItem);
                }
            }
            #endregion
            #region ПРГ
            if (_selectedType == EntityType.ReducingStation)
            {
                var rsList = await new ObjectModelServiceProxy().GetReducingStationListAsync(
                    new GetReducingStationListParameterSet
                    {
                        SiteId = SelectedSite.Id
                    });

                foreach (var rs in rsList)
                {
                    var rsItem = CreateBindableItem(rs, _bindings);

                    Items.Add(rsItem);

                    addToDictItemWithHierarchy(rsItem.EntityId, rsItem);
                }
            }
            #endregion
            #region Кран
            if (_selectedType == EntityType.Valve)
            {
                var pipeTree = await new ObjectModelServiceProxy().GetPipelineTreeAsync(SelectedSite.Id);

                foreach (var pipeType in ClientCache.DictionaryRepository.PipelineTypes.Values)
                {
                    var typeItem = new GroupItem(pipeType.Name) { IsExpanded = false };

                    foreach (var pipe in pipeTree.Pipelines.Where(p => p.Type == pipeType.PipelineType))
                    {
                        var pipeItem = CreateBindableItem(pipe, _bindings);

                        typeItem.Children.Add(pipeItem);

                        addToDictItemWithHierarchy(pipeItem.EntityId, pipeItem, new List<ItemBase> { typeItem });

                        foreach (var valve in pipeTree.LinearValves.Where(v => v.ParentId == pipe.Id))
                        {
                            var valveItem = CreateBindableItem(valve, _bindings);

                            pipeItem.Children.Add(valveItem);

                            addToDictItemWithHierarchy(valveItem.EntityId, valveItem, new List<ItemBase> { typeItem, pipeItem });
                        }
                    }
                    if (typeItem.Children.Any()) Items.Add(typeItem);
                }
            }
            #endregion
            #region ПЭН
            if (_selectedType == EntityType.OperConsumer)
            {
                var ocList = await new ObjectModelServiceProxy().GetOperConsumersAsync(null);
                foreach (var cons in ocList.Where(c => c.ParentId == SelectedSite.Id))
                {
                    var consItem = CreateBindableItem(cons, _bindings);
                    Items.Add(CreateBindableItem(cons, _bindings));
                    addToDictItemWithHierarchy(consItem.EntityId, consItem);
                }
            }
            #endregion
            OnPropertyChanged(() => Items);
        }


        private BindableItem CreateBindableItem(CommonEntityDTO e, List<ExchangeEntityDTO> bindings)
        {
            var rsBinding = _bindings.SingleOrDefault(b => b.EntityId == e.Id) ??
                            new ExchangeEntityDTO
                            {
                                EntityId = e.Id,
                                EntityName = e.Name,
                                EntityTypeId = e.EntityType
                            };

            return new BindableItem(rsBinding, UpdateEntityBinding);
        }



        // Загрузка списка свойств по выбранному объекту
        private void LoadPropertyList()
        {
            PropertyList = new List<PropertyItem>();

            var item = SelectedItem as BindableItem;
            if (item != null)
            {
                var propList =
                    ClientCache.DictionaryRepository.EntityTypes.Single(et => et.EntityType == item.EntityType)
                        .EntityProperties;

                foreach (var prop in propList)
                {
                    var bnd =
                        _propertyBindings.SingleOrDefault(
                            b => b.EntityId == item.EntityId && b.PropertyTypeId == prop.PropertyType) ??
                        new ExchangePropertyDTO
                        {
                            EntityId = item.EntityId,
                            PropertyTypeId = prop.PropertyType,
                            ExchangeTaskId = SelectedTask.Id
                        };
                    PropertyList.Add(new PropertyItem(prop.Name, bnd, UpdatePropertyBinding));
                }
            }
            OnPropertyChanged(() => PropertyList);
            #region specifiedproperty
            if (specifiedPropertyExtId != "")
            {
                foreach (PropertyItem pr in PropertyList)
                {
                    if (pr.ExtId == specifiedPropertyExtId)
                    {
                        SelectedProperty = pr;
                        break;
                    }
                }
            }
            #endregion
        }



        public IEnumerable<PeriodType> PeriodList
        {
            get
            {
                yield return PeriodType.Twohours;
                yield return PeriodType.Day;
            }
        }

        private PeriodType _selectedPeriod = PeriodType.Twohours;
        public PeriodType SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                {
                    OnPropertyChanged(() => IsDaySelected);

                    SelectedDate = value == PeriodType.Twohours ? SeriesHelper.GetCurrentSession() : SeriesHelper.GetCurrentDispDay();
                }
            }
        }

        public bool IsDaySelected => _selectedPeriod == PeriodType.Day;


        private DateTime _selectedDate = SeriesHelper.GetCurrentSession();
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }

        public DelegateCommand RunCommand { get; set; }

        private void Run()
        {
            new DataExchangeServiceProxy().AsutpImportAsync(
                new ASUTPImportParameterSet
                {
                    Timestamp = _selectedDate.ToLocal(),
                    PeriodType = SelectedPeriod
                });

            RadWindow.Alert(
                new DialogParameters
                {
                    Header = "Загрузка данных",
                    CancelButtonContent = "Закрыть",
                    OkButtonContent = "Закрыть",
                    Content = new TextBlock
                    {
                        Text = "Процесс загрузки данных запущен. Такая загрузка обычно занимает некоторое время (иногда весьма продолжительное). После завершения загрузки результат будет виден в журнале загрузок.",
                        Width = 300,
                        TextWrapping = TextWrapping.Wrap
                    }
                });
        }

        #region Spec
        // Все, относящееся к поиску по идентификатору внешней системы
        public bool SpecMode;  // режим
        public bool Reload;    // перезагрузка дерева
        public ExchangeEntityDTO SpecEntity = null; // объект
        public string specifiedEntityExtId;   // идентификатор внешней системы для объекта
        public string specifiedPropertyExtId; // идентификатор внешней системы для параметра
        // Словари:
        private Dictionary<string, ExchangeEntityDTO> EntityByExtId;
        private Dictionary<string, ExchangeEntityDTO> EntityByPropertyExtId;
        public Dictionary<Guid, ItemBase> ItemByEntityId; // узлы загруженного дерева
        public Dictionary<ItemBase, List<ItemBase>> AncestorsOfItem; //их иерархии
        public DelegateCommand FindByExtIdCommand { get; set; }
        private void FindByExtId()
        {
            var findByExtIdViewModel = new FindByExtIdViewModel(SelectByExtId);
            var view = new FindByExtIdView { DataContext = findByExtIdViewModel };
            view.ShowDialog();
        }
        private async void SelectByExtId(string extId)
        {
            specifiedEntityExtId = specifiedPropertyExtId = "";
            List<SpecifiedEntityOrProperty> list = new List<SpecifiedEntityOrProperty>();
            new SpecifiedEntityOrProperty().exactEntityMatch(extId, EntityByExtId, list);
            new SpecifiedEntityOrProperty().exactPropertyMatch(extId, EntityByPropertyExtId, _propertyBindings, list);
            if (list.Count == 0)
            {
                new SpecifiedEntityOrProperty().aroundEntityMatch(extId, EntityByExtId, list);
                new SpecifiedEntityOrProperty().aroundPropertyMatch(extId, EntityByPropertyExtId, _propertyBindings, list);
            }
            if (list.Count == 1)
            {
                SpecifiedEntityOrProperty eorp = list[0];
                specifiedEntityExtId = eorp.entity.ExtId;
                if (eorp.proptype != PropertyType.None)
                    specifiedPropertyExtId = eorp.extid;
                SpecEntity = eorp.entity;
                await SpecProcess();
                return;
            }
            if (list.Count > 1) // выбор из нескольких удовлетворяющих критериям объектов (параметров)
            {
                var selectObjectOrParameterViewModel = new SelectObjectOrParameterViewModel(SelectedObjectOrParameter, list);
                var view = new SelectObjectOrParameter { DataContext = selectObjectOrParameterViewModel };
                view.ShowDialog();
                return;
            }
            NotFound();
        }
        private async void SelectedObjectOrParameter(SpecifiedEntityOrProperty redefinedEntityOrProperty)
        {
            if (redefinedEntityOrProperty != null)
            {
                specifiedEntityExtId = redefinedEntityOrProperty.entity.ExtId;
                if (redefinedEntityOrProperty.proptype != PropertyType.None)
                    specifiedPropertyExtId = redefinedEntityOrProperty.extid;
                SpecEntity = redefinedEntityOrProperty.entity;
                await SpecProcess();
            }
        }
        private async Task SpecProcess()
        {
            SpecMode = true;
            Reload = false;
            SiteDTO specifiedSite = null;
            EntityType specifiedType = new EntityType();
            if (SpecMode && specifiedPropertyExtId != "")
            {
                foreach (PropertyItem prop in PropertyList)
                {
                    if (prop.ExtId == specifiedPropertyExtId)
                    {
                        SelectedProperty = prop; // выбираем нужный параметр
                        OnPropertyChanged(() => SelectedProperty);
                        SpecMode = false;
                        return;
                    }
                }
            }
            FindSpecItemInTree();
            if (SpecItem == null)
            {
                try
                {
                    Behavior.TryLock();
                    foreach (SiteDTO site in SiteList)
                    {
                        if (SpecEntity.EntityPath.Contains(site.Name))
                        {
                            specifiedSite = site;
                            break;
                        }
                    }
                    if (specifiedSite == null)
                        specifiedSite = await SpecifiedSite();
                    if (specifiedSite != null)
                    {
                        specifiedType = await SpecifiedType();
                        if (SelectedSite != specifiedSite || SelectedType != specifiedType)
                        {
                            SelectedSite = specifiedSite;
                            SelectedType = specifiedType;
                            Reload = true;
                            await RefreshItems();
                        }
                    }
                    else NotFound();
                }
                catch
                {
                    NotFound();
                }
                finally
                {
                    Behavior.TryUnlock();
                }
            }
        }
        private void NotFound()
        {
            MessageBoxProvider.Alert("Объект не найден!", "Сообщение");
            SpecMode = false;
        }
        private async Task<SiteDTO> SpecifiedSite()
        {
            var siteId = await new ObjectModelServiceProxy().FindSiteAsync(SpecEntity.EntityId);
            if (siteId != null)
                foreach (SiteDTO site in SiteList)
                    if ((Guid)siteId == site.Id)
                        return site;
            foreach (SiteDTO site in SiteList)
            {
                var pipeTree = await new ObjectModelServiceProxy().GetPipelineTreeAsync(site.Id);
                if (SpecEntity.EntityTypeId == EntityType.Pipeline)
                {
                    if (pipeTree.Pipelines.Any(pipeline => pipeline.Id == SpecEntity.EntityId))
                        return site;
                }
                else if (SpecEntity.EntityTypeId == EntityType.Valve)
                    if (pipeTree.LinearValves.Any(valve => valve.Id == SpecEntity.EntityId))
                        return site;
            }
            return null;
        }
        private async Task<EntityType> SpecifiedType()
        {
            if (SpecEntity.EntityTypeId == EntityType.CompShop ||
                SpecEntity.EntityTypeId == EntityType.CompUnit ||
                SpecEntity.EntityTypeId == EntityType.CoolingStation ||
                SpecEntity.EntityTypeId == EntityType.CoolingUnit)
                return EntityType.CompStation;
            if (SpecEntity.EntityTypeId == EntityType.Pipeline)
                return EntityType.Valve;
            if (SpecEntity.EntityTypeId == EntityType.MeasLine)
                return EntityType.MeasStation;
            if (SpecEntity.EntityTypeId == EntityType.Consumer ||
                SpecEntity.EntityTypeId == EntityType.DistrStationOutlet)
                return EntityType.DistrStation;
            if (SpecEntity.EntityTypeId == EntityType.MeasPoint)
            {
                var measPoint = await new ObjectModelServiceProxy().GetEntityByIdAsync(SpecEntity.EntityId);
                if (measPoint != null)
                {
                    if (((MeasPointDTO)(measPoint)).MeasLineId != null)
                        return EntityType.MeasStation;
                    if (((MeasPointDTO)(measPoint)).DistrStationId != null)
                        return EntityType.DistrStation;
                    if (((MeasPointDTO)(measPoint)).CompShopId != null)
                        return EntityType.CompStation;
                }
            }
            return SpecEntity.EntityTypeId; // EntityType.DistrStation || EntityType.MeasStation || EntityType.ReducingStation || EntityType.OperConsumer || EntityType.Valve
        }
        public void FindSpecItemInTree()
        {
            ItemByEntityId.TryGetValue(SpecEntity.EntityId, out SpecItem);
            if (SpecItem != null) // Если объект - в загруженном дереве,
            {  // находим иерархию, если она есть, и выбираем нужный объект
                AncestorsOfItem.TryGetValue(SpecItem, out SpecHierarchy);
                if ((SpecItem as BindableItem).EntityType == EntityType.OperConsumer)
                    LocateItemInTree(SpecEntity);
                else SelectedItem = SpecItem;
            }
        }
        private void LocateItemInTree(ExchangeEntityDTO entity)
        {
            SelectedItem = LocateItemInTree(Items, entity);
        }
        private ItemBase LocateItemInTree(List<ItemBase> items, ExchangeEntityDTO entity)
        {
            foreach (var item in items)
            {
                BindableItem bindItem = item as BindableItem;
                if (bindItem != null && bindItem.ExtId == entity.ExtId)
                    return item;

                var subResult = LocateItemInTree(item.Children, entity);
                if (subResult != null)
                    return subResult;
            }
            return null;
        }
        public class SpecifiedEntityOrProperty
        {
            public string extid { get; set; }
            public string path { get; set; }
            public ExchangeEntityDTO entity { get; set; }
            public PropertyType proptype { get; set; }
            SpecifiedEntityOrProperty Create(string id, ExchangeEntityDTO ent, string pth, PropertyType type)
            {
                return new SpecifiedEntityOrProperty() { extid = id, path = pth, entity = ent, proptype = type };
            }
            public void exactEntityMatch(string extId, Dictionary<string, ExchangeEntityDTO> sourceDict, List<SpecifiedEntityOrProperty> list)
            {
                ExchangeEntityDTO ent;
                if (sourceDict.TryGetValue(extId, out ent))
                    list.Add(new SpecifiedEntityOrProperty().Create(extId, ent, ent.EntityPath, PropertyType.None));
            }
            public void exactPropertyMatch(string extId, Dictionary<string, ExchangeEntityDTO> sourceDict, List<ExchangePropertyDTO> prBindings, List<SpecifiedEntityOrProperty> list)
            {
                ExchangeEntityDTO ent;
                if (sourceDict.TryGetValue(extId, out ent))
                    list.Add(EntityWithPropertyParams(extId, ent, prBindings));
            }
            SpecifiedEntityOrProperty EntityWithPropertyParams(string extId, ExchangeEntityDTO ent, List<ExchangePropertyDTO> prBindings)
            {
                string Path = ent.EntityPath;
                PropertyType Propertytype = PropertyType.None;
                var propList = ClientCache.DictionaryRepository.EntityTypes.Single(et => et.EntityType == ent.EntityTypeId).EntityProperties;
                foreach (var pr in propList)
                {
                    if (prBindings.SingleOrDefault(b => b.EntityId == ent.EntityId && b.PropertyTypeId == pr.PropertyType && b.ExtId == extId) != null)
                    {
                        Path += " (" + pr.Name + ")";
                        Propertytype = pr.PropertyType;
                        break;
                    }
                }
                return new SpecifiedEntityOrProperty().Create(extId, ent, Path, Propertytype);
            }
            public void aroundEntityMatch(string extId, Dictionary<string, ExchangeEntityDTO> sourceDict, List<SpecifiedEntityOrProperty> list)
            {
                ExchangeEntityDTO ent;
                Dictionary<string, ExchangeEntityDTO>.KeyCollection keyCollection = sourceDict.Keys;
                List<string> keyList = (from kC in keyCollection where (String.Compare(kC, extId, StringComparison.CurrentCultureIgnoreCase) == 0) select kC).ToList<string>();
                if (keyList.Count > 0)
                {
                    foreach (string eid in keyList)
                    {
                        if (sourceDict.TryGetValue(eid, out ent))
                            list.Add(new SpecifiedEntityOrProperty().Create(eid, ent, ent.EntityPath, PropertyType.None));
                    }
                }
            }
            public void aroundPropertyMatch(string extId, Dictionary<string, ExchangeEntityDTO> sourceDict, List<ExchangePropertyDTO> prBindings, List<SpecifiedEntityOrProperty> list)
            {
                ExchangeEntityDTO ent;
                Dictionary<string, ExchangeEntityDTO>.KeyCollection keyCollection = sourceDict.Keys;
                List<string> keyList = (from kC in keyCollection where (String.Compare(kC, extId, StringComparison.CurrentCultureIgnoreCase) == 0) select kC).ToList<string>();
                if (keyList.Count > 0)
                {
                    foreach (string eid in keyList)
                    {
                        if (sourceDict.TryGetValue(eid, out ent))
                            list.Add(EntityWithPropertyParams(eid, ent, prBindings));
                    }
                }
            }
        }
        #endregion

    }
}