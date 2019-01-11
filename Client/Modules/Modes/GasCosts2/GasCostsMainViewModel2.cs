using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.Diagnostics;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.GasCosts;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ManualInput.CompUnitStates;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.Modes.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.BoilerConsumptions;
using GazRouter.Modes.GasCosts.Dialogs.ChemicalAnalysisCosts;
using GazRouter.Modes.GasCosts.Dialogs.CleaningCosts;
using GazRouter.Modes.GasCosts.Dialogs.CompStationLoss;
using GazRouter.Modes.GasCosts.Dialogs.CompUnitsHeatingCosts;
using GazRouter.Modes.GasCosts.Dialogs.CompUnitsTestingCosts;
using GazRouter.Modes.GasCosts.Dialogs.ControlEquipmentCosts;
using GazRouter.Modes.GasCosts.Dialogs.CoolingCosts;
using GazRouter.Modes.GasCosts.Dialogs.DiaphragmReplacementCosts;
using GazRouter.Modes.GasCosts.Dialogs.EnergyGenerationCosts;
using GazRouter.Modes.GasCosts.Dialogs.FluidControllerCosts;
using GazRouter.Modes.GasCosts.Dialogs.FuelGasHeatingCosts;
using GazRouter.Modes.GasCosts.Dialogs.FuelGasInputVolumes;
using GazRouter.Modes.GasCosts.Dialogs.HeaterWorkCosts;
using GazRouter.Modes.GasCosts.Dialogs.HeatingCosts;
using GazRouter.Modes.GasCosts.Dialogs.HydrateRemoveCosts;
using GazRouter.Modes.GasCosts.Dialogs.KptgOwnNeedsCosts;
using GazRouter.Modes.GasCosts.Dialogs.MethanolFillingCosts;
using GazRouter.Modes.GasCosts.Dialogs.PipelineLoss;
using GazRouter.Modes.GasCosts.Dialogs.PneumaticExploitationCosts;
using GazRouter.Modes.GasCosts.Dialogs.PopValveTuningCosts;
using GazRouter.Modes.GasCosts.Dialogs.PurgeCosts;
using GazRouter.Modes.GasCosts.Dialogs.ReducingStationOwnNeedsCosts;
using GazRouter.Modes.GasCosts.Dialogs.RepairCosts;
using GazRouter.Modes.GasCosts.Dialogs.ReservePowerStationMaintenanceCosts;
using GazRouter.Modes.GasCosts.Dialogs.ShutdownCosts;
using GazRouter.Modes.GasCosts.Dialogs.ThermalDisposalUnitCosts;
using GazRouter.Modes.GasCosts.Dialogs.TreatingShopHeatingCosts;
using GazRouter.Modes.GasCosts.Dialogs.UnitBleedingCosts;
using GazRouter.Modes.GasCosts.Dialogs.UnitFuelCosts;
using GazRouter.Modes.GasCosts.Dialogs.UnitStartCosts;
using GazRouter.Modes.GasCosts.Dialogs.UnitStopCosts;
using GazRouter.Modes.GasCosts.Dialogs.ValveControlCosts;
using GazRouter.Modes.GasCosts.Dialogs.ValveExploitationCosts;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using GazRouter.Modes.GasCosts.Imports;
using GazRouter.Repair.Agreement;
using JetBrains.Annotations;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
namespace GazRouter.Modes.GasCosts2
{
    public class Unit
    {
        public string Description { get; set; }
        public int Coef { get; set; }
    }
    /// <summary>
    /// 
    /// удаление и добавление costs - делать в CostsViewModel
    /// 
    /// </summary>
    public class GasCostParameters : PropertyChangedBase
    {
        public GasCostParameters()
        {
            CostTypeEntityTypeLinkDict = new Dictionary<CostType, GasCostTypeDTO>();
        }

        private bool _isEditPermission;
        public void SetEditPermission(bool permission)
        {
            _isEditPermission = permission;
        }
#region actions
        public Action RadDatePickerAction { get; set; }
        public Action SelectedSiteIdAction { get; set; }
        public Action SetInputStateAction { get; set; }
        public Action<int> SetUnitAction { get; set; }
#endregion
#region selectedMonth
        // DateMode
        public string DateMode => "Day";
        // SelectedMonth
        private DateTime _selectedMonth;
        public DateTime SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                SetProperty(ref _selectedMonth, value);
                RadDatePickerAction.Invoke();// todo:  LoadGasCosts();  +  GetAccessList();  +                
            }
        }
        // DateEnd
        public DateTime DateEnd => DateTime.Today.AddYears(1).YearEnd();
        public void PrivateSetSelectedMonth(DateTime dateTime)
        {
            _selectedMonth = dateTime;
        }
        // CultureWithFormattedPeriod
        public CultureInfo CultureWithFormattedPeriod
        {
            get
            {
                var tempCultureInfo = new CultureInfo("ru-RU")
                {
                    DateTimeFormat = { ShortDatePattern = "dd MMMM yyyy" }
                };
                return tempCultureInfo;
            }
        }
#endregion
#region sites
        //  Sites
        private List<SiteDTO> _sites;
        public List<SiteDTO> Sites
        {
            get { return _sites; }
            set { SetProperty(ref _sites, value); }
        }
        //  SelectedSiteId
        private Guid? _selectedSiteId;
        public Guid? SelectedSiteId
        {
            get { return _selectedSiteId; }
            set
            {
                if (SetProperty(ref _selectedSiteId, value))
                {
                    if (_selectedSiteId == null) return;
                    SelectedSiteIdAction.Invoke();
                }
            }
        }
        public void PrivateSetSelectedSiteId(Guid? guid)
        {
            _selectedSiteId = guid;
            OnPropertyChanged(() => SelectedSiteId);
        }
        #endregion
#region input_State        
        public bool IsNotApproved => InputState == ManualInputState.Input;
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
                    SetInputStateAction.Invoke();
                    InputStateInfo = value == ManualInputState.Approved ? UserProfile.Current.UserName : "";
                    OnPropertyChanged(() => InputStateInfo);
                }
                OnPropertyChanged(() => IsInputStateChangeAllowed);
                OnPropertyChanged(() => InputState);
                OnPropertyChanged(() => IsNotApproved);
            }
        }
        public void SetPrivateInputState(ManualInputState inputState)
        {
            _inputState = inputState;
        }
        /// <summary> Список возможных статусов (ввод, подтверждено) </summary>
        public IEnumerable<ManualInputState> InputStateList
        {
            get
            {
                yield return ManualInputState.Input;
                yield return ManualInputState.Approved;
            }
        }
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
        // Информация о том, кто и когда установил текущий статус
        public string InputStateInfo { get; set; }
#endregion
#region selectedUnitType
        public List<Unit> Units { get; set; }

        private Unit _selectedUnit;
        public Unit SelectedUnit
        {
            get { return _selectedUnit; }
            set
            {
                if (SetProperty(ref _selectedUnit, value))
                    SetUnitAction(value.Coef);
            }
        }
#endregion
#region not_sorted
        private List<GasCostDTO> _gasCostsForMonth;
        public List<GasCostDTO> GasCostsForMonth
        {
            get { return _gasCostsForMonth; }
            set { SetProperty(ref _gasCostsForMonth, value); }
        }
        //        private List<GasCostDTO> _gasCostsForDay;
        //        public List<GasCostDTO> GasCostsForDay
        //        {
        //            get { return _gasCostsForDay; }
        //            set { SetProperty(ref _gasCostsForDay, value); }
        //        }
        private List<GasCostTypeDTO> _costTypeEntityTypeLinkList;
        public List<GasCostTypeDTO> CostTypeEntityTypeLinkList
        {
            get { return _costTypeEntityTypeLinkList; }
            set
            {
                _costTypeEntityTypeLinkList = value;
                CostTypeEntityTypeLinkDict = CostTypeEntityTypeLinkList
                    .ToDictionary(k => k.CostType, v => v);
            }
        }
        public Dictionary<CostType, GasCostTypeDTO> CostTypeEntityTypeLinkDict { get; set; }

        public List<DefaultParamValues> DefaultParamValues { get; set; }
        public List<GasCostAccessDTO> AccessList;

        public CostType SelectedCostType { get; set; }
