using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using Microsoft.Practices.Prism.Commands;
namespace GazRouter.Modes.GasCosts2
{
    public class CostModelBase : PropertyChangedBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private double _factTotalToDate;
        public double FactTotalToDate
        {
            get { return _factTotalToDate; }
            set { SetProperty(ref _factTotalToDate, value); }
        }

        private double _fact;
        public double Fact
        {
            get { return _fact; }
            set { SetProperty(ref _fact, value); }
        }

        private double _norm;
        public double Norm
        {
            get { return _norm; }
            set { SetProperty(ref _norm, value); }
        }

        private double _plan;
        public double Plan
        {
            get { return _plan; }
            set { SetProperty(ref _plan, value); }
        }

        public virtual void UpdateProperty()
        {
            OnPropertyChanged(() => FactTotalToDate);
            OnPropertyChanged(() => Fact);
            OnPropertyChanged(() => Norm);
            OnPropertyChanged(() => Plan);
        }
    }
    public class EntityRowBase : CostModelBase
    {
        public EntityRowBase(Guid objectId)
        {
            ObjectId = objectId;
            PropertyChanged += OnPropertyChanged;
        }

        public Guid ObjectId { get; }

        private double _planDelta;
        public double PlanDelta
        {
            get { return _planDelta; }
            set { SetProperty(ref _planDelta, value); }
        }

        private double _normDelta;
        public double NormDelta
        {
            get { return _normDelta; }
            set { SetProperty(ref _normDelta, value); }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Fact)))
            {
                CalcPlanDelta();
                CalcNormDelta();
                return;
            }
            if (e.PropertyName.Equals(nameof(Plan)))
            {
                CalcPlanDelta();
                return;
            }
            if (e.PropertyName.Equals(nameof(Norm)))
            {
                CalcNormDelta();
                return;
            }
        }
        private void CalcPlanDelta()
        {
            PlanDelta = Plan - Fact;
        }
        private void CalcNormDelta()
        {
            NormDelta = Norm - Fact;
        }

        public override void UpdateProperty()
        {
            base.UpdateProperty();
            OnPropertyChanged(() => PlanDelta);
            OnPropertyChanged(() => NormDelta);
        }
    }
    public class ObjectItem : EntityRowBase
    {
        public ObjectItem(Guid objectId) : base(objectId)
        {
            _dayCosts   = new List<GasCostDTO>();
            _monthCosts = new List<GasCostDTO>();
        }

        public EntityType EntityType { get; set; }
        private readonly List<GasCostDTO> _dayCosts;
        private readonly List<GasCostDTO> _monthCosts;

        public List<GasCostDTO> GetDayCosts()
        {
            return _dayCosts;
        }
        public List<GasCostDTO> GetMonthCosts()
        {
            return _monthCosts;
        }
        public void AddCost(GasCostDTO cost, DateTime selectedDate)
        {
            _monthCosts.Add(cost);
            if (cost.Date.Day == selectedDate.Day) _dayCosts.Add(cost);           
        }
        public void RemoveCost(int id)
        {
            var cost = _dayCosts.Single(e => e.Id == id);
            _monthCosts.Remove(cost);
            _dayCosts.Remove(cost);
        }
        public void UpdateAll()
        {
            UpdateFactTotalToDate();
            UpdateFact();
            UpdatePlan();
            UpdateNorm();
        }
        private void UpdateFactTotalToDate()
        {
            FactTotalToDate = _dayCosts.Where(e => e.Target == Target.Fact).Sum(e => e.Volume);
        }
        private void UpdateFact()
        {
            Fact = _monthCosts.Where(e => e.Target == Target.Fact).Sum(e => e.Volume);
        }
        private void UpdatePlan()
        {
            Plan = _monthCosts.Where(e => e.Target == Target.Plan).Sum(e => e.Volume);
        }
        private void UpdateNorm()
        {
            Norm = _monthCosts.Where(e=>e.Target == Target.Norm).Sum(e => e.Volume);
        }
    }
    public class PipelineObjectItem : ObjectItem
    {
        public PipelineObjectItem(Guid objectId) : base(objectId)
        {
        }
        public PipelineType PipelineType { get; set; }
    }
    public class MainCommands
    {
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand SetDefaultValuesCommand { get; set; }
        public DelegateCommand PipelineFilterCommand { get; set; }

#region magic_wand
        public DelegateCommand QuickInputCommand { get; set; }
        public DelegateCommand ImportValveSwitchesCommand { get; set; }
        public DelegateCommand FuelGasInputVolumesCommand { get; set; }
        public DelegateCommand BoilerAllConsumptionsInputCommand { get; set; }
        public DelegateCommand BoilerCompStationConsumptionsInputCommand { get; set; }
        public DelegateCommand BoilerPipelineConsumptionsInputCommand { get; set; }
        public DelegateCommand BoilerDistrStationConsumptionsInputCommand { get; set; }
        public DelegateCommand BoilerMeasStationConsumptionsInputCommand { get; set; }
#endregion

#region load_previous
        public DelegateCommand PreviewStateCopyCommand  { get; set; }
        public DelegateCommand LoadPreviousDayDataAllTabsCommand { get; set; }
        public DelegateCommand LoadPreviousDayDataKcCommand { get; set; }
        public DelegateCommand LoadPreviousDayDataPipelineCommand { get; set; }
        public DelegateCommand LoadPreviousDayDataGrsCommand { get; set; }
        public DelegateCommand LoadPreviousDayDataGisCommand { get; set; }
#endregion
    }
}
#region trash
//public DelegateCommand BoilerConsumptionsInputCommand { get; set; }


