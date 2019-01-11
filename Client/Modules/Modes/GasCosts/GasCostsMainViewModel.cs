using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.GasCosts;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.Entities;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.Modes.GasCosts.Access;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Imports;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Utils.Extensions;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.Modes.GasCosts.Dialogs.FuelGasInputVolumes;
using GazRouter.Modes.GasCosts.Dialogs.UnitFuelCosts;
using GazRouter.Modes.GasCosts.Dialogs.CoolingCosts;
using GazRouter.Modes.GasCosts.Dialogs.ChemicalAnalysisCosts;
using GazRouter.Modes.GasCosts.Dialogs.CleaningCosts;
using GazRouter.Modes.GasCosts.Dialogs.ControlEquipmentCosts;
using GazRouter.Modes.GasCosts.Dialogs.DiaphragmReplacementCosts;
using GazRouter.Modes.GasCosts.Dialogs.EnergyGenerationCosts;
using GazRouter.Modes.GasCosts.Dialogs.FuelGasHeatingCosts;
using GazRouter.Modes.GasCosts.Dialogs.HeatingCosts;
using GazRouter.Modes.GasCosts.Dialogs.HydrateRemoveCosts;
using GazRouter.Modes.GasCosts.Dialogs.MethanolFillingCosts;
using GazRouter.Modes.GasCosts.Dialogs.PipelineLoss;
using GazRouter.Modes.GasCosts.Dialogs.PopValveTuningCosts;
using GazRouter.Modes.GasCosts.Dialogs.PurgeCosts;
using GazRouter.Modes.GasCosts.Dialogs.RepairCosts;
using GazRouter.Modes.GasCosts.Dialogs.ShutdownCosts;
using GazRouter.Modes.GasCosts.Dialogs.TreatingShopHeatingCosts;
using GazRouter.Modes.GasCosts.Dialogs.UnitBleedingCosts;
using GazRouter.Modes.GasCosts.Dialogs.UnitStartCosts;
using GazRouter.Modes.GasCosts.Dialogs.UnitStopCosts;
using GazRouter.Modes.GasCosts.Dialogs.ValveControlCosts;
using GazRouter.Modes.GasCosts.Dialogs.CompStationLoss;
using GazRouter.Modes.GasCosts.Dialogs.CompUnitsHeatingCosts;
using GazRouter.Modes.GasCosts.Dialogs.CompUnitsTestingCosts;
using GazRouter.Modes.GasCosts.Dialogs.DrainageBleedingCosts;
using GazRouter.Modes.GasCosts.Dialogs.FluidControllerCosts;
using GazRouter.Modes.GasCosts.Dialogs.HeaterWorkCosts;
using GazRouter.Modes.GasCosts.Dialogs.KptgOwnNeedsCosts;
using GazRouter.Modes.GasCosts.Dialogs.PneumaticExploitationCosts;
using GazRouter.Modes.GasCosts.Dialogs.ReducingStationOwnNeedsCosts;
using GazRouter.Modes.GasCosts.Dialogs.ReservePowerStationMaintenanceCosts;
using GazRouter.Modes.GasCosts.Dialogs.ThermalDisposalUnitCosts;
using GazRouter.Modes.GasCosts.Dialogs.ValveExploitationCosts;
using GazRouter.Modes.GasCosts.Dialogs.VesselsExaminationCosts;
using GazRouter.Modes.GasCosts.Dialogs.BoilerConsumptions;
using GazRouter.Modes.GasCosts.Dialogs.IncidentLoss;

