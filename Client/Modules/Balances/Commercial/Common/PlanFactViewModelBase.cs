using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using GazRouter.Balances.Commercial.BalanceDiagram;
using GazRouter.Balances.Commercial.Dialogs.ClearValues;
using GazRouter.Balances.Commercial.Dialogs.SelectOwner;
using GazRouter.Balances.Commercial.Dialogs.ShowHideOwners;
using GazRouter.Balances.Commercial.OwnersSummary;
using GazRouter.Balances.Commercial.Summary;
using GazRouter.Balances.Commercial.Transport;
using GazRouter.Balances.Common.TreeGroupType;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.MonthAlgorithms;
using GazRouter.DTO.Balances.Transport;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Dictionaries.Targets;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using ShowHideOwnersView = GazRouter.Balances.Commercial.Dialogs.ShowHideOwners.ShowHideOwnersView;

namespace GazRouter.Balances.Commercial.Common
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class PlanFactViewModelBase : LockableViewModel
    {
        protected BalanceDataBase _data;
        protected BalanceTreeBuilder _treeBuilder;
        protected bool _hasChanges;
        protected bool _isEditPermission;
        protected readonly Target _target;
        protected ItemActions _itemActions;


        public PlanFactViewModelBase(Target target)
        {
            _target = target;
            _isEditPermission = Authorization2.Inst.IsEditable(_target == Target.Plan ? LinkType.Plan : LinkType.MonthBalance);

            RefreshCommand = new DelegateCommand(Load);
            SaveCommand = new DelegateCommand(Save, () => _hasChanges && _isEditPermission && !IsFinal);
            CalculateTransportCommand = new DelegateCommand(CalculateTransport, () => _isEditPermission && !IsFinal);
            ClearTransportCommand = new DelegateCommand(ClearTransport, () => _isEditPermission && !IsFinal);
            ClearValuesCommand = new DelegateCommand(ClearValues, () => _isEditPermission && !IsFinal);
            LoadAuxCostsCommand = new DelegateCommand(LoadAuxCosts, () => _isEditPermission && !IsFinal);
            FromFinalCommand = new DelegateCommand(FromFinal, () => _isEditPermission && !IsFinal);
            ToFinalCommand = new DelegateCommand(ToFinal, () => _isEditPermission && !IsFinal);


            _selectedDate = DateTime.Now.MonthStart();
            _selectedSystem = SystemList.First();
            _versionNum = 0;
            _selectedUnitType = IsolatedStorageManager.Get<int?>("VolumeInputUnits") ?? 1;
            

            Intake = new TableViewModel(_target, BalanceItem.Intake, x => { SelectedItem = x; }, RefreshSummary);
            Transit = new TableViewModel(_target, BalanceItem.Transit, x => { SelectedItem = x; }, RefreshSummary);
            Consumers = new TableViewModel(_target, BalanceItem.Consumers, x => { SelectedItem = x; }, RefreshSummary);
            AuxCosts = new TableViewModel(_target, BalanceItem.AuxCosts, x => { SelectedItem = x; }, RefreshSummary);
            OperConsumers = new TableViewModel(_target, BalanceItem.OperConsumers, x => { SelectedItem = x; }, RefreshSummary);
            GasSupply = new TableViewModel(_target, BalanceItem.GasSupply, x => { SelectedItem = x; }, RefreshSummary);
            BalanceLoss = new TableViewModel(_target, BalanceItem.BalanceLoss, x => { SelectedItem = x; }, RefreshSummary);
            Transport = new TableViewModel(_target, BalanceItem.Transport, null, null) { ItemColumnName = "ТТР", UnitsName = "млрд.м3/км"};
            CalculatedTransport = new TransportViewModel();


            BalanceDiagram = new BalanceDiagramViewModel();
            SummaryViewModel = new BalanceSummaryViewModel(_target);
            OwnersSummary = new OwnersSummaryViewModel();

            _itemActions = new ItemActions { ShowHideOwnerAction = ShowHideOwners };
        }

        


        public DelegateCommand RefreshCommand { get; set; }


        #region ВЫБОР ДАТЫ

        private DateTime _selectedDate;

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if (SetProperty(ref _selectedDate, value))
                    Load();
            }
        }
        #endregion

        
        #region ВЫБОР ГТС

        /// <summary>
        /// Список ГТС
        /// </summary>
        public List<GasTransportSystemDTO> SystemList => ClientCache.DictionaryRepository.GasTransportSystems;


        private GasTransportSystemDTO _selectedSystem;

        /// <summary>
        /// Выбранная ГТС
        /// </summary>
        public GasTransportSystemDTO SelectedSystem
        {
            get { return _selectedSystem; }
            set
            {
                if (SetProperty(ref _selectedSystem, value))
                    Load();
            }
        }

        /// <summary>
        /// Если в ДООО только одна ГТС, то и не нужно показывать элемент выбора ГТС
        /// </summary>
        public bool IsSystemSelectorVisible => SystemList.Count > 1;

        #endregion


        #region ВЫБОР ВЕРСИИ

        private int _versionNum;

        public int VersionNum
        {
            get { return _versionNum; }
            set
            {
                if (SetProperty(ref _versionNum, value))
                {
                    OnPropertyChanged(() => IsFinal);
                    Load();
                }
            }
        }

        public bool IsFinal => _versionNum == 1;

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
                    _treeBuilder.RefreshTrees(_selectedOwner?.Id);
                    Intake.AddTree(_treeBuilder.Intake);
                    Transit.AddTree(_treeBuilder.Transit);
                    GasSupply.AddTree(_treeBuilder.GasSupply);
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
                    _treeBuilder.RefreshTrees(_selectedOwner?.Id);
                    Consumers.AddTree(_treeBuilder.Consumers);
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
                    _treeBuilder.RefreshTrees(_selectedOwner?.Id);
                    OperConsumers.AddTree(_treeBuilder.OperConsumers);
                }
            }
        }
        #endregion


        #region РАЗМЕРНОСТЬ ВЕЛИЧИН

        private int _selectedUnitType;
        public int SelectedUnitType
        {
            get { return _selectedUnitType; }
            set
            {
                if (SetProperty(ref _selectedUnitType, value))
                {
                    IsolatedStorageManager.Set("VolumeInputUnits", value);
                    Load();
                }
            }
        }

        private double Coef => _selectedUnitType == 0 ? 1.0 : 1000.0;

        private string VolumeFormat => _selectedUnitType == 0 ? "#,0.000" : "#,0";

        #endregion


        #region ФИЛЬТР ПО ПОСТАВЩИКАМ

        public IEnumerable<GasOwnerDTO> OwnerList => _data?.Owners.Where(o => o.SystemList.Any(s => s == SelectedSystem?.Id));


        private GasOwnerDTO _selectedOwner;
        public GasOwnerDTO SelectedOwner
        {
            get { return _selectedOwner; }
            set
            {
                if (SetProperty(ref _selectedOwner, value))
                {
                    RefreshTrees();
                    RefreshSummary(null);
                }
            }
        }

        #endregion


        // Поступление
        public TableViewModel Intake { get; set; }

        // Транзит
        public TableViewModel Transit { get; set; }

        // Потребители
        public TableViewModel Consumers { get; set; }

        // СТН
        public TableViewModel AuxCosts { get; set; }

        // БАЛАНСОВЫЕ ПОТЕРИ
        public TableViewModel BalanceLoss { get; set; }

        // ПЭН
        public TableViewModel OperConsumers { get; set; }

        // Запас газа
        public TableViewModel GasSupply { get; set; }

        // ТТР (ввод)
        public TableViewModel Transport { get; set; }


        public IEnumerable<TableViewModel> TabList
        {
            get
            {
                yield return Intake;
                yield return Transit;
                yield return Consumers;
                yield return AuxCosts;
                yield return BalanceLoss;
                yield return OperConsumers;
                yield return GasSupply;
                yield return Transport;
            }
        }


        #region LOAD
        

        protected async void Load()
        {
            if (SelectedSystem == null) return;
            
            Lock();

            _data = await BalanceDataBase.GetData(SelectedSystem, SelectedDate, _target, IsFinal);
            OnPropertyChanged(() => OwnerList);

            TabList.ForEach(t => t.ValueFormat = VolumeFormat);

            _treeBuilder = new BalanceTreeBuilder(_target, _data, Coef, _itemActions);
            RefreshTrees();
            RefreshSummary(null);

            _hasChanges = false;
            UpdateCommands();

            CalculatedTransport.LoadData(_target, _data, Coef);

            OnLoadComplete();
            
            Unlock();
        }

        protected virtual void UpdateCommands()
        {
            SaveCommand.RaiseCanExecuteChanged();
            CalculateTransportCommand.RaiseCanExecuteChanged();
            ClearValuesCommand.RaiseCanExecuteChanged();
            LoadAuxCostsCommand.RaiseCanExecuteChanged();

            var isReadOnly = !_isEditPermission || IsFinal;

            TabList.ForEach(t => t.IsReadOnly = isReadOnly);
        }


        protected virtual void OnLoadComplete()
        {
            
        }


        protected void RefreshTrees()
        {
            _treeBuilder.RefreshTrees(_selectedOwner?.Id);

            Intake.AddTree(_treeBuilder.Intake);
            Transit.AddTree(_treeBuilder.Transit);
            Consumers.AddTree(_treeBuilder.Consumers);
            AuxCosts.AddTree(_treeBuilder.AuxCosts);
            OperConsumers.AddTree(_treeBuilder.OperConsumers);
            GasSupply.AddTree(_treeBuilder.GasSupply);
            BalanceLoss.AddTree(_treeBuilder.BalanceLoss);
            Transport.AddTree(_treeBuilder.Transport);
        }
        
        #endregion


        #region SAVE

        public DelegateCommand SaveCommand { get; set; }

        /// <summary>
        /// Сохранение значений плана
        /// </summary>
        private async void Save()
        {
            var contractId = _target == Target.Plan ? _data.PlanContract.Id : _data.FactContract.Id;
            
            var values = new SaveBalanceValuesParameterSet
            {
                ContractId = contractId,
                ValueList = _treeBuilder.OwnerItems.Select(o => o.GetSetValueParamSet(contractId, Coef)).Where(ps => ps != null).ToList(),
                SwapList = _data.GetAddSwapParameterSets()
            };

            await new BalancesServiceProxy().SaveBalanceValuesAsync(values);

            _hasChanges = false;
            SaveCommand.RaiseCanExecuteChanged();
        }

        // Проверяет наличие несохраненных данных и выдает предупреждение.
        // Возвращает TRUE - если нет несохраненных данных, FALSE - в противном случае.
        protected bool CheckChanges()
        {
            if (!_hasChanges) return true;

            MessageBoxProvider.Alert(
                "Внимание! Сейчас есть несохраненные значения. Перед выполнением данного действия нужно обязательно сделать сохранение.",
                "Несохраненные данные");
            return false;
        }

        #endregion

        


        private ItemBase _selectedItem;

        public ItemBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    OnSelectedItemChanged();
                }
            }
        }

        protected virtual void OnSelectedItemChanged()
        {
            
        }
        


        #region SUMMARY_DIAGRAM

        /// <summary>
        /// Дельта между поступлением и распределением
        /// </summary>
        public double? BalanceDelta => Summary?.BalanceDelta(_target);


        /// <summary>
        /// TRUE - если поступление равно распределению
        /// </summary>
        public bool IsBalanced => BalanceDelta == 0;


        public BalanceDiagramViewModel BalanceDiagram { get; set; }

        public void UpdateDiagram()
        {
            if (Summary == null) return;
            BalanceDiagram.UpdateDiagram(Summary, _target);
        }


        
        public BalanceSummary Summary { get; set; }

        public BalanceSummaryViewModel SummaryViewModel { get; set; }

        private void RefreshSummary(BalanceItem? balItem)
        {
            if (balItem.HasValue)
            {
                Summary.UpdateItem(balItem.Value, _treeBuilder.OwnerItems);
            }
            else
            {
                var owners = SelectedOwner != null
                    ? _data.Owners.Where(o => o == SelectedOwner)
                    : _data.Owners.Where(o => o.SystemList.Any(s => s == SelectedSystem.Id));
                Summary = new BalanceSummary(owners);
                Summary.CreateSummary(_treeBuilder.OwnerItems);
            }

            SummaryViewModel.Update(Summary);
            SummaryViewModel.ValueFormat = VolumeFormat;

            UpdateDiagram();
            RefreshOwnerSummary();

            OnPropertyChanged(() => BalanceDelta);
            OnPropertyChanged(() => IsBalanced);

            _hasChanges = true;
            SaveCommand.RaiseCanExecuteChanged();
        }

        #endregion


        #region OWNERS SUMMARY
        
        public OwnersSummaryViewModel OwnersSummary { get; set; }
        
        private void RefreshOwnerSummary()
        {
            if (_data?.Owners == null) return;
            var owners = _selectedOwner != null
                    ? _data.Owners.Where(o => o == _selectedOwner)
                    : _data.Owners.Where(o => o.SystemList.Any(s => s == SelectedSystem.Id));
            OwnersSummary.UpdateSummary(owners, Summary, _target);
        }

        #endregion



       

        #region SHOW|HIDE OWNES
        // Вызывает форму, через которую, можно настроить видимость поставщиков
        // для определенных точек приема/сдачи газа
        private void ShowHideOwners(Guid entityId, BalanceItem balItem)
        {
            new ShowHideOwnersView
            {
                DataContext = new ShowHideOwnersViewModel(entityId, balItem, SelectedSystem.Id, async() =>
                {
                    _data.OwnerDisables = await new BalancesServiceProxy().GetGasOwnerDisableListAsync();
                    RefreshTrees();
                })
            }.ShowDialog();
        }
        #endregion



        #region РАСЧЕТ ТТР

        /// <summary>
        /// Команда расчета ТТР
        /// </summary>
        public DelegateCommand CalculateTransportCommand { get; set; }

        private async void CalculateTransport()
        {
            if (!CheckChanges()) return;

            Lock();

            await new BalancesServiceProxy().CalculateTransportListAsync(
                new HandleTransportListParameterSet { ContractId = _data.GetContract(_target).Id });
            Unlock();

            Load();
        }


        public DelegateCommand ClearTransportCommand { get; set; }

        private async void ClearTransport()
        {
            if (!CheckChanges()) return;

            Lock();
            await new BalancesServiceProxy().CLearTransportResultsAsync(
                new HandleTransportListParameterSet { ContractId = _data.GetContract(_target).Id });
            Unlock();

            Load();
        }


        /// <summary>
        /// ViewModel для вкладки ТТР (отображаются результаты расчета)
        /// </summary>
        public TransportViewModel CalculatedTransport { get; set; }

        #endregion


        #region УДАЛЕНИЕ ЗНАЧЕНИЙ

        public DelegateCommand ClearValuesCommand { get; set; }
        private void ClearValues()
        {
            var vm = new ClearValuesViewModel(_data.GetContract(_target).Id, null, _data.Owners, Load);
            var v = new ClearValuesView {DataContext = vm};
            v.ShowDialog();
        }

        #endregion


        #region ЗАГРУЗКА СТН

        public DelegateCommand LoadAuxCostsCommand { get; set; }

        private void LoadAuxCosts()
        {
            if (!CheckChanges()) return;


            var vm = new SelectOwnerViewModel(_data.Owners,
                async ownerId =>
                {
                    await new BalancesServiceProxy().MoveAuxCostsAsync(
                        new MoveAuxCostsParameterSet
                        {
                            ContractId = _data.GetContract(_target).Id,
                            OwnerId = ownerId
                        });
                    Load();
                });
            var v = new SelectOwnerView { DataContext = vm };
            v.ShowDialog();
        }

        #endregion


        #region КОПИРОВАНИЕ ВЕРСИЙ

        public DelegateCommand FromFinalCommand { get; set; }

        private void FromFinal()
        {
            MessageBoxProvider.Confirm(
                "Загружаем значения из финальной версии. Все значения в рабочей версии будут заменены. Нужно подтверждение.",
                async response =>
                {
                    if (response)
                    {
                        Lock();
                        await new BalancesServiceProxy().MoveValuesToOtherVersionAsync(
                            new MoveValuesToOtherVersionParameterSet
                            {
                                ContractId = _data.GetContract(_target).Id,
                                ToFinal = false
                            });

                        Unlock();

                        Load();
                    }
                },
                "Загрузка значений из финальной версии",
                "Загрузить",
                "Отмена"
                );
        }

        public DelegateCommand ToFinalCommand { get; set; }
        private void ToFinal()
        {
            if (!CheckChanges()) return;

            MessageBoxProvider.Confirm(
                "Сохраняем значения в финальную версию. Все значения в финальной версии будут заменены. Нужно подтверждение.",
                async response =>
                {
                    if (response)
                    {
                        Lock();
                        await new BalancesServiceProxy().MoveValuesToOtherVersionAsync(
                            new MoveValuesToOtherVersionParameterSet
                            {
                                ContractId = _data.GetContract(_target).Id,
                                ToFinal = true
                            });
                        Unlock();
                    }
                },
                "Сохранение значений в финальную версию",
                "Сохранить",
                "Отмена"
                );
        }

        #endregion
    }
}