//        public DelegateCommand SetAccessCommand { get; set; }

//        public DelegateCommand RefreshTree { get; set; }

//public void AddCost(GasCostDTO cost, DateTime selectedDate)
//{
//    _monthCosts.Add(cost);
//    switch (cost.Target)
//    {
//        case Target.Fact:
//            if (cost.Date.Day == selectedDate.Day)
//            {
//                _dayCosts.Add(cost);
//                FactTotalToDate = _dayCosts.Sum(e => e.Volume); // += cost.Volume;
//            }
//            else
//                FactTotalToDate += 0;
//
//            Fact += cost.Volume;
//            break;
//
//        case Target.Plan:
//            Plan += cost.Volume;
//            break;
//        case Target.Norm:
//            Norm += cost.Volume;
//            break;
//    }
//}
//public void RemoveCost(int id, DateTime selectedDate)
//{
//    var cost = _monthCosts.Single(e => e.Id == id);
//    RemoveCost(cost, selectedDate);
//}
//public void RemoveCost(GasCostDTO cost, DateTime selectedDate)
//{
//    _monthCosts.Remove(cost);
//    _dayCosts.Remove(cost);
//    switch (cost.Target)
//    {
//        case Target.Fact:
//            Fact -= cost.Volume;
//            FactTotalToDate -= cost.Date.Day == selectedDate.Day ? cost.Volume : 0;
//            break;
//
//        case Target.Plan:
//            Plan -= cost.Volume;
//            break;
//
//        case Target.Norm:
//            Norm -= cost.Volume;
//            break;
//    }
//    if (_dayCosts.Count == 0) FactTotalToDate = 0;
//    if (_monthCosts.Count == 0) Fact = Plan = Norm = 0;
//}


//FactTotalToDate += cost.Date.Day == selectedDate.Day? cost.Volume : 0;

//        public GasCostDTO GetCost(int id)
//        {
//            return _costs.Single(e => e.Id == id);
//        }



//public class EntitySummary : EntityRowBase
//{
//    public EntitySummary(Guid id) : base(id)
//    {
//    }
//}