#endregion
#region piepline_filter
        private List<PipelineTypeForFilter> _pipeLinesTreeFilters = new List<PipelineTypeForFilter>();
        public List<PipelineTypeForFilter> PipeLinesTreeFilters
        {
            get { return _pipeLinesTreeFilters; }
            set
            {
                _pipeLinesTreeFilters = value;
                OnPropertyChanged(() => PipeLinesTreeFilterTooltipText);
            }
        }

        private string _pipeLinesTreeFilterTooltipText;
        public string PipeLinesTreeFilterTooltipText
        {
            get { return _pipeLinesTreeFilterTooltipText; }
            set
            {
                SetProperty(ref _pipeLinesTreeFilterTooltipText, value);
            }
        }
#endregion
    }
    /// <summary>
    /// todo:  возможно стоит перенести в отдельый класс данные которые изменяются внутри  форм
    /// от данных которые заполняются в тулбаре!?
    /// 
    /// GasCosts -  change to Dictionary!? - load data when data is change selected?
    /// 
    /// if (parameters.ThisEnterprise) parameters.EnterpriseId = AppSettingsManager.CurrentEnterpriseId
    /// 
    /// </summary>
    [RegionMemberLifetime(KeepAlive = false), UsedImplicitly]
    public class GasCostsMainViewModel2 : MainViewModelBase
    {
#region constructor
        public GasCostsMainViewModel2()
        {
            IsEditPermission = Authorization2.Inst.IsEditable(LinkType.GasCosts2);
            //
            _calcDialogHelper = new CalcDialogHelper();
            Parameters        = InitInputParameters();
            MainCommands      = InitCommands();
            // 
            _statesViewModel = new StatesViewModel(OnStateSelected);
            var consumptionsParameters = new ConsumptionsParameters(IsEditPermission, OnObjectCellSelected, OnManualInput);
            ConsumptionsViewModel = new ConsumptionsViewModel(consumptionsParameters);
            // 
            var currentCostsParameters = new CostsParameters(IsEditPermission, AddCost, EditCost, DeleteCost);            
            CostsViewModel = new CostsViewModel(currentCostsParameters);
            CostsViewModel.SetAccessAllowed(Parameters.InputState);
            //
            _parameters.SelectedUnit = _parameters.Units.Single(e => e.Coef == 1000);
            SetUnits(_parameters.SelectedUnit.Coef);
            //
            LoadDataAsyncByConstructor();
            LoadListPipeLinesTreeFilters();
            TestCommand = new DelegateCommand(OnPipelineFilterChanged, () => true);
        }
        private async void LoadDataAsyncByConstructor()
        {
            await LoadDataAsync();
        }
        private GasCostParameters InitInputParameters()
        {
            _parameters = new GasCostParameters
            {
                SetUnitAction = SetUnits,
                Units = new List<Unit>
                {
                    new Unit {Coef = 1000, Description = "м³"},
                    new Unit {Coef = 1, Description = "тыс.м³"},
                }
            };

            _parameters.SetEditPermission(IsEditPermission);
            _parameters.PrivateSetSelectedMonth(GetToday());
            //
            _parameters.RadDatePickerAction = async () =>
            {
                ClearInterfaceRightPart();
                await LoadDataAsync();
            };
            _parameters.SelectedSiteIdAction = async () =>
            {
                ClearInterfaceRightPart();
                await LoadDataAsync();
            };
            _parameters.SetInputStateAction = async () =>
            {
                await SetInputState();
                CostsViewModel.SetAccessAllowed(Parameters.InputState);
            };

            return _parameters;
        }
#endregion
#region variables
        private readonly CalcDialogHelper _calcDialogHelper;
        private Dictionary<Guid, CommonEntityDTO> _commonEntityDtos;
        private IEnumerable<PipelineObjectItem> _pipelineObjectItem;
        #endregion
        #region property
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

        private GasCostParameters _parameters;
        public GasCostParameters Parameters
        {
            get { return _parameters; }
            set { SetProperty(ref _parameters, value); }
        }

        private MainCommands _mainCommands;
        public MainCommands MainCommands
        {
            get { return _mainCommands; }
            set { SetProperty(ref _mainCommands, value); }
        }

        private StatesViewModel _statesViewModel;
        public StatesViewModel StatesViewModel
        {
            get { return _statesViewModel; }
            set { SetProperty(ref _statesViewModel, value); }
        }

        private ConsumptionsViewModel _consumptionsViewModel;
        public ConsumptionsViewModel ConsumptionsViewModel
        {
            get { return _consumptionsViewModel; }
            set { SetProperty(ref _consumptionsViewModel, value); }
        }

        private CostsViewModel _costsViewModel;
        public CostsViewModel CostsViewModel
        {
            get { return _costsViewModel; }
            set { SetProperty(ref _costsViewModel, value); }
        }

        private bool _pipelineFilterVisibility;
        public bool PipelineFilterVisibility
        {
            get { return _pipelineFilterVisibility; }
            set { SetProperty(ref _pipelineFilterVisibility, value); }
        }
#endregion
#region commands
        public DelegateCommand TestCommand { get; set; }
        public void AddCost()
        {               
            _calcDialogHelper.Show(
                new GasCostDTO
                {
                    Date     = Parameters.SelectedMonth,      
                    CostType = Parameters.SelectedCostType,   
                    Entity   = GetSelectedCommonEntityDto(ConsumptionsViewModel.SelectedItem.ObjectId), 
                    Target   = Target.Fact, 
                    SiteId   = Parameters.SelectedSiteId.Value, 
                },
            OnCostAddedByPlus, 
            Parameters.DefaultParamValues, true);
        }
        public void EditCost(CostItem selectedCost)
        {
            new CalcDialogHelper().Show(selectedCost.GasCost, OnCostEdited, Parameters.DefaultParamValues, true/*Parameters.ShowDayly*/);            
        }
        public void DeleteCost(CostItem selectedCost)
        {
            if (selectedCost.GasCost.Id == 0) return;
            MessageBoxProvider.Confirm("Удалить расход?", async b =>
            {
                if (!b) return;
                await new GasCostsServiceProxy().DeleteGasCostAsync(selectedCost.GasCost.Id);
                OnCostDeleted(selectedCost.GasCost.Id); 
            }, "Удаление расхода", "Удалить", "Отмена");
        }
        private async Task RefreshCommandExecution()
        {
            ConsumptionsViewModel.Clear();
            CostsViewModel.Clear();
            await LoadDataAsync();
        }
        private void SetDefaultValuesCommandExecuted()
        {
            var vm = new DefaultParamValuesViewModel(null) { DefaultParamValues = Parameters.DefaultParamValues };
            var view = new DefaultParamValuesView { DataContext = vm };
            view.ShowDialog();
        }
        private int CanExecuteQuickInput2()
        {
            var cost = Parameters.SelectedCostType;
            if (Parameters.SelectedCostType == CostType.None || !IsEditPermission) return 0;
            //
            var entityType = Parameters.CostTypeEntityTypeLinkDict[Parameters.SelectedCostType].EntityType;
            if (cost == CostType.CT12)
                return 1;
            if (cost == CostType.CT21)
                return 2;
            if (cost == CostType.CT28)
                return 3;
            return EntityType.Boiler == entityType ? 4 : 0;
        }
        private bool CanExecuteLoadPreviewCosts()
        {
            if (!IsEditPermission) return false;
            if (Parameters.SelectedCostType == CostType.None) return false;
            if (Parameters.CostTypeEntityTypeLinkDict[Parameters.SelectedCostType].IsRegular != 1) return false;
            //
            return true;
        }
        private MainCommands InitCommands()
        {
            _mainCommands = new MainCommands {
#region common
                RefreshCommand = new DelegateCommand(async () => { await RefreshCommandExecution(); }, () => _parameters.SelectedSiteId != null), 
                SetDefaultValuesCommand = new DelegateCommand(SetDefaultValuesCommandExecuted, () => IsEditPermission),
                PipelineFilterCommand = new DelegateCommand(OnPipelineFilterChanged, () => true),
#endregion
#region magic
                QuickInputCommand = new DelegateCommand(QuickInput, () => CanExecuteQuickInput2() > 0),

                //                ImportValveSwitchesCommand = new DelegateCommand(ImportValveSwitches, () => IsEditPermission),
                //                FuelGasInputVolumesCommand = new DelegateCommand(() => FuelGasInputVolume(@params.SelectedMonth, 
                //                                                                                          @params.SelectedSiteId.Value,
                //                                                                                          @params.Coef), () => true),
                //                BoilerAllConsumptionsInputCommand = new DelegateCommand(() =>
                //                {
                //                    BoilerConsumptionsInput(Parameters.SelectedSiteId.Value, 0); //_compStationConsumptionViewModel, //_pipelineConsumptionViewModel, //_distrStationConsumptionViewModel, //_measStationConsumptionViewModel
                //                }, () => IsEditPermission && Parameters.SelectedSiteId.HasValue ),// && IsAccessAllowed()
                //                 BoilerCompStationConsumptionsInputCommand = new DelegateCommand(() =>
                //                 {
                //                     BoilerConsumptionsInput(Parameters.SelectedSiteId.Value, 1);/*_compStationConsumptionViewModel*/
                //                 }, () => IsEditPermission && Parameters.SelectedSiteId.HasValue ),// && IsAccessAllowed()
                //                 BoilerPipelineConsumptionsInputCommand = new DelegateCommand(() =>
                //                 {
                //                     BoilerConsumptionsInput(Parameters.SelectedSiteId.Value, 2);/*_pipelineConsumptionViewModel*/
                //                 }, () => IsEditPermission && Parameters.SelectedSiteId.HasValue ), //  IsAccessAllowed()
                //                BoilerDistrStationConsumptionsInputCommand = new DelegateCommand(() => {
                //                    BoilerConsumptionsInput(Parameters.SelectedSiteId.Value, 3);/*_distrStationConsumptionViewModel*/
                //                }, () => IsEditPermission && Parameters.SelectedSiteId.HasValue ),// && IsAccessAllowed()
                //                BoilerMeasStationConsumptionsInputCommand = new DelegateCommand(() =>
                //                {
                //                    BoilerConsumptionsInput(Parameters.SelectedSiteId.Value, 4);/*_measStationConsumptionViewModel*/
                //                }, 
                //                () => IsEditPermission && Parameters.SelectedSiteId.HasValue),// && IsAccessAllowed()
                #endregion
#region load_preview
                // для выбранной статьи
                PreviewStateCopyCommand = new DelegateCommand(() => LoadPreviousDayDataForState(Parameters.SelectedCostType), 
                                                              CanExecuteLoadPreviewCosts),

//                LoadPreviousDayDataAllTabsCommand  = new DelegateCommand(() => LoadPreviousDayDataAll(0), () => IsEditPermission && Parameters.SelectedSiteId.HasValue),
//                LoadPreviousDayDataKcCommand       = new DelegateCommand(() => LoadPreviousDayDataAll(1), () => IsEditPermission && Parameters.SelectedSiteId.HasValue),
//                LoadPreviousDayDataPipelineCommand = new DelegateCommand(() => LoadPreviousDayDataAll(2), () => IsEditPermission && Parameters.SelectedSiteId.HasValue),
//                LoadPreviousDayDataGrsCommand      = new DelegateCommand(() => LoadPreviousDayDataAll(3), () => IsEditPermission && Parameters.SelectedSiteId.HasValue),
//                LoadPreviousDayDataGisCommand      = new DelegateCommand(() => LoadPreviousDayDataAll(4), () => IsEditPermission && Parameters.SelectedSiteId.HasValue),
#endregion
            };
            return _mainCommands;
        }
        private void UpdateCommands1()
        {
            MainCommands.QuickInputCommand.RaiseCanExecuteChanged();
            MainCommands.PreviewStateCopyCommand.RaiseCanExecuteChanged();
        }
        #endregion
#region events
        private void OnPipelineFilterChanged()
        {
            UpdatePipelineDataByFilter().ContinueWith(task =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (task.Result == null) return;
                    //
                    OnPropertyChanged(() => Parameters.PipeLinesTreeFilterTooltipText);
                    ConsumptionsViewModel.Clear();
                    ConsumptionsViewModel.UpdateRows(task.Result);
                });
            }, TaskContinuationOptions.None);
        }
        private Task<IEnumerable<EntityRowBase>> UpdatePipelineDataByFilter()
        {
            return Task<IEnumerable<EntityRowBase>>.Factory.StartNew(() =>
            {
                StoreListPipeLinesTreeFilters();
                Parameters.PipeLinesTreeFilterTooltipText = GetPipeLinesTreeFilterTooltipText();
                //
                if (StatesViewModel.SelectedState == null || StatesViewModel.SelectedState.TabNum != 2) return null;
                //
                var filteredObjects = PipelineFilter(_pipelineObjectItem);
                return filteredObjects;
            });
        }
        private async void OnStateSelected(StateItem stateItem)
        {
            PipelineFilterVisibility = stateItem != null && stateItem.TabNum == 2;
            if (stateItem == null || stateItem.CostType == CostType.None)
            {
                ClearInterfaceRightPart();
                Parameters.SelectedCostType = CostType.None;
                UpdateCommands1();
                return;
            }
            var stateObjects = (await LoadStateObjects(stateItem.CostType)).ToList();
            await Task.Factory.StartNew(() => FillCosts(stateObjects, stateItem.CostType));
            // 
            ClearInterfaceRightPart();
            ConsumptionsViewModel.UpdateRows(stateObjects);
            Parameters.SelectedCostType = stateItem.CostType;
            UpdateCommands1();
        }
        private void OnObjectCellSelected(EntityRowBase entityRowBase)
        {
            if (entityRowBase == null)
            {
                if (StatesViewModel.SelectedState == null) return;
                CostsViewModel.UpdateAddCommandCanExecute(StatesViewModel.SelectedState.Regular, false);
                return;
            }
            //
            var regular = StatesViewModel.SelectedState.Regular;
            CostsViewModel.UpdateCosts(entityRowBase, Parameters.SelectedMonth);
            CostsViewModel.UpdateAddCommandCanExecute(regular, entityRowBase != null);
            CostsViewModel.UpdateChart(Parameters.SelectedMonth);
            ConsumptionsViewModel.SetCellManualInputAccess(Parameters.InputState == ManualInputState.Input,
                                                           regular, CostsViewModel.DayCosts.Count);
        }
        private void OnCostAddedByPlus(GasCostDTO cost)
        {
            OnCostAdded(cost.Id, true);
        }
        private async void OnCostAdded(int costId, bool plus = false)
        {
            var costSaved = await GetCostSaved(costId);
            var selectedRow = plus ? (ObjectItem)ConsumptionsViewModel.SelectedItem : 
                                     (ObjectItem)ConsumptionsViewModel.GetCapturedItem();
            selectedRow.AddCost(costSaved, Parameters.SelectedMonth);
            selectedRow.UpdateAll();
            CostsViewModel.UpdateCosts(ConsumptionsViewModel.SelectedItem, Parameters.SelectedMonth);
            StatesViewModel.Update(ConsumptionsViewModel.Rows.ToList());
            Parameters.GasCostsForMonth.Add(costSaved);
            CostsViewModel.UpdateAddCommandCanExecute(StatesViewModel.SelectedState.Regular, 
                                                      ConsumptionsViewModel.SelectedItem != null);
            CostsViewModel.UpdateChart(Parameters.SelectedMonth);
        }
        /// <summary> редактирование через форму </summary>
        /// <param name="cost"></param>
        private void OnCostEdited(GasCostDTO cost)
        {
            OnCostEdited(cost, ConsumptionsViewModel.SelectedItem);
        }
        /// <summary>
        /// редактирование через 
        ///                         форму 
        ///                         ручной ввод
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="capturedEntityRowBase"></param>
        private async void OnCostEdited(GasCostDTO cost, EntityRowBase capturedEntityRowBase)
        {
            var costSaved = await GetCostSaved(cost.Id);
            cost.ChangeDate = costSaved?.ChangeDate;
            cost.ChangeUserName = costSaved?.ChangeUserName;
            cost.ChangeUserSiteName = costSaved?.ChangeUserSiteName;
            // update Consumption
            var capturedSelectedRow = (ObjectItem)capturedEntityRowBase;
            capturedSelectedRow.RemoveCost(cost.Id);
            capturedSelectedRow.AddCost(cost, Parameters.SelectedMonth);
            capturedSelectedRow.UpdateAll();
            // update costs            - отобразить выделенный объект
            CostsViewModel.UpdateCosts(ConsumptionsViewModel.SelectedItem, Parameters.SelectedMonth);
            // update summary
            StatesViewModel.Update(ConsumptionsViewModel.Rows.ToList());
            //
            var existingCost = Parameters.GasCostsForMonth.Single(e => e.Id == cost.Id);
            Parameters.GasCostsForMonth.Remove(existingCost);
            Parameters.GasCostsForMonth.Add(cost);
        }
        private async Task<GasCostDTO> GetCostSaved(int costId)
        {
            var dataSaved = await new GasCostsServiceProxy().GetGasCostListAsync(
                new GetGasCostListParameterSet
                {
                    StartDate = Parameters.SelectedMonth.ToLocal().MonthStart(),
                    EndDate = Parameters.SelectedMonth.ToLocal().MonthEnd(),
                    SiteId = Parameters.SelectedSiteId
            });
            var costSaved = dataSaved.SingleOrDefault(item => item.Id == costId);
            return costSaved;
        }
        private void OnCostDeleted(int id)
        {
            CostsViewModel.RemoveCost(id);
            ConsumptionsViewModel.UpdateRow();
            StatesViewModel.Update(ConsumptionsViewModel.Rows.ToList());
            var cost = Parameters.GasCostsForMonth.Single(e => e.Id == id);
            Parameters.GasCostsForMonth.Remove(cost);
            CostsViewModel.UpdateAddCommandCanExecute(StatesViewModel.SelectedState.Regular,
                                                      ConsumptionsViewModel.SelectedItem != null);
            CostsViewModel.UpdateChart(Parameters.SelectedMonth);
        }
        private async void OnManualInput(EntityRowBase entityRowBase, double measured)
        {
            if (((ObjectItem)entityRowBase).GetDayCosts().Count == 0)
            {
                // добавление нового объекта
                var addGasCostParameterSet = new AddGasCostParameterSet
                {
                    CalculatedVolume = null,
                    MeasuredVolume   = measured, 
                    Date             = Parameters.SelectedMonth,
                    EntityId         = entityRowBase.ObjectId,
                    CostType         = Parameters.SelectedCostType,
                    Target           = Target.Fact,
                    InputData        = null,
                    SiteId           = Parameters.SelectedSiteId.Value
                };
                var id = await new GasCostsServiceProxy().AddGasCostAsync(addGasCostParameterSet);
                OnCostAdded(id);
            }
            else
            {
                var objectItem = (ObjectItem) entityRowBase;
                var cost = objectItem.GetDayCosts().Single();
                cost.MeasuredVolume = measured;
                var costParameterSet = new EditGasCostParameterSet
                {
                    MeasuredVolume   = measured,
                    Date             = Parameters.SelectedMonth,
                    CostId           = cost.Id,
                    Target           = cost.Target,
                    SiteId           = cost.SiteId,
                    InputData        = cost.InputString,
                    CostType         = cost.CostType,
                    CalculatedVolume = cost.CalculatedVolume,
                    EntityId         = cost.Entity.Id,
                    ImportId         = cost.ImportId,
                    SeriesId         = cost.SeriesId
                };
                await new GasCostsServiceProxy().EditGasCostAsync(costParameterSet);
                OnCostEdited(cost, entityRowBase);
            }
        }
