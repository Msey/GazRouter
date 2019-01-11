using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Balances.Common.TreeGroupType;
using GazRouter.Balances.DayBalance.EditAggregators;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs.EntityPicker;
using GazRouter.Controls.InputStory;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Balances.BalanceGroups;
using GazRouter.DTO.Balances.DayBalance;
using GazRouter.DTO.Balances.MiscTab;
using GazRouter.DTO.Dictionaries.AggregatorTypes;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SeriesData.ValueMessages;
using Microsoft.Practices.ObjectBuilder2;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;


namespace GazRouter.Balances.DayBalance
{
    public class DayBalanceViewModel : LockableViewModel
    {
        private DateTime _selectedDate;
        private GasTransportSystemDTO _selectedSystem;
        private readonly bool _editPermission;
        

        public DayBalanceViewModel()
        {
            _editPermission = Authorization2.Inst.IsEditable(LinkType.DayBalance);

            InputStoryViewModel = new InputStoryViewModel();
            
            _selectedDate = SeriesHelper.GetPastDispDay();
            _selectedSystem = SystemList.FirstOrDefault();
            
            RefreshCommand = new DelegateCommand(LoadData);

            CalcSerieCommand = new DelegateCommand(CalcSerie, () => _editPermission);
            EditGasSupplyCommand = new DelegateCommand(EditGasSupply, () => _editPermission);
            EditBalanceLossCommand = new DelegateCommand(EditBalanceLoss, () => _editPermission);
            CalcBalanceLossCommand = new DelegateCommand(CalcBalanceLoss, () => _editPermission);

            AddMiscObjectCommand = new DelegateCommand(AddMiscObject, () => _editPermission);
            DeleteMiscObjectCommand = new DelegateCommand(DeleteMiscObject, () => _editPermission);

            ExpandTabCommand = new DelegateCommand(ExpandTab);
            CollapseTabCommand = new DelegateCommand(CollapseTab);

            LoadData();
        }

