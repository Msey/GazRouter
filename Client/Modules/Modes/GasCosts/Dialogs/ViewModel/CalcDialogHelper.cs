using System;
using System.Collections.Generic;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ChemicalAnalysisCosts;
using GazRouter.Modes.GasCosts.Dialogs.CleaningCosts;
using GazRouter.Modes.GasCosts.Dialogs.ControlEquipmentCosts;
using GazRouter.Modes.GasCosts.Dialogs.CoolingCosts;
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
using GazRouter.Modes.GasCosts.Dialogs.UnitFuelCosts;
using GazRouter.Modes.GasCosts.Dialogs.UnitStartCosts;
using GazRouter.Modes.GasCosts.Dialogs.UnitStopCosts;
using GazRouter.Modes.GasCosts.Dialogs.ValveControlCosts;
using GazRouter.Modes.GasCosts.Dialogs.CompStationLoss;
using GazRouter.Modes.GasCosts.Dialogs.CompUnitsHeatingCosts;
using GazRouter.Modes.GasCosts.Dialogs.CompUnitsTestingCosts;
using GazRouter.Modes.GasCosts.Dialogs.DrainageBleedingCosts;
using GazRouter.Modes.GasCosts.Dialogs.FluidControllerCosts;
using GazRouter.Modes.GasCosts.Dialogs.HeaterWorkCosts;
using GazRouter.Modes.GasCosts.Dialogs.IncidentLoss;
using GazRouter.Modes.GasCosts.Dialogs.KptgOwnNeedsCosts;
using GazRouter.Modes.GasCosts.Dialogs.PneumaticExploitationCosts;
using GazRouter.Modes.GasCosts.Dialogs.ReducingStationOwnNeedsCosts;
using GazRouter.Modes.GasCosts.Dialogs.ReservePowerStationMaintenanceCosts;
using GazRouter.Modes.GasCosts.Dialogs.ThermalDisposalUnitCosts;
using GazRouter.Modes.GasCosts.Dialogs.ValveExploitationCosts;
using GazRouter.Modes.GasCosts.Dialogs.VesselsExaminationCosts;
using Telerik.Windows.Controls;
namespace GazRouter.Modes.GasCosts.Dialogs.ViewModel
{
    public class CalcDialogHelper
    {
        public CalcDialogHelper()
        {
            RegisterCalcs();
        }
        
        private readonly 
            Dictionary<CostType, Func<GasCostDTO, Action<GasCostDTO>, List<DefaultParamValues>, bool, RadWindow>> 
            _calcDictionary = new Dictionary<CostType, Func<GasCostDTO, Action<GasCostDTO>, 
                                             List<DefaultParamValues>, bool, RadWindow>>();       