#endregion
#region methods
#region main_load
        private async Task LoadDataAsync()
        {
            Behavior.TryLock();

            var selectedSite = Parameters.SelectedSiteId;
            var loadSitesTask = LoadSites(Parameters);
            var costTypeEntityTypeLinkListTask = new GasCostsServiceProxy().GetCostTypeListAsync();
            await TaskEx.WhenAll(loadSitesTask, costTypeEntityTypeLinkListTask);
            Parameters.Sites = loadSitesTask.Result;
            SetSiteId(selectedSite);
            Parameters.CostTypeEntityTypeLinkList = costTypeEntityTypeLinkListTask.Result;

            var stateBuilder = new StateBuilder();
            var accessListTask = GetAccessList(Parameters);
            // Dayly - Fact, Monthly - Fact, Plan, Norm
            var gasCostsForMonthTask = LoadGasCosts(Parameters.SelectedSiteId,
                                                             Parameters.SelectedMonth.ToLocal().MonthStart(),
                                                             Parameters.SelectedMonth.ToLocal());

            var listTask = new GasCostsServiceProxy().GetDefaultParamValuesAsync(new GetGasCostListParameterSet
            {
                StartDate = Parameters.SelectedMonth.ToLocal().MonthStart(),
                EndDate = Parameters.SelectedMonth.ToLocal().MonthEnd(),
                SiteId = Parameters.SelectedSiteId
            });

            var rootTask = stateBuilder.BuildStatesTree(Parameters.CostTypeEntityTypeLinkList, 
                                                          ClientCache.DictionaryRepository.GasCostItemGroups, 
                                                          false,
                                                          Parameters.SelectedSiteId);

            await TaskEx.WhenAll(accessListTask, gasCostsForMonthTask, listTask, rootTask);

            Parameters.AccessList = accessListTask.Result;
            Parameters.GasCostsForMonth = gasCostsForMonthTask.Result;
            var list = listTask.Result;

            LoadDefaultParamValues(list);

            StatesViewModel.BuildStates(rootTask.Result, Parameters.GasCostsForMonth, Parameters.SelectedMonth);
            StatesViewModel.Update();
            StatesViewModel.ClearSelection();
            MainCommands.RefreshCommand.RaiseCanExecuteChanged();

            Behavior.TryUnlock();
        }
        private static async Task<List<SiteDTO>> LoadSites(GasCostParameters gasCostParameters)
        {
            var siteListParameterSet = new GetSiteListParameterSet
            {
                EnterpriseId = UserProfile.Current.Site.IsEnterprise ? UserProfile.Current.Site.Id : (Guid?)null
            };
            var siteList = await new ObjectModelServiceProxy().GetSiteListAsync(siteListParameterSet).ConfigureAwait(false);
            if (UserProfile.Current.Site.IsEnterprise) gasCostParameters.Sites = siteList;
            else
            {
                var site = siteList.Single(s => s.Id == UserProfile.Current.Site.Id);
                siteList = siteList.Where(s => s.Id == site.Id || site.DependantSiteIdList.Contains(s.Id)).ToList();
            }
            return siteList;
        }
        private void SetSiteId(Guid? selectedSite)
        {
            if (UserProfile.Current.Site.IsEnterprise)
            {
                if (!UserProfile.Current.Site.IsEnterprise) return;
                //
                if (selectedSite == null) Parameters.PrivateSetSelectedSiteId(Parameters.Sites.First().Id);
                else
                {
                    var firstOrDefault = Parameters.Sites.FirstOrDefault(e => e.Id == selectedSite.Value);
                    Parameters.PrivateSetSelectedSiteId(firstOrDefault?.Id ?? Parameters.Sites.First().Id);
                }
            }
            else
            {
                Parameters.PrivateSetSelectedSiteId(UserProfile.Current.Site.Id);
            }
        }
        private async Task<List<GasCostAccessDTO>> GetAccessList(GasCostParameters gasCostParameters)
        {
            if (true)
            {
                var proxy = new GasCostsServiceProxy();
                var stateList = await proxy.GetGasCostAccessListAsync(UserProfile.Current.Site.IsEnterprise ?
                        new GetGasCostAccessListParameterSet
                        {
                            Date = gasCostParameters.SelectedMonth,
                            EnterpriseId = UserProfile.Current.Site.Id,
                            SiteId = gasCostParameters.SelectedSiteId,
                            PeriodType = DTO.Dictionaries.PeriodTypes.PeriodType.Day
                        }
                        :
                        new GetGasCostAccessListParameterSet
                        {
                            Date = gasCostParameters.SelectedMonth,
                            SiteId = gasCostParameters.SelectedSiteId,
                            PeriodType = DTO.Dictionaries.PeriodTypes.PeriodType.Day
                        });
                var state = stateList.FirstOrDefault();
                if (state?.Date == gasCostParameters.SelectedMonth)
                {
                    gasCostParameters.InputState = state?.Fact ?? true ? ManualInputState.Input : 
                                                                       ManualInputState.Approved;
                    gasCostParameters.InputStateInfo = state?.ChangeUser;
                    OnPropertyChanged(() => gasCostParameters.InputStateInfo);                    
                }
            }
            List<GasCostAccessDTO> accessList;
            if (UserProfile.Current.Site.IsEnterprise)
            {
                accessList =
                    await new GasCostsServiceProxy().GetGasCostAccessListAsync(
                        new GetGasCostAccessListParameterSet
                        {
                            Date = gasCostParameters.SelectedMonth.MonthStart(),
                            EnterpriseId = UserProfile.Current.Site.Id,
                            PeriodType = DTO.Dictionaries.PeriodTypes.PeriodType.Month
                        });
            }
            else
            {
                accessList =
                    await new GasCostsServiceProxy().GetGasCostAccessListAsync(
                        new GetGasCostAccessListParameterSet
                        {
                            Date = gasCostParameters.SelectedMonth.MonthStart(),
                            SiteId = UserProfile.Current.Site.Id,
                            PeriodType = DTO.Dictionaries.PeriodTypes.PeriodType.Month
                        });
            }
            return accessList;
        }
        private static async Task<List<GasCostDTO>> LoadGasCosts(Guid? siteId, DateTime start, 
                                                                 DateTime end, Target? target = null)
        {
            var parameter = new GetGasCostListParameterSet
            {
                StartDate = start,
                EndDate   = end,
                SiteId    = siteId
            };
            if (target != null) parameter.Target = target.Value;
            var gasCosts = await new GasCostsServiceProxy().GetGasCostListAsync(parameter).ConfigureAwait(false);
            return gasCosts;
        }
        private void LoadDefaultParamValues(List<DefaultParamValuesDTO> list)
        {
            Parameters.DefaultParamValues = new List<DefaultParamValues>
            {
                new DefaultParamValues(
                    list.SingleOrDefault(d => d.Target == Target.Norm) ??
                    new DefaultParamValuesDTO
                    {
                        Target = Target.Norm,
                        Period = Parameters.SelectedMonth,
                        SiteId = Parameters.SelectedSiteId.Value
                    }),
                new DefaultParamValues(
                    list.SingleOrDefault(d => d.Target == Target.Plan) ??
                    new DefaultParamValuesDTO
                    {
                        Target = Target.Plan,
                        Period = Parameters.SelectedMonth,
                        SiteId = Parameters.SelectedSiteId.Value
                    }),
                new DefaultParamValues(
                    list.SingleOrDefault(d => d.Target == Target.Fact) ??
                    new DefaultParamValuesDTO
                    {
                        Target = Target.Fact,
                        Period = Parameters.SelectedMonth,
                        SiteId = Parameters.SelectedSiteId.Value
                    })
            };
        }
        private void ClearInterfaceRightPart()
        {            
            ConsumptionsViewModel.Clear();
            CostsViewModel.Clear();
        }