        public bool IsEnterprise => UserProfile.Current.Site.IsEnterprise;

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if(SetProperty(ref _selectedDate, value.ToLocal()))
                    LoadData();
            }
        }

        public List<GasTransportSystemDTO> SystemList => ClientCache.DictionaryRepository.GasTransportSystems;

        public GasTransportSystemDTO SelectedSystem
        {
            get { return _selectedSystem; }
            set
            {
                if(SetProperty(ref _selectedSystem, value))
                    LoadData();
            }
        }

        
        public DelegateCommand RefreshCommand { get; set; }


        #region СПИСОК БАЛАНСОВЫХ ГРУПП

        public List<BalanceGroupDTO> GroupList => _data?.DataDto.BalanceGroups;

        private BalanceGroupDTO _selectedGroup;

        public BalanceGroupDTO SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                if (SetProperty(ref _selectedGroup, value))
                {
                    Balance = new Balance(_data, _selectedGroup?.Id);
                    BuildTrees();
                }
            }
        }

        #endregion



        #region ТИП ГРУППИРОВКИ ГИСов
        /// <summary>
        /// Тип группировки ГИСов для вкладок поступление, транзит
        /// </summary>
        public TreeGroupType SelectedMeasStationsTreeGroupType
        {
            get { return BalanceGroupTypes.MeasStationGroupType; }
            set
            {
                if (BalanceGroupTypes.MeasStationGroupType != value)
                {
                    BalanceGroupTypes.MeasStationGroupType = value;
                    OnPropertyChanged(() => SelectedMeasStationsTreeGroupType);
                    BuildTrees();
                }
            }
        }
        #endregion


        #region ТИП ГРУППИРОВКИ ГРС
        /// <summary>
        /// Тип группировки ГРС для вкладки Потребители
        /// </summary>

        public TreeGroupType SelectedDistrStationsTreeGroupType
        {
            get { return BalanceGroupTypes.DistrStationGroupType; }
            set
            {
                if (BalanceGroupTypes.DistrStationGroupType != value)
                {
                    BalanceGroupTypes.DistrStationGroupType = value;
                    OnPropertyChanged(() => SelectedDistrStationsTreeGroupType);
                    BuildTrees();
                }
            }
        }
        #endregion

        #region ТИП ГРУППИРОВКИ ПЭН
        /// <summary>
        /// Тип группировки ПЭН
        /// </summary>

        public TreeGroupType SelectedOperConsumersTreeGroupType
        {
            get { return BalanceGroupTypes.OperConsumerGroupType; }
            set
            {
                if (BalanceGroupTypes.OperConsumerGroupType != value)
                {
                    BalanceGroupTypes.OperConsumerGroupType = value;
                    OnPropertyChanged(() => SelectedOperConsumersTreeGroupType);
                    BuildTrees();
                }
            }
        }
        #endregion


        #region SORT ORDER

        private bool _showSortOrder;
        public bool ShowSortOrder
        {
            get { return _showSortOrder; }
            set
            {
                if (SetProperty(ref _showSortOrder, value))
                    ShowHideSortOrderColumns(value);
            } 
        }

        private void ShowHideSortOrderColumns(bool isVisible)
        {
            Tabs.ForEach(i => i.ShowSortOrder = isVisible);
        }

        #endregion

        #region EXPAND|COLLAPSE
        public DelegateCommand ExpandTabCommand { get; set; }

        private void ExpandTab()
        {
            SelectedTab?.Expand();
        }

        public DelegateCommand CollapseTabCommand { get; set; }

        private void CollapseTab()
        {
            SelectedTab?.Collapse();
        }

        #endregion

       


        public Balance Balance { get; set; }


        private BalanceData _data;
        private async void LoadData()
        {
            Lock();

            var grId = SelectedGroup?.Id;

            _data = new BalanceData(
                await new BalancesServiceProxy().GetDayBalanceDataAsync(
                    new GetDayBalanceDataParameterSet
                    {
                        Day = SelectedDate.Day,
                        Month = SelectedDate.Month,
                        Year = SelectedDate.Year,
                        SystemId = SelectedSystem.Id
                    }), 
                SelectedSystem, 
                SelectedDate);

            
            OnPropertyChanged(() => GroupList);
            LoadMiscObjects(_data);

            _selectedGroup = GroupList.SingleOrDefault(g => g.Id == grId);
            OnPropertyChanged(() => SelectedGroup);

            Balance = new Balance(_data, _selectedGroup?.Id);
            BuildTrees();

            Unlock();
        }

        private void BuildTrees()
        {
            Balance.Build();
            // Формирование вкладки справочно
            
            OnPropertyChanged(() => Tabs);
            SelectedTab = _curTabIndex < 0 ? Tabs.FirstOrDefault() : Tabs.ToList()[_curTabIndex];

            Tabs.ForEach(t => t.ShowSortOrder = ShowSortOrder);
        }

        
        #region TABS

        public IEnumerable<TableViewModel> Tabs
        {
            get
            {
                if (Balance != null)
                {
                    yield return Balance.Summary;
                    yield return Balance.Intake;
                    yield return Balance.Transit;
                    yield return Balance.Consumers;
                    yield return Balance.AuxCosts;
                    yield return Balance.OperConsumers;
                    yield return MiscObjects;
                }

            }
        }


        private int _curTabIndex; // для сохранения выбранной вкладки после обновления
        private TableViewModel _selectedTab;
        public TableViewModel SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                if (SetProperty(ref _selectedTab, value))
                {
                    OnPropertyChanged(() => IsMiscTabSelected);
                    if (_selectedTab != null) _curTabIndex = Tabs.ToList().IndexOf(SelectedTab);
                }
            }
        }

        #endregion


        #region SUMMARY

        public TableViewModel Summary => Balance?.Summary;
        
        #endregion


        public InputStoryViewModel InputStoryViewModel { get; set; }



        #region MISC OBJECTS
        public TableViewModel MiscObjects { get; set; }


        private void LoadMiscObjects(BalanceData data)
        {
            MiscObjects = new TableViewModel("Справочно", SelectedDate);
            foreach (var e in data.GetMiscObject())
            {
                //var miscItem = new ItemBase(e, SelectedDate) {BalItem = BalanceItem.MiscObjects};
                var miscItem = new ItemBase(e, SelectedDate);
                data.GetFactValue(miscItem);
                MiscObjects.Childs.Add(miscItem);
            }
            OnPropertyChanged(() => MiscObjects);
        }


        public bool IsMiscTabSelected => SelectedTab == MiscObjects;
        

        public DelegateCommand AddMiscObjectCommand { get; }

        private EntityPickerDialogViewModel _picker;

        private void AddMiscObject()
        {
            var allowedTypes = new List<EntityType>
            {
                EntityType.Aggregator,
                EntityType.Consumer,
                EntityType.DistrStation,
                EntityType.DistrStationOutlet,
                EntityType.MeasLine,
                EntityType.MeasStation,
                EntityType.ReducingStation
            };

            _picker = new EntityPickerDialogViewModel(async () =>
            {
                if (_picker.SelectedItem != null)
                {
                    await new BalancesServiceProxy().AddMiscTabEntityAsync(
                        new AddRemoveMiscTabEntityParameterSet
                        {
                            EntityId = _picker.SelectedItem.Id,
                            SystemId = SelectedSystem.Id,
                            BalGroupId = SelectedGroup?.Id
                        });
                    LoadData();
                }
            }, allowedTypes);
            var v = new EntityPickerDialogView { DataContext = _picker };
            v.ShowDialog();
        }


        public DelegateCommand DeleteMiscObjectCommand { get; }

        private async void DeleteMiscObject()
        {
            if (MiscObjects?.SelectedItem != null)
            {
                await new BalancesServiceProxy().RemoveMiscTabEntityAsync(
                    new AddRemoveMiscTabEntityParameterSet
                    {
                        EntityId = MiscObjects.SelectedItem.Entity.Id,
                        SystemId = SelectedSystem.Id,
                        BalGroupId = SelectedGroup?.Id
                    });
                LoadData();
            }
        }

        #endregion



        #region ПЕРЕСЧЕТ СЕРИИ

        public DelegateCommand CalcSerieCommand { get; set; }

        private async void CalcSerie()
        {
            Lock();
            var serie = await new SeriesDataServiceProxy().AddSerieAsync(
                new AddSeriesParameterSet
                {
                    Day = _selectedDate.Day,
                    Month = _selectedDate.Month,
                    Year = _selectedDate.Year,
                    PeriodTypeId = PeriodType.Day
                });

            var pSet = new PerformCheckingParameterSet
            {
                SerieId = serie.Id
            };

            await new SeriesDataServiceProxy().PerformCheckingAsync(
                new List<PerformCheckingParameterSet> {pSet});

            Unlock();

            LoadData();
        }

        #endregion


        #region ВВОД ЗАПАСА ГАЗА

        public DelegateCommand EditGasSupplyCommand { get; set; }

        private void EditGasSupply()
        {
            var vm = new EditAggregatorsViewModel(_selectedSystem.Id, _selectedDate, AggregatorType.GasSupply, LoadData);
            var v = new EditAggregatorsView { DataContext = vm};
            v.ShowDialog();
        }

        #endregion


        #region ВВОД НЕБАЛАНСА (ПОТЕРЬ)

        public DelegateCommand EditBalanceLossCommand { get; set; }

        private void EditBalanceLoss()
        {
            var vm = new EditAggregatorsViewModel(_selectedSystem.Id, _selectedDate, AggregatorType.BalanceLoss, LoadData);
            var v = new EditAggregatorsView { DataContext = vm };
            v.ShowDialog();
        }

        #endregion


        #region РАСЧЕТ НЕБАЛАНСА (ПОТЕРЬ)

        public DelegateCommand CalcBalanceLossCommand { get; set; }

        private async void CalcBalanceLoss()
        {
            var pSets = new List<SetPropertyValueParameterSet>();
            var aggrList = _data.DataDto.Aggregators.Where(a => a.AggregatorType == AggregatorType.BalanceLoss).ToList();
            var grList = new List<BalanceGroupDTO>();
            grList.AddRange(_data.DataDto.BalanceGroups);
            grList.Add(null);
            foreach (var gr in grList)
            {
                var aggr = aggrList.SingleOrDefault(a => a.BalanceGroupId == gr?.Id);
                if (aggr == null) continue;

                var balance = new Balance(_data, gr?.Id);
                balance.Build();
                balance.BalanceLoss.Current = 0; //Обнуляем, чтобы не учитывался при расчете

                if (balance.Summary.Current > 0)
                    pSets.Add(
                        new SetPropertyValueParameterSet
                        {
                            EntityId = aggr.Id,
                            PropertyTypeId = PropertyType.Flow,
                            SeriesId = _data.DataDto.Serie.Id,
                            Value = balance.Summary.Current ?? 0
                        });
            }

            await new SeriesDataServiceProxy().SetPropertyValueAsync(pSets);

            LoadData();
        }

        #endregion


    }


    
    

   
}