        private void RegisterCalcs()
        {
#region КЦ, КС
            _calcDictionary.Add(CostType.CT12, (e, callback, defaultParamValue, daily) => new UnitFuelCostsView { DataContext = new UnitFuelCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT13, (e, callback, defaultParamValue, daily) => new CoolingCostsView { DataContext = new CoolingCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT14, (e, callback, defaultParamValue, daily) => new HeatingCostsView { DataContext = new HeatingCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT15, (e, callback, defaultParamValue, daily) => new EnergyGenerationCostsView { DataContext = new EnergyGenerationCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT16, (e, callback, defaultParamValue, daily) => new TreatingShopHeatingCostsView { DataContext = new TreatingShopHeatingCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT20, (e, callback, defaultParamValue, daily) => new FuelGasHeatingCostsView { DataContext = new FuelGasHeatingCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT18, (e, callback, defaultParamValue, daily) => new UnitStartCostsView { DataContext = new UnitStartCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT19, (e, callback, defaultParamValue, daily) => new UnitStopCostsView { DataContext = new UnitStopCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT21, (e, callback, defaultParamValue, daily) => new ValveControlCostsView { DataContext = new ValveControlCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT22, (e, callback, defaultParamValue, daily) => new ShutdownCostsView { DataContext = new ShutdownCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT23, (e, callback, defaultParamValue, daily) => new ShutdownCostsView { DataContext = new ShutdownCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT24, (e, callback, defaultParamValue, daily) => new PurgeCostsView { DataContext = new PurgeCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT17, (e, callback, defaultParamValue, daily) => new UnitBleedingCostsView { DataContext = new UnitBleedingCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT57, (e, callback, defaultParamValue, daily) => new PopValveTuningCostsView { DataContext = new PopValveTuningCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT26, (e, callback, defaultParamValue, daily) => new ChemicalAnalysisCostsView { DataContext = new ChemicalAnalysisCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT27, (e, callback, defaultParamValue, daily) => new ControlEquipmentCostsView { DataContext = new ControlEquipmentCostsViewModel(e, callback, defaultParamValue, daily) });            
            _calcDictionary.Add(CostType.CT61, (e, callback, defaultParamValue, daily) => new CompStationLossView { DataContext = new CompStationLossViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT65, (e, callback, defaultParamValue, daily) => new ThermalDisposalUnitCostsView { DataContext = new ThermalDisposalUnitCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT66, (e, callback, defaultParamValue, daily) => new CompUnitsHeatingCostsView { DataContext = new CompUnitsHeatingCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT83, (e, callback, defaultParamValue, daily) => new CompUnitsTestingCostsView { DataContext = new CompUnitsTestingCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT84, (e, callback, defaultParamValue, daily) => new DiaphragmReplacementCostsView { DataContext = new DiaphragmReplacementCostsViewModel(e, callback, defaultParamValue, daily) });            
            _calcDictionary.Add(CostType.CT85, (e, callback, defaultParamValue, daily) => new PurgeCostsView { DataContext = new VesselsExaminationCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT87, (e, callback, defaultParamValue, daily) => new PurgeCostsView { DataContext = new DrainageBleedingCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT92, (e, callback, defaultParamValue, daily) => new KptgOwnNeedsCostsView { DataContext = new KptgOwnNeedsCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT95, (e, callback, defaultParamValue, daily) => new ReservePowerStationMaintenanceCostsView{ DataContext = new ReservePowerStationMaintenanceCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT98, (e, callback, defaultParamValue, daily) => new IncidentLossView { DataContext = new IncidentLossViewModel(e, callback, defaultParamValue, daily) });
            #endregion
            #region ЛЧ
            _calcDictionary.Add(CostType.CT28, (e, callback, defaultParamValue, daily) => new PneumaticExploitationCostsView { DataContext = new PneumaticExploitationCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT88, (e, callback, defaultParamValue, daily) => new ValveExploitationCostsView { DataContext = new ValveExploitationCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT89, (e, callback, defaultParamValue, daily) => new PopValveTuningCostsView { DataContext = new PopValveTuningCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT29, (e, callback, defaultParamValue, daily) => new RepairCostsView { DataContext = new RepairCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT30, (e, callback, defaultParamValue, daily) => new PurgeCostsView { DataContext = new PurgeCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT31, (e, callback, defaultParamValue, daily) => new CleaningCostsView { DataContext = new CleaningCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT32, (e, callback, defaultParamValue, daily) => new CleaningCostsView { DataContext = new CleaningCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT33, (e, callback, defaultParamValue, daily) => new MethanolFillingCostsView { DataContext = new MethanolFillingCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT34, (e, callback, defaultParamValue, daily) => new HydrateRemoveCostsView { DataContext = new HydrateRemoveCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT55, (e, callback, defaultParamValue, daily) => new EnergyGenerationCostsView { DataContext = new EnergyGenerationCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT96, (e, callback, defaultParamValue, daily) => new ReservePowerStationMaintenanceCostsView { DataContext = new ReservePowerStationMaintenanceCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT36, (e, callback, defaultParamValue, daily) => new HeatingCostsView { DataContext = new HeatingCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT60, (e, callback, defaultParamValue, daily) => new ChemicalAnalysisCostsView { DataContext = new ChemicalAnalysisCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT64, (e, callback, defaultParamValue, daily) => new CompStationLossView { DataContext = new CompStationLossViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT93, (e, callback, defaultParamValue, daily) => new ReducingStationOwnNeedsCostsView { DataContext = new ReducingStationOwnNeedsCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT90, (e, callback, defaultParamValue, daily) => new ControlEquipmentCostsView { DataContext = new ControlEquipmentCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT35, (e, callback, defaultParamValue, daily) => new PurgeCostsView { DataContext = new PurgeCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT37, (e, callback, defaultParamValue, daily) => new PneumaticExploitationCostsView { DataContext = new PneumaticExploitationCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT44, (e, callback, defaultParamValue, daily) => new ValveExploitationCostsView { DataContext = new ValveExploitationCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT53, (e, callback, defaultParamValue, daily) => new PopValveTuningCostsView { DataContext = new PopValveTuningCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT51, (e, callback, defaultParamValue, daily) => new CompStationLossView { DataContext = new CompStationLossViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT94, (e, callback, defaultParamValue, daily) => new ControlEquipmentCostsView { DataContext = new ControlEquipmentCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT100, (e, callback, defaultParamValue, daily) => new PipelineLossView { DataContext = new PipelineLossViewModel(e, callback, defaultParamValue, daily) });
            #endregion
            #region ГРС
            _calcDictionary.Add(CostType.CT38, (e, callback, defaultParamValue, daily) => new HeaterWorkCostsView { DataContext = new HeaterWorkCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT39, (e, callback, defaultParamValue, daily) => new PurgeCostsView { DataContext = new PurgeCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT40, (e, callback, defaultParamValue, daily) => new MethanolFillingCostsView { DataContext = new MethanolFillingCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT41, (e, callback, defaultParamValue, daily) => new DiaphragmReplacementCostsView { DataContext = new DiaphragmReplacementCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT42, (e, callback, defaultParamValue, daily) => new PneumaticExploitationCostsView { DataContext = new PneumaticExploitationCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT43, (e, callback, defaultParamValue, daily) => new RepairCostsView { DataContext = new RepairCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT25, (e, callback, defaultParamValue, daily) => new PopValveTuningCostsView{DataContext = new PopValveTuningCostsViewModel(e, callback, defaultParamValue, daily)});
            _calcDictionary.Add(CostType.CT74, (e, callback, defaultParamValue, daily) => new FluidControllerCostsView { DataContext = new FluidControllerCostsViewModel(e, callback, defaultParamValue, daily)});
            _calcDictionary.Add(CostType.CT45, (e, callback, defaultParamValue, daily) => new HeatingCostsView { DataContext = new HeatingCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT59, (e, callback, defaultParamValue, daily) => new ChemicalAnalysisCostsView { DataContext = new ChemicalAnalysisCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT46, (e, callback, defaultParamValue, daily) => new ControlEquipmentCostsView { DataContext = new ControlEquipmentCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT75, (e, callback, defaultParamValue, daily) => new PurgeCostsView { DataContext = new PurgeCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT62, (e, callback, defaultParamValue, daily) => new CompStationLossView { DataContext = new CompStationLossViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT86, (e, callback, defaultParamValue, daily) => new PurgeCostsView { DataContext = new VesselsExaminationCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT102, (e, callback, defaultParamValue, daily) => new IncidentLossView { DataContext = new IncidentLossViewModel(e, callback, defaultParamValue, daily) });
            #endregion
            #region ГИС
            _calcDictionary.Add(CostType.CT47, (e, callback, defaultParamValue, daily) => new DiaphragmReplacementCostsView { DataContext = new DiaphragmReplacementCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT49, (e, callback, defaultParamValue, daily) => new PneumaticExploitationCostsView { DataContext = new PneumaticExploitationCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT48, (e, callback, defaultParamValue, daily) => new FluidControllerCostsView { DataContext = new FluidControllerCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT50, (e, callback, defaultParamValue, daily) => new RepairCostsView { DataContext = new RepairCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT56, (e, callback, defaultParamValue, daily) => new PopValveTuningCostsView { DataContext = new PopValveTuningCostsViewModel(e, callback, defaultParamValue, daily)});
            _calcDictionary.Add(CostType.CT52, (e, callback, defaultParamValue, daily) => new HeatingCostsView { DataContext = new HeatingCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT58, (e, callback, defaultParamValue, daily) => new ChemicalAnalysisCostsView { DataContext = new ChemicalAnalysisCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT54, (e, callback, defaultParamValue, daily) => new PurgeCostsView { DataContext = new PurgeCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT63, (e, callback, defaultParamValue, daily) => new CompStationLossView { DataContext = new CompStationLossViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT91, (e, callback, defaultParamValue, daily) => new ControlEquipmentCostsView { DataContext = new ControlEquipmentCostsViewModel(e, callback, defaultParamValue, daily) });
            _calcDictionary.Add(CostType.CT104, (e, callback, defaultParamValue, daily) => new IncidentLossView { DataContext = new IncidentLossViewModel(e, callback, defaultParamValue, daily) });
            #endregion
        }
        public void Show(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool showDayly)
        {
            var window = GetCalcView(gasCost, callback, defaultParamValues, showDayly);
            window?.ShowDialog();
        }
        private RadWindow GetCalcView(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool showDayly)
        {
            return _calcDictionary[gasCost.CostType](gasCost, callback, defaultParamValues, showDayly);
        }
        public bool CanShow(CostType costType)
        {
            return _calcDictionary.ContainsKey(costType);
        }
    }
}