//public class GasCostParameters : PropertyChangedBase
//{
//    public GasCostParameters()
//    {
//        CostTypeEntityTypeLinkDict = new Dictionary<CostType, GasCostTypeDTO>();
//    }
//
//    private bool _isEditPermission;
//    public void SetEditPermission(bool permission)
//    {
//        _isEditPermission = permission;
//    }
//    #region Actions
//    public Action RadDatePickerAction { get; set; }
//    public Action SelectedSiteIdAction { get; set; }
//    public Action SetInputStateAction { get; set; }
//    #endregion
//    #region SelectedMonth
//    //  DateMode
//    public string DateMode { get { return ShowDayly ? "Day" : "Month"; } }
//    //  SelectedMonth
//    private DateTime _selectedMonth;
//    public DateTime SelectedMonth
//    {
//        get { return _selectedMonth; }
//        set
//        {
//            SetProperty(ref _selectedMonth, value);
//            RadDatePickerAction.Invoke();// todo:  LoadGasCosts();  +  GetAccessList();  +                
//        }
//    }
//    //  DateEnd
//    public DateTime DateEnd => DateTime.Today.AddYears(1).YearEnd();
//    public void PrivateSetSelectedMonth(DateTime dateTime)
//    {
//        _selectedMonth = dateTime;
//    }
//    //  CultureWithFormattedPeriod
//    public CultureInfo CultureWithFormattedPeriod
//    {
//        get
//        {
//            var tempCultureInfo = new CultureInfo("ru-RU") { DateTimeFormat = { ShortDatePattern = ShowDayly ? "dd MMMM yyyy" : "MMMM yyyy" } };
//            return tempCultureInfo;
//        }
//    }
//    #endregion
//    #region Sites
//    //  Sites
//    private List<SiteDTO> _sites;
//    public List<SiteDTO> Sites
//    {
//        get { return _sites; }
//        set { SetProperty(ref _sites, value); }
//    }
//    //  SelectedSiteId
//    private Guid? _selectedSiteId;
//    public Guid? SelectedSiteId
//    {
//        get { return _selectedSiteId; }
//        set
//        {
//            if (SetProperty(ref _selectedSiteId, value))
//            {
//                if (_selectedSiteId == null) return;
//                SelectedSiteIdAction.Invoke();
//            }
//        }
//    }
//    public void PrivateSetSelectedSiteId(Guid? guid)
//    {
//        _selectedSiteId = guid;
//        OnPropertyChanged(() => SelectedSiteId);
//    }
//    #endregion
//    #region SetAccessCommand
//    // - ShowDayly
//    private bool _showDayly = true;
//    public bool ShowDayly
//    {
//        get { return _showDayly; }
//        set
//        {
//            bool changed = (_showDayly != value);
//            _showDayly = value;
//            OnPropertyChanged(() => ShowDayly);
//            OnPropertyChanged(() => DateMode);
//            OnPropertyChanged(() => CultureWithFormattedPeriod);
//            //                OnPropertyChanged(() => EditDayly);
//            if (changed)
//            {
//                //                    DateTime today = GetToday();
//                //                    SelectedMonth = value ? today : today.ToLocal().MonthStart();
//            }
//            else
//            {
//                SelectedMonth = SelectedMonth; //TODO: разобраться с тем, чтобы данные загружались в нужный момент.
//                                               //сейчас приходится принудительно присваивать значение, чтобы вызвать загрузку
//            }
//
//            OnPropertyChanged(() => SelectedMonth);
//            //                FuelGasInputVolumesCommand.RaiseCanExecuteChanged();
//
//            /*if (!old && value)
//            {
//                //SelectedTarget = Targets.FirstOrDefault(o => o.Target == Target.Fact);
//                SelectedMonth = DateTime.Today;
//                OnPropertyChanged(() => SelectedMonth);
//            }
//            else if (old && !value)
//            {
//                SelectedMonth = DateTime.Today.ToLocal().MonthStart();
//                OnPropertyChanged(() => SelectedMonth);
//            }*/
//        }
//    }
//    #endregion
//    #region Input_State
//    private ManualInputState _inputState = ManualInputState.Input;
//    /// <summary>
//    /// Текущей статус ввода (ввод, подтверждено)
//    /// </summary>
//    public ManualInputState InputState
//    {
//        get { return _inputState; }
//        set
//        {
//            if (SetProperty(ref _inputState, value))
//            {
//                SetInputStateAction.Invoke();
//                InputStateInfo = value == ManualInputState.Approved ? UserProfile.Current.UserName : "";
//                OnPropertyChanged(() => InputStateInfo);
//            }
//            OnPropertyChanged(() => IsInputStateChangeAllowed);
//            OnPropertyChanged(() => InputState);
//            OnPropertyChanged(() => IsNotApproved);
//            // FuelGasInputVolumesCommand.RaiseCanExecuteChanged();       
//        }
//    }
//    public void SetPrivateInputState(ManualInputState inputState)
//    {
//        _inputState = inputState;
//    }
//    /// <summary> Список возможных статусов (ввод, подтверждено) </summary>
//    public IEnumerable<ManualInputState> InputStateList
//    {
//        get
//        {
//            yield return ManualInputState.Input;
//            yield return ManualInputState.Approved;
//        }
//    }
//    /// <summary>
//    /// Разрешена ли смена текущего статуса ввода
//    /// </summary>
//    public bool IsInputStateChangeAllowed
//    {
//        get
//        {
//            switch (InputState)
//            {
//                case ManualInputState.Input:
//                    // Изменить статус можно только если нет ошибок данных по объектам
//                    return _isEditPermission;
//
//                case ManualInputState.Approved:
//                    // Сбросить статус "Подтверждено" может только пользователь ПДС
//                    return UserProfile.Current.Site.IsEnterprise && _isEditPermission;
//
//                default:
//                    return false;
//            }
//        }
//    }
//    // Информация о том, кто и когда установил текущий статус
//    public string InputStateInfo { get; set; }
//    #endregion
//    #region SelectedUnit
//    // SelectedUnit
//    private int _previousUnitType;
//    private int _selectedUnitType;
//    public int SelectedUnit
//    {
//        get { return _selectedUnitType; }
//        set
//        {
//            _previousUnitType = _selectedUnitType;
//            if (SetProperty(ref _selectedUnitType, value))
//            {
//                IsolatedStorageManager.Set("VolumeInputUnits", value);
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<UnitFuelCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<CoolingCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<EnergyGenerationCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<TreatingShopHeatingCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<FuelGasHeatingCostModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<UnitStartCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<UnitStopCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<ValveControlCostsModel>.Coef = //?
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<ShutdownCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<PurgeCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<UnitBleedingCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<ChemicalAnalysisCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<CompStationLossModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<ControlEquipmentCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<ThermalDisposalUnitCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<CompUnitsTestingCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<CompUnitsHeatingCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<DiaphragmReplacementCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<PneumaticExploitationCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<ValveExploitationCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<RepairCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<CleaningCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<MethanolFillingCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<HydrateRemoveCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<HeatingCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<ReducingStationOwnNeedsCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<FluidControllerCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<HeaterWorkCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<PopValveTuningCostsModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<PipelineLossModel>.Coef =
//                //                    GasCosts.Dialogs.ViewModel.CalcViewModelBase<KptgOwnNeedsCostsModel>.Coef = Coef;
//                //                    if (SelectedConsumption == _consumptionSummaryViewModel)  +
//                //                        UpdateConsumptionSummaryTab();                        +
//                //                    else                                                      +
//                //                    {                                                         + 
//                //                        UpdateAllConsumptionColumns(Coef);                    +
//                //                        _consumptionSummaryViewModel.UpdateFormat(Coef);      +
//                //                        LoadGasCosts();                                       +
//                //                    }                                                         +
//            }
//        }
//    }
//    #endregion
//    #region not_sorted
//    private IConsumptionTab _selectedConsumption;
//    public IConsumptionTab SelectedConsumption
//    {
//        get { return _selectedConsumption; }
//        set
//        {
//            //                previousConsumption = SelectedConsumption;   
//            //
//            //                if (previousConsumption != null)
//            //                {
//            //                    if (value == _consumptionSummaryViewModel)
//            //                    {
//            //                        UpdateAllConsumptionColumns(1);
//            //                        _consumptionSummaryViewModel.UpdateFormat(Coef);
//            //                    }
//            //                    else
//            //                    {
//            //                        UpdateAllConsumptionColumns(Coef);
//            //                    }
//            //                    LoadGasCosts();
//            //                }
//
//
//            //                SetProperty(ref _selectedConsumption, value);
//            //                ShowPipelineFilter = value == _pipelineConsumptionViewModel;
//        }
//    }
//    public int Coef => _selectedUnitType == 0 ? 1 : 1000;
//    // IsNotApproved
//    public bool IsNotApproved => InputState == ManualInputState.Input;
//    #endregion
//    #region not_sorted2
//    private List<GasCostDTO> _gasCostsForMonth;
//    public List<GasCostDTO> GasCostsForMonth
//    {
//        get { return _gasCostsForMonth; }
//        set { SetProperty(ref _gasCostsForMonth, value); }
//    }
//    //        private List<GasCostDTO> _gasCostsForDay;
//    //        public List<GasCostDTO> GasCostsForDay
//    //        {
//    //            get { return _gasCostsForDay; }
//    //            set { SetProperty(ref _gasCostsForDay, value); }
//    //        }
//    private List<GasCostTypeDTO> _costTypeEntityTypeLinkList;
//    public List<GasCostTypeDTO> CostTypeEntityTypeLinkList
//    {
//        get { return _costTypeEntityTypeLinkList; }
//        set
//        {
//            _costTypeEntityTypeLinkList = value;
//            CostTypeEntityTypeLinkDict = CostTypeEntityTypeLinkList
//                .ToDictionary(k => k.CostType, v => v);
//        }
//    }
//    public Dictionary<CostType, GasCostTypeDTO> CostTypeEntityTypeLinkDict { get; set; }
//
//    public List<DefaultParamValues> DefaultParamValues { get; set; }
//    public List<GasCostAccessDTO> AccessList;
//
//    public CostType SelectedCostType { get; set; }
//    #endregion
//    #region piepline_filter
//    // ShowPipelineFilter
//    //  
//    //  PipeLinesTreeFilterChanged - event?    DisplayName, IsSelected
//    private bool _showPipelineFilter = false;
//    public bool ShowPipelineFilter
//    {
//        get { return _showPipelineFilter; }
//        set
//        {
//            _showPipelineFilter = value;
//            OnPropertyChanged(() => ShowPipelineFilter);
//        }
//    }
//    // PipeLinesTreeFilters
//    private List<PipelineTypeForFilter> _pipeLinesTreeFilters = new List<PipelineTypeForFilter>();
//    public List<PipelineTypeForFilter> PipeLinesTreeFilters
//    {
//        get { return _pipeLinesTreeFilters; }
//        set { _pipeLinesTreeFilters = value; }
//    }
//    //  PipeLinesTreeFilterTooltipText
//    public string PipeLinesTreeFilterTooltipText
//    {
//        get
//        {
//            return string.Join("\n", _pipeLinesTreeFilters
//                .Where(o => o.IsSelected)
//                .Select(x => x.DisplayName).ToArray());
//        }
//    }
//    public List<PipelineTypeForFilter> PipelineMainTypes = new List<PipelineTypeForFilter>();
//    #endregion
//}