namespace GazRouter.Modes.GasCosts
{
    [RegionMemberLifetime(KeepAlive = false), UsedImplicitly]
    public class GasCostsMainViewModel : MainViewModelBase
    {
        #region constructor
        public GasCostsMainViewModel()
        {
            FillPipeLinesMainFilters();
            LoadListPipeLinesTreeFilters();
            IsEditPermission = Authorization2.Inst.IsEditable(LinkType.GasCosts);
            //
            _selectedMonth = GetToday();
            _compStationConsumptionViewModel = new CompStationConsumptionViewModel(this)
            {
                EditPermission = IsEditPermission
            };
            _pipelineConsumptionViewModel = new PipelineConsumptionViewModel(this)
            {
                EditPermission = IsEditPermission
            };
            _distrStationConsumptionViewModel = new DistrStationConsumptionViewModel(this)
            {
                EditPermission = IsEditPermission
            };
            _measStationConsumptionViewModel = new MeasStationConsumptionViewModel(this)
            {
                EditPermission = IsEditPermission
            };
            _consumptionSummaryViewModel = new ConsumptionSummaryViewModel(this);
            Tabs = new List<IConsumptionTab>
            {
                _compStationConsumptionViewModel,
                _pipelineConsumptionViewModel,
                _distrStationConsumptionViewModel,
                _measStationConsumptionViewModel,
                _consumptionSummaryViewModel
            };
            SelectedConsumption = Tabs.First();
            var targets = new List<Target> { Target.Norm, Target.Plan, Target.Fact };
            Targets = ClientCache.DictionaryRepository.Targets.Where(t => targets.Any(x => x == t.Target)).ToList();
            RefreshTarget();
            OnPropertyChanged(() => SelectedTarget);
            RefreshTree = new DelegateCommand(LoadTrees);
            SetDefaultValuesCommand = new DelegateCommand(OnSetDefaultValuesCommandExecuted, () => IsEditPermission);
            SetAccessCommand = new DelegateCommand(SetAccess, () => UserProfile.Current.Site.IsEnterprise && IsEditPermission);
            ImportValveSwitchesCommand = new DelegateCommand(ImportValveSwitches, () => SelectedTarget.Target == Target.Fact && IsEditPermission);
            FuelGasInputVolumesCommand = new DelegateCommand(FuelGasInputVolume, () => SelectedTarget.Target == Target.Fact && IsEditPermission && SelectedSiteId.HasValue && ShowDayly && IsAccessAllowed());

            BoilerCompStationConsumptionsInputCommand = new DelegateCommand(() => { BoilerConsumptionsInput(_compStationConsumptionViewModel); }, () => SelectedTarget.Target == Target.Fact && IsEditPermission && SelectedSiteId.HasValue && ShowDayly && IsAccessAllowed());
            BoilerPipelineConsumptionsInputCommand = new DelegateCommand(() => { BoilerConsumptionsInput(_pipelineConsumptionViewModel); }, () => SelectedTarget.Target == Target.Fact && IsEditPermission && SelectedSiteId.HasValue && ShowDayly && IsAccessAllowed());
            BoilerDistrStationConsumptionsInputCommand = new DelegateCommand(() => { BoilerConsumptionsInput(_distrStationConsumptionViewModel); }, () => SelectedTarget.Target == Target.Fact && IsEditPermission && SelectedSiteId.HasValue && ShowDayly && IsAccessAllowed());
            BoilerMeasStationConsumptionsInputCommand = new DelegateCommand(() => { BoilerConsumptionsInput(_measStationConsumptionViewModel); }, () => SelectedTarget.Target == Target.Fact && IsEditPermission && SelectedSiteId.HasValue && ShowDayly && IsAccessAllowed());
            BoilerAllConsumptionsInputCommand = new DelegateCommand(() =>
            {
                BoilerConsumptionsInput(_compStationConsumptionViewModel,
                                        _pipelineConsumptionViewModel,
                                        _distrStationConsumptionViewModel,
                                        _measStationConsumptionViewModel);
            }, () => SelectedTarget.Target == Target.Fact && IsEditPermission && SelectedSiteId.HasValue && ShowDayly && IsAccessAllowed());

            LoadPreviousDayDataThisTabCommand = new DelegateCommand(LoadPreviousDayDataCurrentTab, () => IsEditPermission);
            LoadPreviousDayDataAllTabsCommand = new DelegateCommand(LoadPreviousDayDataAllTabs, () => IsEditPermission);
        }
        #endregion

        #region variables
        private  CompStationConsumptionViewModel _compStationConsumptionViewModel;
        private  PipelineConsumptionViewModel _pipelineConsumptionViewModel;
        private  DistrStationConsumptionViewModel _distrStationConsumptionViewModel;
        private  MeasStationConsumptionViewModel _measStationConsumptionViewModel;
        private  ConsumptionSummaryViewModel _consumptionSummaryViewModel;
        private List<GasCostAccessDTO> _acl;
        public List<IConsumptionTab> Tabs { get; }
        public List<GasCostTypeDTO> CostTypeEntityTipeLinkList { get; set; }
        public List<DefaultParamValues> DefaultParamValues { get; set; }
        public CultureInfo CultureWithFormattedPeriod
        {
            get
            {
                var tempCultureInfo = new CultureInfo("ru-RU") { DateTimeFormat = { ShortDatePattern = ShowDayly ? "dd MMMM yyyy" : "MMMM yyyy" } };
                return tempCultureInfo;
            }
        }
        public DateTime DateEnd => DateTime.Today.AddYears(1).YearEnd();

        private bool _showDayly = true;
        public bool ShowDayly
        {
            get { return _showDayly; }
            set
            {
                bool changed = (_showDayly != value);

                _showDayly = value;
                OnPropertyChanged(() => ShowDayly);
                OnPropertyChanged(() => DateMode);
                OnPropertyChanged(() => CultureWithFormattedPeriod);
                OnPropertyChanged(() => EditDayly);

                if (changed)
                {
                    DateTime today = GetToday();
                    SelectedMonth = value ? today : today.ToLocal().MonthStart();
                }
                else
                {
                    SelectedMonth = SelectedMonth; //TODO: разобраться с тем, чтобы данные загружались в нужный момент.
                                                   //сейчас приходится принудительно присваивать значение, чтобы вызвать загрузку
                }

                OnPropertyChanged(() => SelectedMonth);
                FuelGasInputVolumesCommand.RaiseCanExecuteChanged();

                /*if (!old && value)
                {
                    //SelectedTarget = Targets.FirstOrDefault(o => o.Target == Target.Fact);
                    SelectedMonth = DateTime.Today;
                    OnPropertyChanged(() => SelectedMonth);
                }
                else if (old && !value)
                {
                    SelectedMonth = DateTime.Today.ToLocal().MonthStart();
                    OnPropertyChanged(() => SelectedMonth);
                }*/
            }
        }