#endregion
#region costs
        private Task<IEnumerable<ObjectItem>> LoadStateObjects(CostType costType)
        {
            if (Parameters.SelectedSiteId == null)
                throw new NullReferenceException("Parameters.SelectedSiteId == null");
            //
            var selectedSiteId = Parameters.SelectedSiteId.Value;
            var costTypeDTO    = Parameters.CostTypeEntityTypeLinkDict[costType];
            return GetObjects(costTypeDTO.EntityType, selectedSiteId, StatesViewModel.SelectedState.TabNum);
        }

        private async Task<IEnumerable<ObjectItem>> GetObjects(EntityType entityType, Guid siteId, int tabNum)
        {
            var dataProvider = new ObjectModelServiceProxy();
            switch (entityType)
            {
                case EntityType.CompShop:
                    var compShops = await dataProvider.GetCompShopListAsync(new GetCompShopListParameterSet {SiteId = siteId}).ConfigureAwait(false);
                    return CastEntityDtoToObjectItem(compShops);
            
                case EntityType.CompUnit:
                    var compUnits = await dataProvider.GetCompUnitListAsync(new GetCompUnitListParameterSet {SiteId = siteId}).ConfigureAwait(false);
                    return CastEntityDtoToObjectItem(compUnits);
            
                case EntityType.PowerUnit:
                    var powerUnits = await GetPowerUnits(siteId, tabNum);
                    var castPowerUnits = CastEntityDtoToObjectItem(powerUnits);
                    return castPowerUnits;
            
                case EntityType.Boiler:
                    var boilers = await GetBoilers(siteId, tabNum);
                    var castBoilers = CastEntityDtoToObjectItem(boilers);
                    return castBoilers;
            
                case EntityType.CoolingUnit:
                    var coolingUnits = await dataProvider.GetCoolingUnitListAsync(new GetCoolingUnitListParameterSet { SiteId = siteId}).ConfigureAwait(false);
                    var castCoolingUnits = CastEntityDtoToObjectItem(coolingUnits);
                    return castCoolingUnits;
            
                case EntityType.Pipeline:
                    var pipelines = await dataProvider.GetPipelineListAsync(new GetPipelineListParameterSet{SiteId = siteId}).ConfigureAwait(false);
                    var castPipeline = CastEntityDtoToPipelineObjectItem(pipelines);
                    return castPipeline;
            
                case EntityType.ReducingStation:
                    var reducingStation = await dataProvider.GetReducingStationListAsync(new GetReducingStationListParameterSet {SiteId = siteId}).ConfigureAwait(false);
                    return CastEntityDtoToObjectItem(reducingStation);
            
                case EntityType.DistrStation:
                    var distrStation = await dataProvider.GetDistrStationListAsync(new GetDistrStationListParameterSet { SiteId = siteId }).ConfigureAwait(false);
                    return CastEntityDtoToObjectItem(distrStation);
            
                case EntityType.MeasStation:
                    var measStations = await dataProvider.GetMeasStationListAsync(new GetMeasStationListParameterSet{ SiteId = siteId }).ConfigureAwait(false);
                    return CastEntityDtoToObjectItem(measStations);
            
                default:
                    throw new ArgumentOutOfRangeException(nameof(entityType));
            }
        }
        private static async Task<IEnumerable<BoilerDTO>> GetBoilers(Guid siteId, int tabNum)
        {
            var filter = GetObjectParentEntity(tabNum);
            var dataProvider = new ObjectModelServiceProxy();
            var boilers = await dataProvider.GetBoilerListAsync(new GetBoilerListParameterSet { SiteId = siteId }).ConfigureAwait(false);

            return filter == null ? 
                boilers : 
                boilers.Where(e => e.ParentEntityType == filter).ToList();
        }
        private static async Task<IEnumerable<PowerUnitDTO>> GetPowerUnits(Guid siteId, int tabNum)
        {
            var filter = GetObjectParentEntity(tabNum);
            var dataProvider = new ObjectModelServiceProxy();
            var powerUnits = await dataProvider.GetPowerUnitListAsync(new GetPowerUnitListParameterSet { SiteId = siteId }).ConfigureAwait(false);
            if (tabNum == 1)
            {
                return filter == null ?
                    powerUnits :
                    powerUnits.Where(e => e.ParentEntityType == EntityType.CompStation).ToList();
            }
            return filter == null ?
                powerUnits :
                powerUnits.Where(e => e.ParentEntityType == filter).ToList();
        }

        public IEnumerable<ObjectItem> CastEntityDtoToObjectItem(IEnumerable<CommonEntityDTO> objects)
        {
            SetSelectedCommonEntitiesDtos(objects);
            var objectItems = _commonEntityDtos.Values.Select(e => new ObjectItem(e.Id)
            {
                EntityType = e.EntityType,
                Name = e.ShortPath,
            });
            return objectItems;
        }
        public IEnumerable<ObjectItem> CastEntityDtoToPipelineObjectItem(IEnumerable<PipelineDTO> objects)
        {
            var objectsList = objects.ToList();
            SetSelectedCommonEntitiesDtos(objectsList);
            _pipelineObjectItem = objectsList.ToList().Select(e => new PipelineObjectItem(e.Id)
            {
                PipelineType = e.Type,
                EntityType = e.EntityType,
                Name = e.ShortPath,
            });
            var filteringObjects = PipelineFilter(_pipelineObjectItem);
            return filteringObjects.Cast<ObjectItem>();
        }
        public Task<IEnumerable<ObjectItem>> CastEntityDtoToPipelineObjectItem2(IEnumerable<PipelineDTO> objects)
        {
            return
            Task<IEnumerable<ObjectItem>>.Factory.StartNew(() =>
            {
                var objectsList = objects.ToList();
                SetSelectedCommonEntitiesDtos(objectsList);
                _pipelineObjectItem = objectsList.ToList().Select(e => new PipelineObjectItem(e.Id)
                {
                    PipelineType = e.Type,
                    EntityType   = e.EntityType,
                    Name         = e.ShortPath,
                });
                var filteringObjects = PipelineFilter(_pipelineObjectItem);
                return filteringObjects.Cast<ObjectItem>();
            });
        }
        private void SetSelectedCommonEntitiesDtos(IEnumerable<CommonEntityDTO> selectedCommonEntitiesDtos)
        {
            _commonEntityDtos = selectedCommonEntitiesDtos.ToDictionary(k => k.Id, v => v);
        }
        private CommonEntityDTO GetSelectedCommonEntityDto(Guid id)
        {
            return _commonEntityDtos[id];
        }
        public void FillCosts(IEnumerable<ObjectItem> costObjects, CostType costType)
        {
            var gasCosts = Parameters.GasCostsForMonth.Where(e => e.CostType == costType);
            var costObjectsDictionary = costObjects.ToDictionary(k => k.ObjectId, v => v);
            var listObjectItems = new List<ObjectItem>();
            foreach (var cost in gasCosts)
            {
                if (!costObjectsDictionary.ContainsKey(cost.Entity.Id)) continue;
                // 
                var costObject = costObjectsDictionary[cost.Entity.Id];
                costObject.AddCost(cost, Parameters.SelectedMonth);
                listObjectItems.Add(costObject);
            }
            listObjectItems.ForEach(e=>e.UpdateAll());
        }
        public void LoadListPipeLinesTreeFilters()
        {
            Parameters.PipeLinesTreeFilters.Clear();
            foreach (var p in ClientCache.DictionaryRepository.PipelineTypes.Values.OrderBy(c => c.SortOrder).ToArray())
            {
                var nf = new PipelineTypeForFilter { DisplayName = p.Name, Value = p.PipelineType };
                if (nf.Value != PipelineType.Main &&
                    nf.Value != PipelineType.Distribution &&
                    nf.Value != PipelineType.Looping &&
                    nf.Value != PipelineType.Inlet &&
                    nf.Value != PipelineType.CompressorShopBridge)
                    Parameters.PipeLinesTreeFilters.Add(new PipelineTypeForFilter
                    {
                        DisplayName = p.Name,
                        Value = p.PipelineType
                    });
            }
            var storage = IsolatedStorageManager.PipelineTypesForPipelineConsumption;
            if (storage == null) return;
            //
            foreach (var p in Parameters.PipeLinesTreeFilters)
                if (storage.Contains(p.Value))
                    p.IsSelected = true;
        }
        private IEnumerable<EntityRowBase> PipelineFilter(IEnumerable<PipelineObjectItem> pipelinesObjects)
        {
            var excludingTypes = Parameters.PipeLinesTreeFilters.Where(e=>!e.IsSelected)
                                                                .Select(e=>e.Value)
                                                                .ToLookup(e=>e);
            var filteringRows = pipelinesObjects.Where(e=>!excludingTypes.Contains(e.PipelineType));
            return filteringRows; 
        }
        private void StoreListPipeLinesTreeFilters()
        {
            IsolatedStorageManager.PipelineTypesForPipelineConsumption = 
                Parameters.PipeLinesTreeFilters.Where(o=>o.IsSelected)
                                               .Select(e=>e.Value)
                                               .ToArray();
        }
        private string GetPipeLinesTreeFilterTooltipText()
        {
            return string.Join("\n", Parameters.PipeLinesTreeFilters.Where(o => o.IsSelected).Select(x => x.DisplayName).ToArray());
        }