//public void AddCost(GasCostDTO cost)
//{
//    if (_costs == null) _costs = new List<GasCostDTO>();
//    _costs.Add(cost);
//    switch (cost.Target)
//    {
//        case Target.Fact:
//            Fact += cost.Volume;
//            return;
//        case Target.Plan:
//            Plan += cost.Volume;
//            return;
//        case Target.Norm:
//            Norm += cost.Volume;
//            return;
//    }
//}

//        private double _factTotalToDate;
//        public double FactTotalToDate
//        {
//            get { return _factTotalToDate; }
//            set
//            {
//                SetProperty(ref _factTotalToDate, value);
//                CalcPlanDelta();
//                CalcNormDelta();
//            }
//        }

//        private double _fact;
//        public double Fact
//        {
//            get { return _fact; }
//            set
//            {
//                SetProperty(ref _fact, value);
//                CalcPlanDelta();
//                CalcNormDelta();
//            }
//        }
//        private double _plan;
//        public double Plan
//        {
//            get { return _plan; }
//            set
//            {
//                SetProperty(ref _plan, value);
//                CalcPlanDelta();
//            }
//        }//
//        private double _norm;
//        public double Norm
//        {
//            get { return _norm; }
//            set
//            {
//                SetProperty(ref _norm, value);
//                CalcNormDelta();
//            }
//        }

#endregion