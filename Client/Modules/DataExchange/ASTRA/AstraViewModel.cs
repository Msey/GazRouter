using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Browser;
using DataExchange.CustomSource.Dialogs;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataExchange.CustomSource.Dialogs;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Commands;
using Utils.Extensions;
using UriBuilder = GazRouter.DataProviders.UriBuilder;
using GazRouter.DataExchange.Dialogs;
using System.Threading.Tasks;

namespace GazRouter.DataExchange.ASTRA
{
    public class AstraViewModel : LockableViewModel
    {
        private const int AstraDataSourceId = 3;

        private List<ExchangeEntityDTO> _bindings;

        private List<ExchangeTypeDTO> _exchangTypeList;

        private DateTime? _selectedDate;

        private ExchangeTypeDTO _selectedExchangeType;

        private PeriodType _selectedPeriod = PeriodType.Twohours;

        private SiteDTO _selectedSite;

        private ExchangeTaskDTO _selectedTask;

        private EntityType _selectedType;



        #region Spec
        // Все, относящееся к поиску по идентификатору внешней системы
        public bool SpecMode;  // режим
        public ExchangeEntityDTO SpecEntity = null; // объект
        // Словари:
        private Dictionary<string, ExchangeEntityDTO> EntityByExtId; // все объекты, у которых есть идентификатор внешней системы
        private Dictionary<string, ItemBase> ItemByExtId; // все узлы дерева, у которых есть идентификатор внешней системы
        private Dictionary<ItemBase, List<ItemBase>> AncestorsOfItem; // и их иерархии
        public ItemBase SpecItem; // узел дерева, определенный с помощью идентификатора внешней системы, заданного пользователем
        public List<ItemBase> SpecHierarchy; // и его иерархия
        private void NotFoundInfo() // Сообщение, если объект не найден
        {
            MessageBoxProvider.Alert("Объект не найден!", "Сообщение");
            SpecMode = false;
        }
        public DelegateCommand FindByExtIdCommand { get; set; }
        private void FindByExtId()
        {
            var findByExtIdViewModel = new FindByExtIdViewModel(SelectByExtId);
            var view = new FindByExtIdView { DataContext = findByExtIdViewModel };
            view.ShowDialog();
        }
        private async void SelectByExtId(string extId)
        {
            
            Dictionary<string, ExchangeEntityDTO> qualifyingDict = new Dictionary<string, ExchangeEntityDTO>();
            if (EntityByExtId.TryGetValue(extId, out SpecEntity))
            {
                await SpecProcess();
                return;
            }
            Dictionary<string, ExchangeEntityDTO>.KeyCollection keyColl = EntityByExtId.Keys;
            List<string> kcList = (from kc in keyColl where (String.Compare(kc, extId, StringComparison.CurrentCultureIgnoreCase) == 0) select kc).ToList<string>();
            if (kcList.Count() == 1)
            {
                if (EntityByExtId.TryGetValue(kcList[0], out SpecEntity))
                {
                    await SpecProcess();
                    return;
                }
            }
            if (kcList.Count() > 1) // высветить окно для выбора объекта
            {
                bool allRight = true;
                qualifyingDict.Clear();
                ExchangeEntityDTO Entity = null;
                foreach (string key in kcList)
                {
                    if (allRight)
                    {
                        if (EntityByExtId.TryGetValue(key, out Entity))
                            qualifyingDict.Add(key, Entity);
                        else allRight = false;
                    }
                }
                if (allRight)
                {
                    var selectObjectViewModel = new SelectObjectViewModel(SelectedObject, qualifyingDict);
                    var view = new SelectObject { DataContext = selectObjectViewModel };
                    view.ShowDialog();
                    return;
                }
            }
            NotFoundInfo();
        }
        private async void SelectedObject(ExchangeEntityDTO redefinedEntity)
        {
            if (redefinedEntity != null)
            {
                SpecEntity = redefinedEntity;
                await SpecProcess();
            }
        }
        private async Task<SiteDTO> SpecifiedSite()
        {
            var siteId = await new ObjectModelServiceProxy().FindSiteAsync(SpecEntity.EntityId);
            if (siteId != null)
                foreach (SiteDTO site in SiteList)
                    if (siteId == site.Id)
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
        private EntityType SpecifiedType()
        {
            if (SpecEntity.EntityTypeId == EntityType.CompStation ||
                SpecEntity.EntityTypeId == EntityType.CompUnit ||
                SpecEntity.EntityTypeId == EntityType.CoolingStation ||
                SpecEntity.EntityTypeId == EntityType.CoolingUnit)
                return EntityType.CompShop;
            if (SpecEntity.EntityTypeId == EntityType.Pipeline)
                return EntityType.Valve;
            if (SpecEntity.EntityTypeId == EntityType.MeasLine)
                return EntityType.MeasStation;
            if (SpecEntity.EntityTypeId == EntityType.Consumer ||
                SpecEntity.EntityTypeId == EntityType.DistrStationOutlet)
                return EntityType.DistrStation;
            return SpecEntity.EntityTypeId; // EntityType.CompShop || EntityType.Valve || EntityType.MeasStation || EntityType.DistrStation
        }
        public bool IsSpecItemInTree()
        {
            bool isIn = false;
            ItemByExtId.TryGetValue(SpecEntity.ExtId, out SpecItem);
            if (SpecItem != null) // Если объект - в загруженном дереве
            {
                AncestorsOfItem.TryGetValue(SpecItem, out SpecHierarchy);
                if (SelectedItem == SpecItem) // если искомый объект в загруженном дереве и выбран, но может быть не виден,
                    SelectedItem = null;      // принудительно создадим событие "TreeListView_SelectionChanged"
                SelectedItem = SpecItem; // выбираем нужный объект
                isIn = true;
            }
            return isIn;
        }
        private async Task SpecProcess()
        {
            SpecMode = true;
            SiteDTO specifiedSite = null;
            EntityType specifiedType = new EntityType();
            SpecItem = null;
            SpecHierarchy = new List<ItemBase>();
            try
            {
                Behavior.TryLock();
                if (!IsSpecItemInTree())
                {
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

                        specifiedType = SpecifiedType();
                        if (SelectedSite != specifiedSite || SelectedType != specifiedType)
                        {
                            SelectedSite = specifiedSite;
                            SelectedType = specifiedType;
                            await RefreshItems();
                        }
                        else NotFoundInfo();
                    }
                }
            }
            catch
            {
                NotFoundInfo();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
        #endregion
        public AstraViewModel()
        {
            IsReadOnly = !Authorization2.Inst.IsEditable(LinkType.Astra);
            RefreshCommand = new DelegateCommand(Refresh);
            _selectedDate = SeriesHelper.GetCurrentSession();
            RunCommand = new DelegateCommand(Run, () => SelectedDate.HasValue && SelectedTask?.ExchangeTypeId == ExchangeType.Export);
            SaveCommand = new DelegateCommand(SaveAs, () => SelectedDate.HasValue);
            EditTaskCommand = new DelegateCommand(EditTask, () => SelectedTask != null);
            CheckXslCommand = new DelegateCommand(CheckXsl, () => SelectedTask?.ExchangeTypeId == ExchangeType.Import);
            FindByExtIdCommand = new DelegateCommand(FindByExtId);
            EntityByExtId = new Dictionary<string, ExchangeEntityDTO>();
            ItemByExtId = new Dictionary<string, ItemBase>();
            AncestorsOfItem = new Dictionary<ItemBase, List<ItemBase>>();
            SpecMode = false;
            SpecItem = null;
            SpecHierarchy = new List<ItemBase>();
            Init();
        }

        public DelegateCommand RefreshCommand { get; }


        public DelegateCommand EditTaskCommand { get; set; }

        public DelegateCommand RunCommand { get; set; }

        public DelegateCommand SaveCommand { get; set; }

        public DelegateCommand CheckXslCommand { get; set; }

        public bool IsReadOnly { get; set; }

        private void CheckXsl()
        {
            var vm = new CheckXslViewModel(SelectedTask);
            var v = new CheckXslView() { DataContext = vm };
            v.ShowDialog();
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
                    if (!SpecMode) Refresh();
            }
        }

        public IEnumerable<EntityType> TypeList
        {
            get
            {
                yield return EntityType.CompShop;
                yield return EntityType.DistrStation;
                yield return EntityType.MeasStation;
                yield return EntityType.ReducingStation;
                yield return EntityType.Valve;
            }
        }

        public EntityType SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (SetProperty(ref _selectedType, value))
                    if (!SpecMode) Refresh();
            }
        }