#endregion
#region input_state
        private async Task SetInputState()
        {
            try
            {
                Behavior.TryLock();
                var v = new GasCostAccessDTO
                {
                    Date       = Parameters.SelectedMonth,
                    SiteId     = Parameters.SelectedSiteId.Value,
                    Fact       = Parameters.InputState == ManualInputState.Input,
                    Plan       = true,
                    Norm       = true,
                    PeriodType = DTO.Dictionaries.PeriodTypes.PeriodType.Day
                };
                await new GasCostsServiceProxy().UpdateGasCostAccessListAsync(new List<GasCostAccessDTO> {v}).ConfigureAwait(false);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
#endregion
#region units
        private void SetUnits(int units)
        {
            IsolatedStorageManager.Set("VolumeInputUnits", units);
#region viewModels
            CalcViewModelBase<UnitFuelCostsModel>.Coef =
            CalcViewModelBase<CoolingCostsModel>.Coef =
            CalcViewModelBase<EnergyGenerationCostsModel>.Coef =
            CalcViewModelBase<TreatingShopHeatingCostsModel>.Coef =
            CalcViewModelBase<FuelGasHeatingCostModel>.Coef =
            CalcViewModelBase<UnitStartCostsModel>.Coef =
            CalcViewModelBase<UnitStopCostsModel>.Coef =
            CalcViewModelBase<ValveControlCostsModel>.Coef = //?
            CalcViewModelBase<ShutdownCostsModel>.Coef =
            CalcViewModelBase<PurgeCostsModel>.Coef =
            CalcViewModelBase<UnitBleedingCostsModel>.Coef =
            CalcViewModelBase<ChemicalAnalysisCostsModel>.Coef =
            CalcViewModelBase<CompStationLossModel>.Coef =
            CalcViewModelBase<ControlEquipmentCostsModel>.Coef =
            CalcViewModelBase<ThermalDisposalUnitCostsModel>.Coef =
            CalcViewModelBase<CompUnitsTestingCostsModel>.Coef =
            CalcViewModelBase<CompUnitsHeatingCostsModel>.Coef =
            CalcViewModelBase<DiaphragmReplacementCostsModel>.Coef =
            CalcViewModelBase<PneumaticExploitationCostsModel>.Coef =
            CalcViewModelBase<ValveExploitationCostsModel>.Coef =
            CalcViewModelBase<RepairCostsModel>.Coef =
            CalcViewModelBase<CleaningCostsModel>.Coef =
            CalcViewModelBase<MethanolFillingCostsModel>.Coef =
            CalcViewModelBase<HydrateRemoveCostsModel>.Coef =
            CalcViewModelBase<HeatingCostsModel>.Coef =
            CalcViewModelBase<ReducingStationOwnNeedsCostsModel>.Coef =
            CalcViewModelBase<FluidControllerCostsModel>.Coef =
            CalcViewModelBase<HeaterWorkCostsModel>.Coef =
            CalcViewModelBase<PopValveTuningCostsModel>.Coef =
            CalcViewModelBase<PipelineLossModel>.Coef =
            CalcViewModelBase<KptgOwnNeedsCostsModel>.Coef =
            CalcViewModelBase<ReservePowerStationMaintenanceCostsModel>.Coef = units;
            CalcViewModelBase<PipelineLossModel>.Coef = units;
#endregion
            StatesViewModel.Units       = units;
            ConsumptionsViewModel.Units = units;
            CostsViewModel.Units        = units;
        }
#endregion
#region magic_wand
        private void QuickInput()
        {
            var i = CanExecuteQuickInput2();
            switch (i)
            {
                case 0: return;
                case 1:
                    FuelGasInputVolume(Parameters.SelectedMonth, Parameters.SelectedSiteId.Value, Parameters.SelectedUnit.Coef);
                    return;
                case 2:
                    ImportValveSwitches();
                    return;
                case 3:
                    ImportValveSwitches();
                    return;
                case 4:
                    BoilerConsumptionsInput(Parameters.SelectedSiteId.Value, StatesViewModel.SelectedState.TabNum);
                    return;
            }
        }
        private void ImportValveSwitches()
        {
            var vm = new ImportValveSwitchesViewModel(() => { }, Parameters.SelectedMonth, Parameters.SelectedSiteId.Value);
            var v = new ImportValveSwitchesView { DataContext = vm };
            v.Closed += (s, a) => { if (v.DialogResult == true) RefreshCommandExecution(); };
            v.ShowDialog();
        }
        private void FuelGasInputVolume(DateTime selectedMonth, Guid selectedSiteId, int coef)
        {
            var vm = new FuelGasInputVolumeViewModel(() => { }, selectedMonth, selectedSiteId, coef);
            var v = new FuelGasInputVolumeView { DataContext = vm };
            v.Closed += async (s, a) =>
            {
                if (v.DialogResult == true)
                    await LoadDataAsync();
            };
            v.ShowDialog();
        }
        private async void BoilerConsumptionsInput(Guid siteId, int tabNum)
        {
            var previewCosts = LoadPrewiewCosts();
            var boilers      = GetBoilers(siteId, tabNum);
            await TaskEx.WhenAll(previewCosts, boilers);
            var costTypes = Parameters.CostTypeEntityTypeLinkList.Where(e => e.EntityType == EntityType.Boiler)
                                                                 .ToList();
            var boilersByTab = boilers.Result.ToLookup(k => GetBoilerCostTypeByParent(costTypes, k.ParentEntityType), 
                                                       val => new BoilerExtension {BoilerDTO = val});
            var dataProvider  = new ObjectModelServiceProxy();
            var compStation   = new Dictionary<Guid, CompStationDTO>();
            var boilerPlant   = new Dictionary<Guid, BoilerPlantDTO>();
            var pipelines     = new Dictionary<Guid, PipelineDTO>();
            var distrStations = new Dictionary<Guid, DistrStationDTO>();
            var measStation   = new Dictionary<Guid, MeasStationDTO>();
            var systemId = Parameters.Sites.Single(e=>e.Id == Parameters.SelectedSiteId).SystemId;
            if (tabNum == 0)
            {
                var compStationDto   = dataProvider.GetCompStationListAsync(new GetCompStationListParameterSet {SiteId = siteId});
                var boilerPlantDto   = dataProvider.GetBoilerPlantListAsync(systemId);
                var pipelinesDto     = dataProvider.GetPipelineListAsync(new GetPipelineListParameterSet { SiteId = siteId});
                var distrStationsDto = dataProvider.GetDistrStationListAsync(new GetDistrStationListParameterSet { SiteId = siteId });
                var measStationDto   = dataProvider.GetMeasStationListAsync(new GetMeasStationListParameterSet { SiteId = siteId });

                await TaskEx.WhenAll(compStationDto, pipelinesDto, distrStationsDto, measStationDto, boilerPlantDto);

                compStation   = compStationDto.Result.ToDictionary(k => k.Id);
                boilerPlant   = boilerPlantDto.Result.ToDictionary(k => k.Id);                
                pipelines     = pipelinesDto.Result.ToDictionary(k => k.Id);
                distrStations = distrStationsDto.Result.ToDictionary(k=>k.Id);
                measStation   = measStationDto.Result.ToDictionary(k => k.Id);
            }
            else if (tabNum == 1)
            {
                compStation = (await dataProvider.GetCompStationListAsync(new GetCompStationListParameterSet { SiteId = siteId }))
                    .ToDictionary(k => k.Id);
                boilerPlant = (await dataProvider.GetBoilerPlantListAsync(systemId))
                    .ToDictionary(k => k.Id);
            }
            else if (tabNum == 2)
            {
                pipelines = (await dataProvider.GetPipelineListAsync(new GetPipelineListParameterSet { SiteId = siteId }))
                    .ToDictionary(k => k.Id);
            }
            else if (tabNum == 3)
            {
                distrStations = (await dataProvider.GetDistrStationListAsync(new GetDistrStationListParameterSet { SiteId = siteId }))
                    .ToDictionary(k => k.Id);
            }
            else if (tabNum == 4)
            {
                measStation = (await dataProvider.GetMeasStationListAsync(new GetMeasStationListParameterSet { SiteId = siteId }))
                    .ToDictionary(k => k.Id);
            }
            boilersByTab.ForEach(costBoiler => 
                costBoiler.ToList().ForEach(boiler => {
                    
                    if (boiler.BoilerDTO.ParentEntityType == EntityType.BoilerPlant)
                    {
                        var plantDTO = boilerPlant[boiler.BoilerDTO.ParentId.Value];
                        var cs = compStation[plantDTO.ParentId.Value];
                        boiler.OwnerName = cs.Name;
                        boiler.PathName = GetBoilerName(plantDTO.Name, boiler.BoilerDTO.Name);
                        return;
                    }
                    if (boiler.BoilerDTO.ParentEntityType == EntityType.Pipeline) { 
                        boiler.OwnerName = pipelines[boiler.BoilerDTO.ParentId.Value].Name;
                        boiler.PathName = GetBoilerName(boiler.OwnerName, boiler.BoilerDTO.Name);
                        return;
                    }
                    if (boiler.BoilerDTO.ParentEntityType == EntityType.DistrStation) {
                        boiler.OwnerName = distrStations[boiler.BoilerDTO.ParentId.Value].Name;
                        boiler.PathName = GetBoilerName(boiler.OwnerName, boiler.BoilerDTO.Name);
                        return;
                    }
                    if (boiler.BoilerDTO.ParentEntityType == EntityType.MeasStation) {
                        boiler.OwnerName = measStation[boiler.BoilerDTO.ParentId.Value].Name;
                        boiler.PathName = GetBoilerName(boiler.OwnerName, boiler.BoilerDTO.Name);
                        return;
                    }
                }));
            var vm = new BoilerConsumptionsViewModel2(() => {},
                                                      previewCosts.Result,
                                                      Parameters.SelectedMonth, 
                                                      Parameters.SelectedSiteId.Value, 
                                                      Parameters.DefaultParamValues, 
                                                      Parameters.SelectedUnit.Coef,
                                                      boilersByTab);
            var v = new BoilerConsumptionsView { DataContext = vm };
            v.Closed += (s, a) => { if (v.DialogResult == true) RefreshCommandExecution(); };
            v.ShowDialog();
        }
        private string GetBoilerName(string plantName, string boilerName)
        {
            return string.IsNullOrEmpty(plantName) ? boilerName :             
                                                     $"{plantName} / {boilerName}";
        }
        private static CostType GetBoilerCostTypeByParent(IEnumerable<GasCostTypeDTO> costTypeDtos,
                                                          EntityType parentEntityType)
        {
            var tab = GetBoilerParentEntityInverse(parentEntityType);
            return costTypeDtos.Single(e=>e.TubNum == tab).CostType;
        }
        private static int GetBoilerParentEntityInverse(EntityType entityType)
        {
            switch (entityType) {
                case EntityType.BoilerPlant:  return 1;
                case EntityType.Pipeline:     return 2;
                case EntityType.DistrStation: return 3;
                case EntityType.MeasStation:  return 4;
            }
            return 0;
        }
#endregion
#region load_preview
        /// <summary>
        /// 
        /// 1. копирование доступно только для регулярных объектов
        /// 2. если объект имеет кост - перезапись осуществляется по 
        ///    сообщение  вызов сообщения! прри совпадающем значении
        ///  
        /// Object -> one cost
        /// 
        /// </summary>
        /// <param name="costType"></param>
        private async void LoadPreviousDayDataForState(CostType costType)
        {
            var previewCosts = await LoadPrewiewCosts();
            var objDict = ConsumptionsViewModel.Rows.ToDictionary(k => k.ObjectId);
            var filteredCosts = previewCosts.Where(e => e.CostType == costType && objDict.ContainsKey(e.Entity.Id)).ToList();
            var objCostsDict = Parameters.GasCostsForMonth.Where(e => e.CostType == costType && e.Date == Parameters.SelectedMonth)
                                                             .ToLookup(e => e.Entity.Id);
            var executeWithUserConfirmation = filteredCosts.Where(previewCost => objDict[previewCost.Entity.Id].FactTotalToDate > 0).ToList();
            var executeImmediately = filteredCosts.Except(executeWithUserConfirmation);
            // не требующие подтверждения
            var tasksImmediately = new List<Task>();
            foreach (var gasCostDTO in executeImmediately)
            {
                var t3 = AddPreviewCost(gasCostDTO);
                tasksImmediately.Add(t3);
            }
            await TaskEx.WhenAll(tasksImmediately);
            // требующие подтверждения
            var _g = executeWithUserConfirmation;
            executeWithUserConfirmation.ForEach(previewCost =>
            {
                var obj = objDict[previewCost.Entity.Id];
                MessageBoxProvider.Confirm($"Объект \"{obj.Name}\" \nуже содержит значение расхода. \nПерезаписать существующее значение?",
                   async b => {
                        if (b) {
                            var objectCosts = objCostsDict[previewCost.Entity.Id].ToList();
                            foreach (var objectCost in objectCosts) 
                                await new GasCostsServiceProxy().DeleteGasCostAsync(objectCost.Id);
                            await AddPreviewCost(previewCost);
                        }
                        _g.Remove(previewCost);
                        if (_g.Count == 0)
                            await RefreshWithSelection(StatesViewModel.SelectedState.CostType);
                    });
            });
            if (executeWithUserConfirmation.Count == 0)
                await RefreshWithSelection(StatesViewModel.SelectedState.CostType); 
        }
        private async Task RefreshWithSelection(CostType costType)
        {
            await RefreshCommandExecution();
            StateItem select;
            StatesViewModel.GetStateItem(costType, out select);
            StatesViewModel.SelectedState = select;
        }
        private static async Task AddPreviewCost(GasCostDTO previewCost)
        {
            previewCost.Date = previewCost.Date.Add(new TimeSpan(24, 0, 0));
            var addGasCostParameterSet = new AddGasCostParameterSet
            {
                CalculatedVolume = previewCost.CalculatedVolume,
                MeasuredVolume = previewCost.MeasuredVolume,
                Date = previewCost.Date,
                EntityId = previewCost.Entity.Id,
                CostType = previewCost.CostType,
                Target = previewCost.Target,
                InputData = previewCost.InputString,
                SiteId = previewCost.SiteId
            };
            previewCost.ChangeDate = DateTime.Now;
            previewCost.ChangeUserName = UserProfile.Current.UserName;
            var  id = await new GasCostsServiceProxy().AddGasCostAsync(addGasCostParameterSet);
            previewCost.Id = id;
        }
        private Task<List<GasCostDTO>> LoadPrewiewCosts()
        {
            var startDate = Parameters.SelectedMonth.AddDays(-1);
            var endDate = Parameters.SelectedMonth.AddDays(-1);
            var costs = LoadGasCosts(Parameters.SelectedSiteId, startDate, endDate, Target.Fact);
            return costs;
        }
#endregion
#region helpers
        private static DateTime GetToday()
        {
            return SeriesHelper.GetPastDispDay().ToLocal();
        }
        private static EntityType? GetObjectParentEntity(int tabNum)
        {
            switch (tabNum)
            {
                case 1: return EntityType.BoilerPlant;
                case 2: return EntityType.Pipeline;
                case 3: return EntityType.DistrStation;
                case 4: return EntityType.MeasStation;
                default: return null;
            }
        }
        private static double GetMiliseconds(long before, long after)
        {
            var elapsedTime = new TimeSpan(after - before);
            var totalMilliseconds = elapsedTime.TotalMilliseconds;
            return totalMilliseconds;
        }
#endregion
#endregion
    }
    public class BoilerExtension
    {
        public BoilerDTO BoilerDTO { get; set; }
        public string OwnerName { get; set; }
        public string PathName { get; set; }
    }
}
#region trash
#endregion