        private static DateTime GetToday()
        {
            return SeriesHelper.GetPastDispDay().ToLocal();// DateTime.Today.AddDays(DateTime.Now.Hour < 10 ? -2 : -1);
        }

        public bool EditDayly
        {
            get { return !_showDayly; }
        }

        public string DateMode { get { return ShowDayly ? "Day" : "Month"; } }


        private bool _isEditPermission;
        public bool IsEditPermission
        {
            get { return _isEditPermission; }
            set
            {
                _isEditPermission = value;
                OnPropertyChanged(() => IsEditPermission);
            }
        }
        private TargetDTO _selectedTarget;
        public TargetDTO SelectedTarget
        {
            get { return _selectedTarget; }
            set
            {
                if (SetProperty(ref _selectedTarget, value))
                {
                    FillTabsCosts();
                    ImportValveSwitchesCommand.RaiseCanExecuteChanged();
                    if (value != null) IsolatedStorageManager.GasCostsLastTarget = value.Target;
                }
            }
        }
        private List<TargetDTO> _targets;
        public List<TargetDTO> Targets
        {
            get { return _targets; }
            set { SetProperty(ref _targets, value); }
        }
        private List<GasCostDTO> _gasCosts;
        public List<GasCostDTO> GasCosts
        {
            get { return _gasCosts; }
            set { SetProperty(ref _gasCosts, value); }
        }


        private bool _showPipelineFilter = false;
        public bool ShowPipelineFilter
        {
            get { return _showPipelineFilter; }
            set
            {
                _showPipelineFilter = value;
                OnPropertyChanged(() => ShowPipelineFilter);
            }
        }

        private IConsumptionTab previousConsumption;

        private IConsumptionTab _selectedConsumption;
        public IConsumptionTab SelectedConsumption
        {
            get { return _selectedConsumption; }
            set
            {
                previousConsumption = SelectedConsumption;

                if (previousConsumption != null)
                {
                    if (value == _consumptionSummaryViewModel)
                    {
                        UpdateAllConsumptionColumns(1);
                        _consumptionSummaryViewModel.UpdateFormat(Coef);                        
                    }
                    else
                    {
                        UpdateAllConsumptionColumns(Coef);
                    }
                    LoadGasCosts();
                }



                SetProperty(ref _selectedConsumption, value);
                ShowPipelineFilter = value == _pipelineConsumptionViewModel;
            }
        }
        