        private ItemBase _selectedItem;

        public ItemBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
            }
        }

        public List<ItemBase> Items { get; set; }

        public IEnumerable<PeriodType> PeriodList
        {
            get
            {
                yield return PeriodType.Twohours;
                yield return PeriodType.Day;
            }
        }

        //public PeriodType SelectedPeriod
        //{
        //    get { return _selectedPeriod; }
        //    set
        //    {
        //        if (SetProperty(ref _selectedPeriod, value))
        //        {
        //            OnPropertyChanged(() => IsDaySelected);

        //            if (value == PeriodType.Twohours)
        //                SelectedDate = SeriesHelper.GetCurrentSession();
        //            else
        //                SelectedDate = SeriesHelper.GetCurrentDispDay();
        //        }
        //    }
        //}

        public bool IsDaySelected => SelectedTask?.PeriodTypeId == PeriodType.Day;

        /// <summary>
        ///     Выбранная дата.
        ///     Должна быть задана всегда, NULL недопустим.
        ///     Поэтому первым делом инициализируется в конструкторе.
        /// </summary>
        public DateTime? SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }

        public List<ExchangeTaskDTO> TaskList { get; set; }

        public ExchangeTaskDTO SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                if (SetProperty(ref _selectedTask, value))
                {
                    if (_selectedTask == null) return;
                    OnPropertyChanged(() => SelectedTask);
                    OnPropertyChanged(() => IsExport);
                    OnPropertyChanged(() => IsDaySelected);
                    EditTaskCommand.RaiseCanExecuteChanged();
                    CheckXslCommand.RaiseCanExecuteChanged();
                    RunCommand.RaiseCanExecuteChanged();
                    Refresh();
                }
            }
        }

        public List<ExchangeTypeDTO> ExchangeTypeList
        {
            get { return _exchangTypeList; }
            set { SetProperty(ref _exchangTypeList, value); }
        }

        public ExchangeTypeDTO SelectedExchangeType
        {
            get { return _selectedExchangeType; }
            set
            {
                if (SetProperty(ref _selectedExchangeType, value)) RefreshTasks();
            }
        }
        public bool IsExport => SelectedTask?.ExchangeTypeId == ExchangeType.Export;

        private void EditTask()
        {
            var vm = new AddEditExchangeTaskViewModel(id => RefreshTasks(), SelectedTask, changableExchangeType: false);
            var v = new AddEditExchangeTaskView {DataContext = vm};
            v.ShowDialog();
        }

        private async void RefreshTasks()
        {
            TaskList = await new DataExchangeServiceProxy().GetExchangeTaskListAsync(
                new GetExchangeTaskListParameterSet
                {
                    SourceId = AstraDataSourceId,
                    ExchangeTypeId = (ExchangeType?) SelectedExchangeType.Id
                }
            );

            var id = SelectedTask?.Id;

            OnPropertyChanged(() => TaskList);

            if (SelectedTask == null && id.HasValue)
                SelectedTask = TaskList.FirstOrDefault(t => t.Id == (int)id);
            if (SelectedTask == null && !id.HasValue)
                SelectedTask = TaskList.FirstOrDefault();
        }

        public async void Init()
        {
            // получить список ЛПУ
            if (UserProfile.Current.Site.IsEnterprise)
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                    new GetSiteListParameterSet
                    {
                        //EnterpriseId = UserProfile.Current.Site.Id
                    });
            OnPropertyChanged(() => SiteList);

            ExchangeTypeList = ClientCache.DictionaryRepository.ExchangeTypes;
            _selectedExchangeType = ExchangeTypeList.First();
            OnPropertyChanged(() => SelectedExchangeType);


            _selectedSite = SiteList.First();
            OnPropertyChanged(() => SelectedSite);

            _selectedType = EntityType.CompShop;
            OnPropertyChanged(() => SelectedType);

            PropertyChanged += CheckPropertyChanged;

            RefreshTasks();
        }

        private void CheckPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedExchangeType") _selectedTask = null;
        }

        private void SaveAs()
        {
            //HtmlPage.Window.Navigate(UriBuilder.GetSpecificExchangeHandlerUri(SelectedTask.Id, ((DateTime) SelectedDate).ToLocal(), SelectedTask.PeriodTypeId, !SelectedTask.IsTransform));
            SaveAs(SelectedTask.Id, SelectedDate.Value, SelectedTask.PeriodTypeId, !SelectedTask.IsTransform);
        }

        public static void SaveAs(int taskID, DateTime dt, PeriodType periodType, bool XmlOnly)
        {
            HtmlPage.Window.Navigate(UriBuilder.GetSpecificExchangeHandlerUri(taskID, dt.ToLocal(), periodType, XmlOnly));
        }
    
        private async void Run()
        {
            //await new DataExchangeServiceProxy().RunExchangeTaskAsync(new RunExchangeTaskParameterSet
            //{
            //    Id = SelectedTask.Id,
            //    PeriodTypeId = SelectedTask.PeriodTypeId,
            //    TimeStamp = ((DateTime) SelectedDate).ToLocal()
            //});
            await Run(SelectedTask.Id, SelectedDate.Value, SelectedTask.PeriodTypeId);
        }

        public static async Task Run(int taskID, DateTime dt, PeriodType periodType)
        {
            await new DataExchangeServiceProxy().RunExchangeTaskAsync(new RunExchangeTaskParameterSet
            {
                Id = taskID,
                PeriodTypeId = periodType,
                TimeStamp = dt.ToLocal()
            });
        }

        private async void Refresh()
        {
            if (SelectedSite == null) return;

            try
            {
                Behavior.TryLock();

                // Получить список объектов для обмена
                _bindings = await new DataExchangeServiceProxy().GetExchangeEntityListAsync(
                    new GetExchangeEntityListParameterSet
                    {
                        ExchangeTaskIdList = { SelectedTask.Id }
                    });

                EntityByExtId.Clear(); // Заполнить словарь 
                foreach (ExchangeEntityDTO _binding in _bindings) // для поиска по идентификатору 
                    if (_binding.ExtId + "" != "") EntityByExtId.Add(_binding.ExtId, _binding); // внешней системы
                await RefreshItems();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private async Task RefreshItems()
        {
            Items = new List<ItemBase>();
            ItemByExtId.Clear();
            AncestorsOfItem.Clear();
            #region КС
            if (_selectedType == EntityType.CompShop)
            {
                var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(SelectedSite.Id);
                foreach (var station in stationTree.CompStations)
                {
                    var stationBinding = _bindings.SingleOrDefault(e => e.EntityId == station.Id) ??
                                         new ExchangeEntityDTO
                                         {
                                             EntityId = station.Id,
                                             EntityName = station.Name,
                                             EntityTypeId = EntityType.CompStation
                                         };
                    var stationItem = new BindableItem(stationBinding, UpdateEntityBinding);
                    if (stationItem.ExtId + "" != "")
                    { try { ItemByExtId.Add(stationItem.ExtId, stationItem); } catch { } }
                    foreach (var shop in stationTree.CompShops.Where(cs => cs.ParentId == station.Id))
                    {
                        var shopBinding = _bindings.SingleOrDefault(e => e.EntityId == shop.Id) ??
                                          new ExchangeEntityDTO
                                          {
                                              EntityId = shop.Id,
                                              EntityName = shop.Name,
                                              EntityTypeId = EntityType.CompShop
                                          };

                        var shopItem = new BindableItem(shopBinding, UpdateEntityBinding);
                        stationItem.Children.Add(shopItem);
                        if (shopItem.ExtId + "" != "")
                        { try { ItemByExtId.Add(shopItem.ExtId, shopItem); AncestorsOfItem.Add(shopItem, new List<ItemBase> { stationItem }); } catch { } }
                        foreach (var unit in stationTree.CompUnits.Where(u => u.ParentId == shop.Id))
                        {
                            var unitBinding = _bindings.SingleOrDefault(e => e.EntityId == unit.Id) ??
                                              new ExchangeEntityDTO
                                              {
                                                  EntityId = unit.Id,
                                                  EntityName = unit.Name,
                                                  EntityTypeId = EntityType.CompUnit
                                              };

                            var unitItem = new BindableItem(unitBinding, UpdateEntityBinding);
                            shopItem.Children.Add(unitItem);
                            if (unitItem.ExtId + "" != "") { try { ItemByExtId.Add(unitItem.ExtId, unitItem); AncestorsOfItem.Add(unitItem, new List<ItemBase> { stationItem, shopItem }); } catch { } }
                        }
                    }

                    // СОГи

                    foreach (var cs in stationTree.CoolingStations.Where(cs => cs.ParentId == station.Id))
                    {
                        var csBinding = _bindings.SingleOrDefault(e => e.EntityId == cs.Id) ??
                                        new ExchangeEntityDTO
                                        {
                                            EntityId = cs.Id,
                                            EntityName = cs.Name,
                                            EntityTypeId = cs.EntityType
                                        };

                        var csItem = new BindableItem(csBinding, UpdateEntityBinding);
                        stationItem.Children.Add(csItem);
                        if (csItem.ExtId + "" != "") { try { ItemByExtId.Add(csItem.ExtId, csItem); AncestorsOfItem.Add(csItem, new List<ItemBase> { stationItem }); } catch { } }

                        foreach (var cu in stationTree.CoolingUnits.Where(cu => cu.ParentId == cs.Id))
                        {
                            var cuBinding = _bindings.SingleOrDefault(e => e.EntityId == cu.Id) ??
                                            new ExchangeEntityDTO
                                            {
                                                EntityId = cu.Id,
                                                EntityName = cu.Name,
                                                EntityTypeId = cu.EntityType
                                            };

                            var cuItem = new BindableItem(cuBinding, UpdateEntityBinding);
                            csItem.Children.Add(cuItem);
                            if (cuItem.ExtId + "" != "") { try { ItemByExtId.Add(cuItem.ExtId, cuItem); AncestorsOfItem.Add(cuItem, new List<ItemBase> { stationItem, csItem }); } catch { } }

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
                    var dsBinding = _bindings.SingleOrDefault(e => e.EntityId == ds.Id) ??
                                    new ExchangeEntityDTO
                                    {
                                        EntityId = ds.Id,
                                        EntityName = ds.Name,
                                        EntityTypeId = EntityType.DistrStation
                                    };

                    var dsItem = new BindableItem(dsBinding, UpdateEntityBinding);
                    Items.Add(dsItem);
                    if (dsItem.ExtId + "" != "") { try { ItemByExtId.Add(dsItem.ExtId, dsItem); } catch { } }
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
                    var msItem = new GroupItem(ms.Name);
                    foreach (var ml in measTree.MeasLines.Where(m => m.ParentId == ms.Id))
                    {
                        var mlBinding = _bindings.SingleOrDefault(e => e.EntityId == ml.Id) ??
                                        new ExchangeEntityDTO
                                        {
                                            EntityId = ml.Id,
                                            EntityName = ml.Name,
                                            EntityTypeId = EntityType.MeasLine
                                        };

                        var mlItem = new BindableItem(mlBinding, UpdateEntityBinding);
                        msItem.Children.Add(mlItem);
                        if (mlItem.ExtId + "" != "") { try { ItemByExtId.Add(mlItem.ExtId, mlItem); AncestorsOfItem.Add(mlItem, new List<ItemBase> { msItem }); } catch { } }
                    }
                    Items.Add(msItem);
                }
            }
            #endregion
            #region ПРГ
            if (_selectedType == EntityType.ReducingStation)
            {
                var stations = await new ObjectModelServiceProxy().GetReducingStationListAsync(
                    new GetReducingStationListParameterSet
                    {
                        SiteId = SelectedSite.Id
                    });
                foreach (var rs in stations)
                {
                    var rsBinding = _bindings.SingleOrDefault(e => e.EntityId == rs.Id) ??
                                    new ExchangeEntityDTO
                                    {
                                        EntityId = rs.Id,
                                        EntityName = rs.Name,
                                        EntityTypeId = EntityType.ReducingStation
                                    };

                    var rsItem = new BindableItem(rsBinding, UpdateEntityBinding);
                    Items.Add(rsItem);
                    if (rsItem.ExtId + "" != "") { try { ItemByExtId.Add(rsItem.ExtId, rsItem); } catch { } }
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
                        var pipeBinding = _bindings.SingleOrDefault(e => e.EntityId == pipe.Id) ??
                                          new ExchangeEntityDTO
                                          {
                                              EntityId = pipe.Id,
                                              EntityName = pipe.Name,
                                              EntityTypeId = EntityType.Pipeline
                                          };

                        var pipeItem = new BindableItem(pipeBinding, UpdateEntityBinding) { IsExpanded = false };

                        typeItem.Children.Add(pipeItem);

                        if (pipeItem.ExtId + "" != "") { try { ItemByExtId.Add(pipeItem.ExtId, pipeItem); AncestorsOfItem.Add(pipeItem, new List<ItemBase> { typeItem }); } catch { } }

                        foreach (var valve in pipeTree.LinearValves.Where(v => v.ParentId == pipe.Id))
                        {
                            var valveBinding = _bindings.SingleOrDefault(e => e.EntityId == valve.Id) ??
                                               new ExchangeEntityDTO
                                               {
                                                   EntityId = valve.Id,
                                                   EntityName = valve.Name,
                                                   EntityTypeId = EntityType.Valve
                                               };

                            var valveItem = new BindableItem(valveBinding, UpdateEntityBinding);

                            pipeItem.Children.Add(valveItem);

                            if (valveItem.ExtId + "" != "") { try { ItemByExtId.Add(valveItem.ExtId, valveItem); AncestorsOfItem.Add(valveItem, new List<ItemBase> { typeItem, pipeItem }); } catch { } }

                        }
                    }
                    if (typeItem.Children.Any())
                        Items.Add(typeItem);
                }
            }
            #endregion
            OnPropertyChanged(() => Items);
        }

        public bool UpdateEntityBinding(ExchangeEntityDTO dto)
        {
            if (_bindings.Any(b => !string.IsNullOrEmpty(b.ExtId) && b.ExtId == dto.ExtId &&
                                   b.EntityId != dto.EntityId))
            {
                var bndng = _bindings.Single(b => !string.IsNullOrEmpty(b.ExtId) && b.ExtId == dto.ExtId &&
                                                  b.EntityId != dto.EntityId);
                MessageBoxProvider.Alert(
                    $"Невозможно присвоить идентификатор, т.к. он уже присвоен другому объекту: {Environment.NewLine}{Environment.NewLine}{bndng.EntityPath}",
                    "Недопустимый идентификатор");

                return false;
            }


            new DataExchangeServiceProxy().SetExchangeEntityAsync(
                new AddEditExchangeEntityParameterSet
                {
                    EntityId = dto.EntityId,
                    ExchangeTaskId = SelectedTask.Id,
                    ExtId = dto.ExtId,
                    IsActive = dto.IsActive
                });


            return true;
        }

    }

    public class ItemBase : PropertyChangedBase
    {
        public ItemBase()
        {
            Children = new List<ItemBase>();
            IsExpanded = true;
        }

        public List<ItemBase> Children { get; set; }

        public virtual string Name { get; set; }

        public bool IsExpanded { get; set; }
    }

    public class GroupItem : ItemBase
    {
        private readonly string _name;

        public GroupItem(string name)
        {
            _name = name;
        }

        public override string Name => _name;
    }

    public class BindableItem : ItemBase
    {
        private readonly ExchangeEntityDTO _dto;
        private readonly Func<ExchangeEntityDTO, bool> _saveAction;

        public BindableItem(ExchangeEntityDTO dto, Func<ExchangeEntityDTO, bool> saveAction)
        {
            _dto = dto;
            _saveAction = saveAction;

            IsExpanded = false; //true;
        }

        public override string Name => _dto.EntityName;

        public string ExtId
        {
            get { return _dto.ExtId; }
            set
            {
                if (_dto.ExtId != value)
                {
                    var oldVal = _dto.ExtId;

                    if (string.IsNullOrEmpty(_dto.ExtId))
                        _dto.IsActive = true;
                    if (string.IsNullOrEmpty(value))
                        _dto.IsActive = false;

                    _dto.ExtId = value;
                    if (!_saveAction(_dto))
                        _dto.ExtId = oldVal;

                    OnPropertyChanged(() => ExtId);
                    OnPropertyChanged(() => IsActive);
                    OnPropertyChanged(() => IsActiveEnabled);
                }
            }
        }

        public bool IsActive
        {
            get { return _dto.IsActive; }
            set
            {
                if (_dto.IsActive != value)
                {
                    _dto.IsActive = value;
                    OnPropertyChanged(() => IsActive);
                    _saveAction(_dto);
                }
            }
        }

        public bool IsActiveEnabled => !string.IsNullOrEmpty(_dto.ExtId);


        public EntityType EntityType => _dto.EntityTypeId;
    }
}