        private DateTime _selectedMonth;
        public DateTime SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                //if (SetProperty(ref _selectedMonth, value))
                {
                    SetProperty(ref _selectedMonth, value);
                    LoadGasCosts();
                    GetAccessList();
                }
            }
        }
        private Guid? _selectedSiteId;
        public Guid? SelectedSiteId
        {
            get { return _selectedSiteId; }
            set
            {
                if (SetProperty(ref _selectedSiteId, value))
                {
                    RefreshCommand.RaiseCanExecuteChanged();
                    LoadTrees();              
                }
            }
        }
        private List<SiteDTO> _sites;
        public List<SiteDTO> Sites
        {
            get { return _sites; }
            set { SetProperty(ref _sites, value); }
        }

        public List<PipelineTypeForFilter> pipelineMainTypes = new List<PipelineTypeForFilter>();

        private List<PipelineTypeForFilter> _pipeLinesTreeFilters = new List<PipelineTypeForFilter>();
        public List<PipelineTypeForFilter> PipeLinesTreeFilters
        {
            get { return _pipeLinesTreeFilters; }
            set { _pipeLinesTreeFilters = value; }
        }
        public string PipeLinesTreeFilterText
        {
            get
            { return string.Join(", ", _pipeLinesTreeFilters.Where(o => o.IsSelected).Select(x => x.DisplayName).ToArray()); }
        }
        public string PipeLinesTreeFilterTooltipText
        {
            get
            { return string.Join("\n", _pipeLinesTreeFilters.Where(o => o.IsSelected).Select(x => x.DisplayName).ToArray()); }
        }
        #endregion

        #region commands
        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ??
                       (_refreshCommand = new DelegateCommand(RefreshCommandExecution, () => SelectedSiteId != null));
            }
        }

        public DelegateCommand RefreshTree { get; set; }
        public DelegateCommand SetDefaultValuesCommand { get; set; }
        public DelegateCommand SetAccessCommand { get; set; }
        public DelegateCommand ImportValveSwitchesCommand { get; set; }
        public DelegateCommand FuelGasInputVolumesCommand { get; set; }
        public DelegateCommand BoilerCompStationConsumptionsInputCommand { get; set; }
        public DelegateCommand BoilerPipelineConsumptionsInputCommand { get; set; }
        public DelegateCommand BoilerDistrStationConsumptionsInputCommand { get; set; }
        public DelegateCommand BoilerMeasStationConsumptionsInputCommand { get; set; }
        public DelegateCommand BoilerAllConsumptionsInputCommand { get; set; }
        public DelegateCommand LoadPreviousDayDataAllTabsCommand { get; set; }
        public DelegateCommand LoadPreviousDayDataThisTabCommand { get; set; }
        #endregion

        #region INPUT STATE

        /// <summary>
        /// Список возможных статусов (ввод, подтверждено)
        /// </summary>
        public IEnumerable<ManualInputState> InputStateList
        {
            get
            {
                yield return ManualInputState.Input;
                yield return ManualInputState.Approved;
            }
        }

        private ManualInputState _inputState = ManualInputState.Input;

        /// <summary>
        /// Текущей статус ввода (ввод, подтверждено)
        /// </summary>
        public ManualInputState InputState
        {
            get { return _inputState; }
            set
            {
                if (SetProperty(ref _inputState, value))
                {
                    SetInputState();
                    InputStateInfo = value == ManualInputState.Approved ? UserProfile.Current.UserName : "";
                    OnPropertyChanged(() => InputStateInfo);
                }
                OnPropertyChanged(() => IsInputStateChangeAllowed);
                OnPropertyChanged(() => InputState);
                OnPropertyChanged(() => IsNotApproved);
                OnPropertyChanged(() => ShowDayly);                
                _compStationConsumptionViewModel.RefreshAccessAllowed();
                _pipelineConsumptionViewModel.RefreshAccessAllowed();
                _distrStationConsumptionViewModel.RefreshAccessAllowed();
                _measStationConsumptionViewModel.RefreshAccessAllowed();

                FuelGasInputVolumesCommand.RaiseCanExecuteChanged();
            }
        }

        private async void SetInputState()
        {
            try
            {
                Behavior.TryLock();
                var v = new GasCostAccessDTO
                {
                    Date       = SelectedMonth,
                    SiteId     = SelectedSiteId.Value,
                    Fact       = InputState == ManualInputState.Input,
                    Plan       = true,
                    Norm       = true,
                    PeriodType = DTO.Dictionaries.PeriodTypes.PeriodType.Day
                };
                await new GasCostsServiceProxy().UpdateGasCostAccessListAsync(new[] { v }.ToList());
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
        private void RefreshTarget()
        {
            _selectedTarget = IsolatedStorageManager.GasCostsLastTarget != Target.None &&
                  Targets.Any(e => e.Target == IsolatedStorageManager.GasCostsLastTarget) ?
                       Targets.Single(t => t.Target == IsolatedStorageManager.GasCostsLastTarget) :
                       Targets.First();
        }

        // Информация о том, кто и когда установил текущий статус
        public string InputStateInfo { get; set; }

        private int _previousUnitType = 0;
        private int _selectedUnitType;
        // Тип единиц измерения для ввода расхода газа
        public int SelectedUnitType
        {
            get { return _selectedUnitType; }
            set
            {
                _previousUnitType = _selectedUnitType;
                if (SetProperty(ref _selectedUnitType, value))
                {
                    IsolatedStorageManager.Set("VolumeInputUnits", value);
                    Dialogs.ViewModel.CalcViewModelBase<UnitFuelCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<CoolingCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<EnergyGenerationCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<TreatingShopHeatingCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<FuelGasHeatingCostModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<UnitStartCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<UnitStopCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<ValveControlCostsModel>.Coef = //?
                    Dialogs.ViewModel.CalcViewModelBase<ShutdownCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<PurgeCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<UnitBleedingCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<ChemicalAnalysisCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<CompStationLossModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<ControlEquipmentCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<ThermalDisposalUnitCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<CompUnitsTestingCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<CompUnitsHeatingCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<DiaphragmReplacementCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<PneumaticExploitationCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<ValveExploitationCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<RepairCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<CleaningCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<MethanolFillingCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<HydrateRemoveCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<HeatingCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<ReducingStationOwnNeedsCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<FluidControllerCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<HeaterWorkCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<PopValveTuningCostsModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<PipelineLossModel>.Coef =
                    Dialogs.ViewModel.CalcViewModelBase<KptgOwnNeedsCostsModel>.Coef = 
                    Dialogs.ViewModel.CalcViewModelBase<ReservePowerStationMaintenanceCostsModel>.Coef = Coef;
                    Dialogs.ViewModel.CalcViewModelBase<PipelineLossModel>.Coef = Coef;
                    Dialogs.ViewModel.CalcViewModelBase<IncidentLossModel>.Coef = Coef;
                    if (SelectedConsumption == _consumptionSummaryViewModel)
                        UpdateConsumptionSummaryTab();
                    else
                    {
                        UpdateAllConsumptionColumns(Coef);
                        _consumptionSummaryViewModel.UpdateFormat(Coef);
                        LoadGasCosts();
                    }
                }
            }
        }

        private int Coef => _selectedUnitType == 0 ? 1 : 1000;

        /// <summary>
        /// Разрешена ли смена текущего статуса ввода
        /// </summary>
        public bool IsInputStateChangeAllowed
        {
            get
            {
                switch (InputState)
                {
                    case ManualInputState.Input:
                        // Изменить статус можно только если нет ошибок данных по объектам
                        return _isEditPermission;

                    case ManualInputState.Approved:
                        // Сбросить статус "Подтверждено" может только пользователь ПДС
                        return UserProfile.Current.Site.IsEnterprise && _isEditPermission;

                    default:
                        return false;
                }
            }
        }

        #endregion

        #region methods

        public void FillPipeLinesMainFilters()
        {
            List<PipelineTypeForFilter> ml = new List<PipelineTypeForFilter>()
            {
                new PipelineTypeForFilter() { DisplayName = "Магистральный газопровод", Value = PipelineType.Main, IsSelected = true},
                new PipelineTypeForFilter() { DisplayName = "Распределительный газопровод", Value = PipelineType.Distribution, IsSelected = true },
                new PipelineTypeForFilter() { DisplayName = "Лупинг", Value = PipelineType.Looping, IsSelected = true },
                new PipelineTypeForFilter() { DisplayName = "Газопровод подключения", Value = PipelineType.Inlet, IsSelected = true },
                new PipelineTypeForFilter() { DisplayName = "Газопровод подключения", Value = PipelineType.CompressorShopBridge, IsSelected = true }
            };
            pipelineMainTypes.AddRange(ml);
        }

    public void LoadListPipeLinesTreeFilters()
        {
            _pipeLinesTreeFilters.Clear();

            foreach (var p in ClientCache.DictionaryRepository.PipelineTypes.Values.OrderBy(c => c.SortOrder).ToArray())
            {
                PipelineTypeForFilter nf = new PipelineTypeForFilter() { DisplayName = p.Name, Value = p.PipelineType };
                if (nf.Value != PipelineType.Main && nf.Value != PipelineType.Distribution && nf.Value != PipelineType.Looping && 
                    nf.Value != PipelineType.Inlet && nf.Value != PipelineType.CompressorShopBridge)
                   _pipeLinesTreeFilters.Add(new PipelineTypeForFilter() { DisplayName = p.Name, Value = p.PipelineType });
            }

            var storage = IsolatedStorageManager.PipelineTypesForPipelineConsumption;
            if (storage != null)
            {
                foreach (var p in _pipeLinesTreeFilters)
                {
                    if (storage.Contains(p.Value))
                        p.IsSelected = true;
                }
            }
            foreach (var p in pipelineMainTypes)
                p.IsSelected = true;
                /*else
                {
                    _pipeLinesTreeFilters.Where(x => x.Value == PipelineType.Main ||
                                                     x.Value == PipelineType.Distribution)
                                                     .ToList().ForEach(x => x.IsSelected = true);
                }*/
            }

        public void StoreListPipeLinesTreeFilters()
        {
            var s = PipeLinesTreeFilters.Where(o => o.IsSelected).ToArray();
            List<PipelineType> res = new List<PipelineType>();
            foreach (var p in s)
                res.Add(p.Value);
            IsolatedStorageManager.PipelineTypesForPipelineConsumption = res.ToArray();
        }

        public bool IsAccessAllowed()
        {
            if (SelectedTarget == null || _acl == null) return false;

            GasCostAccessDTO ac;
            if (_acl.Any(a => a.SiteId == SelectedSiteId))
                ac = _acl.Single(a => a.SiteId == SelectedSiteId);
            else
            {
                ac = new GasCostAccessDTO();
                ac.Plan = ac.Fact = ac.Norm = false;
            }            

            if (UserProfile.Current.Site.IsEnterprise)
            {

                if (_acl.All(a => a.SiteId != SelectedSiteId)) return false;

                if (ShowDayly)
                    return (ac.Fact && InputState == ManualInputState.Input && SelectedTarget.Target == Target.Fact);
            }
            else
            {
                if (ShowDayly)
                    return (InputState == ManualInputState.Input && SelectedTarget.Target == Target.Fact);
            }

            switch (SelectedTarget.Target)
            {
                case Target.Fact:
                    //return ac.Fact;
                    return false;
                case Target.Plan:
                    return ac.Plan;
                case Target.Norm:
                    return ac.Norm;
                default:
                    return false;
            }
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            LoadData();
        }
        public void SetTarget(Target target)
        {
            SelectedTarget = Targets.Single(c => c.Target == target);
        }
        private void OnSetDefaultValuesCommandExecuted()
        {
            var vm = new DefaultParamValuesViewModel(null) { DefaultParamValues = DefaultParamValues };

            var view = new DefaultParamValuesView { DataContext = vm };
            view.ShowDialog();
        }
        private void SetAccess()
        {
            var vm = new AccessViewModel(SelectedMonth, GetAccessList);
            var v = new AccessView { DataContext = vm };
            v.Closed += (s, a) =>
            {
                if (v.DialogResult == true)
                {
                    GetAccessList();
                    LoadGasCosts();
                }
            };
            v.ShowDialog();

        }
        private void ImportValveSwitches()
        {
            var vm = new ImportValveSwitchesViewModel(() => { }, SelectedMonth, SelectedSiteId.Value);
            var v = new ImportValveSwitchesView { DataContext = vm };
            v.Closed += (s, a) => { if (v.DialogResult == true) RefreshCommandExecution(); };
            v.ShowDialog();
        }

        private void FuelGasInputVolume()
        {
            var vm = new FuelGasInputVolumeViewModel(() => { }, SelectedMonth, SelectedSiteId.Value, Coef);

            var v = new FuelGasInputVolumeView { DataContext = vm };
            v.Closed += (s, a) => { if (v.DialogResult == true) LoadGasCosts(); };
            v.ShowDialog();
        }

        private void BoilerConsumptionsInput(params ConsumptionViewModelBase[] consumptions)
            {
                var vm = new BoilerConsumptionsViewModel(() => { }, GasCosts, SelectedMonth, consumptions, SelectedSiteId.Value, DefaultParamValues, ShowDayly, Coef);

                var v = new BoilerConsumptionsView { DataContext = vm };
                v.Closed += (s, a) => { if (v.DialogResult == true) RefreshCommandExecution(); };
                v.ShowDialog();
            }




        private async void LoadData()
        {
            Behavior.TryLock();
            CostTypeEntityTipeLinkList = await new GasCostsServiceProxy().GetCostTypeListAsync();

            var siteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    EnterpriseId = UserProfile.Current.Site.IsEnterprise ? UserProfile.Current.Site.Id : (Guid?)null
                });
            if (UserProfile.Current.Site.IsEnterprise)
                Sites = siteList;
            else
            {
                var site = siteList.Single(s => s.Id == UserProfile.Current.Site.Id);
                Sites = siteList.Where(s => s.Id == site.Id || site.DependantSiteIdList.Contains(s.Id)).ToList();
            }

            _selectedSiteId = UserProfile.Current.Site.IsEnterprise ? Sites.First().Id : UserProfile.Current.Site.Id;
            GetAccessList();
            OnPropertyChanged(() => SelectedSiteId);
            RefreshCommand.RaiseCanExecuteChanged();
            LoadTrees();
            Behavior.TryUnlock();
        }



        public async void LoadTrees()
        {
            if (SelectedSiteId == null) return;
            Behavior.TryLock();

            OnPropertyChanged(() => PipeLinesTreeFilterText);
            OnPropertyChanged(() => PipeLinesTreeFilterTooltipText);
            StoreListPipeLinesTreeFilters();

            var dataProvider = new ObjectModelServiceProxy();
            var compStationTreeTask = dataProvider.GetCompStationTreeAsync(
                SelectedSiteId.Value);
            var getPipeLineTreeTask = dataProvider.GetFullTreeAsync(new EntityTreeGetParameterSet
            {
                //todo наверно нужно не все
                Filter = EntityFilter.Sites |
                            EntityFilter.CompStations |
                            EntityFilter.CompShops |
                            EntityFilter.CompUnits |
                            EntityFilter.DistrStations |
                            EntityFilter.MeasStations |
                            EntityFilter.ReducingStations |
                            EntityFilter.MeasLines |
                            EntityFilter.DistrStationOutlets |
                            EntityFilter.Consumers |
                            EntityFilter.MeasPoints |
                            EntityFilter.CoolingStations |
                            EntityFilter.CoolingUnits |
                            EntityFilter.PowerPlants |
                            EntityFilter.PowerUnits |
                            EntityFilter.Boilers |
                            EntityFilter.BoilerPlants |
                            EntityFilter.Pipelines |
                            EntityFilter.LinearValves,
                SiteId = SelectedSiteId.Value
            });
            var getDistrStationTreetask =
                dataProvider.GetDistrStationTreeAsync(new GetDistrStationListParameterSet
                {
                    SiteId = SelectedSiteId.Value
                });
            var getMeasStationsTreeTask = dataProvider.GetMeasStationTreeAsync(
                new GetMeasStationListParameterSet { SiteId = SelectedSiteId.Value });
            await
                TaskEx.WhenAll(compStationTreeTask, getPipeLineTreeTask, getDistrStationTreetask,
                    getMeasStationsTreeTask);
            _compStationConsumptionViewModel.LoadTree(compStationTreeTask.Result);
            _pipelineConsumptionViewModel.LoadTree(getPipeLineTreeTask.Result);
            _distrStationConsumptionViewModel.LoadTree(getDistrStationTreetask.Result);
            _measStationConsumptionViewModel.LoadTree(getMeasStationsTreeTask.Result);
            Behavior.TryUnlock();
            //todo возможно стоит запускать запросы параллельно
            ShowDayly = ShowDayly; 
            //LoadGasCosts();
        }

        private void UpdateConsumptionSummaryTab()
        {
            UpdateAllConsumptionColumns(1);
            _consumptionSummaryViewModel.UpdateFormat(Coef);
            LoadGasCosts();
        }

        private void UpdateAllConsumptionColumns(int Coef)
        {
            _compStationConsumptionViewModel.UpdateColumns(Coef);
            _distrStationConsumptionViewModel.UpdateColumns(Coef);
            _measStationConsumptionViewModel.UpdateColumns(Coef);
            _pipelineConsumptionViewModel.UpdateColumns(Coef);
        }

       private void RefreshCommandExecution()
        {
            GetAccessList();
            _compStationConsumptionViewModel.firstChartData =
            _pipelineConsumptionViewModel.firstChartData =
            _distrStationConsumptionViewModel.firstChartData =
                _measStationConsumptionViewModel.firstChartData = true;  
                      
            if (SelectedConsumption == _consumptionSummaryViewModel)
                UpdateConsumptionSummaryTab();
            else LoadGasCosts();
        }

        public async void LoadGasCosts()
        {
            Behavior.TryLock();
            GasCosts =
                await
                    new GasCostsServiceProxy().GetGasCostListAsync(new GetGasCostListParameterSet
                    {
                        StartDate = ShowDayly ? SelectedMonth : SelectedMonth.ToLocal().MonthStart(),
                        EndDate = ShowDayly ? SelectedMonth : SelectedMonth.ToLocal().MonthEnd(),
                        SiteId = SelectedSiteId
                    });

            LoadDefaultParamValues();
            FillTabsCosts();
            Behavior.TryUnlock();
        }
        private async void LoadDefaultParamValues()
        {
            Behavior.TryLock();
            try
            {
                var list = await new GasCostsServiceProxy().GetDefaultParamValuesAsync(new GetGasCostListParameterSet
                {
                    StartDate = SelectedMonth.ToLocal().MonthStart(),
                    EndDate = SelectedMonth.ToLocal().MonthEnd(),
                    SiteId = SelectedSiteId
                });

                DefaultParamValues = new List<DefaultParamValues>
                {
                    new DefaultParamValues(
                        list.SingleOrDefault(d => d.Target == Target.Norm) ??
                        new DefaultParamValuesDTO
                        {
                            Target = Target.Norm,
                            Period = SelectedMonth,
                            SiteId = SelectedSiteId.Value
                        }),
                    new DefaultParamValues(
                        list.SingleOrDefault(d => d.Target == Target.Plan) ??
                        new DefaultParamValuesDTO
                        {
                            Target = Target.Plan,
                            Period = SelectedMonth,
                            SiteId = SelectedSiteId.Value
                        }),
                    new DefaultParamValues(
                        list.SingleOrDefault(d => d.Target == Target.Fact) ??
                        new DefaultParamValuesDTO
                        {
                            Target = Target.Fact,
                            Period = SelectedMonth,
                            SiteId = SelectedSiteId.Value
                        })
                };
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        /// <summary>
        /// Функция загрузки газовых значений из предыдущих суток в текущие
        /// </summary>
        /// 
        public bool IsNotApproved => InputState == ManualInputState.Input;

        private void LoadPreviousDayDataCurrentTab()
        {
            LoadPreviousDayData(true);
        }


        private void LoadPreviousDayDataAllTabs()
        {
            LoadPreviousDayData(false);
        }

        private void LoadPreviousDayData(bool forCurrentTab)
        {
            if (GasCosts.Count > 0)
            {
                MessageBoxProvider.Confirm("Загрузить и присоединить данные за предыдущий день к уже существующим?", confirm =>
                {
                    if (confirm)
                    {
                        ConfirmLoadPreviousDayData(forCurrentTab);
                    }
                }, "Подтверждение загрузки");
            }
            else
                ConfirmLoadPreviousDayData(forCurrentTab);

        }

        private async void ConfirmLoadPreviousDayData(bool forCurrentTab)
        {
            Behavior.TryLock();

            var loadedCosts = await new GasCostsServiceProxy().GetGasCostListAsync(new GetGasCostListParameterSet
            {
                StartDate = (ShowDayly ? SelectedMonth : SelectedMonth.ToLocal().MonthStart()).Add(new TimeSpan(-24, 0, 0)),
                EndDate = (ShowDayly ? SelectedMonth : SelectedMonth.ToLocal().MonthEnd()).Add(new TimeSpan(-24, 0, 0)),
                SiteId = SelectedSiteId
            });

            if(forCurrentTab)
            {
               var CostTypeCollection = Tabs.Single(x=>x== SelectedConsumption).GetCostTypeCollection();
                loadedCosts = loadedCosts.Where(x => CostTypeCollection.Contains(x.CostType)).ToList<GasCostDTO>();
            }

            foreach (var loadedCost in loadedCosts)
            {
                loadedCost.Date = loadedCost.Date.Add(new TimeSpan(24, 0, 0));

                var addGasCostParameterSet = new AddGasCostParameterSet
                {
                    CalculatedVolume = loadedCost.CalculatedVolume,
                    MeasuredVolume = loadedCost.MeasuredVolume,
                    Date = loadedCost.Date,
                    EntityId = loadedCost.Entity.Id,
                    CostType = loadedCost.CostType,                    
                    Target = loadedCost.Target,
                    InputData = loadedCost.InputString,
                    SiteId = loadedCost.SiteId
                };
                loadedCost.ChangeDate = DateTime.Now;
                loadedCost.ChangeUserName = UserProfile.Current.UserName;
                loadedCost.Id = await new GasCostsServiceProxy().AddGasCostAsync(addGasCostParameterSet);
                GasCosts.Add(loadedCost);
            }
            FillTabsCosts();

            Behavior.TryUnlock();
        }

        private void FillTabsCosts()
        {
            // в зависимости от заданной величины (м3 или тыс.м3) мы переводим размерности умножая Volum"ы на 1000 (м3)
            // или на 1 (тыс.м3), а после загрузки умноженных величин в список мы возвращаем исходные данные в прежнее состояние
            foreach (var gasCostDTO in GasCosts) 
            {
                gasCostDTO.MeasuredVolume = gasCostDTO.MeasuredVolume * Coef;
                gasCostDTO.CalculatedVolume = gasCostDTO.CalculatedVolume * Coef;
            }
            _compStationConsumptionViewModel.LoadGasCost(GasCosts);
            _pipelineConsumptionViewModel.LoadGasCost(GasCosts);
            _distrStationConsumptionViewModel.LoadGasCost(GasCosts);
            _measStationConsumptionViewModel.LoadGasCost(GasCosts);
            _consumptionSummaryViewModel.LoadGasCost(CostTypeEntityTipeLinkList, GasCosts);
            foreach (var gasCostDTO in GasCosts)  // возврат в оригинальную размерность тыс.м3, поделя м3 на 1000
            {
                gasCostDTO.MeasuredVolume = gasCostDTO.MeasuredVolume / Coef;
                gasCostDTO.CalculatedVolume = gasCostDTO.CalculatedVolume / Coef;
            }
        }
        //        public async Task<List<GasCostDTO>> UpdateGasCosts()
        //        {     /            
        //            
        //                   
        //            GasCosts = await
        //            new GasCostsServiceProxy().GetGasCostListAsync(new GetGasCostListParameterSet
        //            {
        //                StartDate = SelectedMonth.ToLocal().MonthStart(),
        //                EndDate = SelectedMonth.ToLocal().MonthEnd(),
        //                SiteId = SelectedSiteId
        //            });
        //            return GasCosts;
        //        }
        private async void GetAccessList()
        {
            if (ShowDayly)
            {
                var proxy = new GasCostsServiceProxy();
                var stateList = await proxy.GetGasCostAccessListAsync(
                    UserProfile.Current.Site.IsEnterprise ?
                        new GetGasCostAccessListParameterSet()
                        {
                            Date = SelectedMonth,
                            EnterpriseId = UserProfile.Current.Site.Id,
                            SiteId = _selectedSiteId,
                            PeriodType = DTO.Dictionaries.PeriodTypes.PeriodType.Day
                        }
                        :
                        new GetGasCostAccessListParameterSet()
                        {
                            Date = SelectedMonth,
                            SiteId = _selectedSiteId,// UserProfile.Current.Site.Id,
                            PeriodType = DTO.Dictionaries.PeriodTypes.PeriodType.Day
                        });

                var state = stateList.FirstOrDefault();
                if (state?.Date == SelectedMonth)
                {
                    _inputState = state?.Fact ?? true ? ManualInputState.Input : ManualInputState.Approved;
                    InputState = _inputState;
                    InputStateInfo = state?.ChangeUser;
                    OnPropertyChanged(() => InputStateInfo);
                    FuelGasInputVolumesCommand.RaiseCanExecuteChanged();
                }
            }
            if (UserProfile.Current.Site.IsEnterprise)
            {
                _acl =
                    await new GasCostsServiceProxy().GetGasCostAccessListAsync(
                        new GetGasCostAccessListParameterSet
                        {
                            Date = SelectedMonth.MonthStart(),
                            EnterpriseId = UserProfile.Current.Site.Id,
                            PeriodType = DTO.Dictionaries.PeriodTypes.PeriodType.Month
                        });
            }
            else
            {
                _acl =
                    await new GasCostsServiceProxy().GetGasCostAccessListAsync(
                        new GetGasCostAccessListParameterSet
                        {
                            Date = SelectedMonth.MonthStart(),
                            SiteId = UserProfile.Current.Site.Id,
                            PeriodType = DTO.Dictionaries.PeriodTypes.PeriodType.Month
                        });
            }

        }
        #endregion
    }

    public class PipelineTypeForFilter
    {
        public PipelineType Value { get; set; }
        public string DisplayName { get; set; }
        public bool IsSelected { get; set; }
    }
}