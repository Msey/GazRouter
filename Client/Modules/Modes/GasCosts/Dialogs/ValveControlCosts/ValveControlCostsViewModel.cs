using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Microsoft.Practices.Prism.Commands;
namespace GazRouter.Modes.GasCosts.Dialogs.ValveControlCosts
{
    public class ValveControlCostsViewModel : CalcViewModelBase<ValveControlCostsModel>
	{
        public ValveControlCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues)
        {

            this.ShowDayly = ShowDayly;
            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);
            }
#region Valve
            ValveShiftings = ClientCache.DictionaryRepository.ValveTypes
                    .Select(v => new ValveShifting {
                        Id = v.Id,
                        Name = v.Name,
                        RatedConsumption = v.RatedConsumption,
                        Count = 0 })
                    .Select(vs => new ValveShiftingViewModel(vs)).ToList();
            ValveShiftings.ForEach(
                v =>
                {
                    var vm = 
                    Model.ValveShiftings.SingleOrDefault(c => Equals(c, v.Model));
                    if (vm != null) v.Model.Count = vm.Count;
                    v.PropertyChanged += ValveShiftingViewModelOnPropertyChanged;
                });
#endregion
#region Regulator
            RegulatorRuntimes =   ClientCache.DictionaryRepository.RegulatorTypes
                 .Select(r => new RegulatorRuntime
                 {
                     Id = r.Id,
                     Name = r.Name,
                     RatedConsumption = r.GasConsumptionRate,
                     Count = 0,
                     Runtime = 0
                 })
                .Select(r => new RegulatorRuntimeViewModel(r)).ToList();
            RegulatorRuntimes.Add(
                new RegulatorRuntimeViewModel(new RegulatorRuntime
                {
                    Name = "Прочее",
                    RatedConsumption = 1.0
                }));
            RegulatorRuntimes.ForEach(
                v =>
                {
                       var vm = 
                    Model.RegulatorRuntimes.SingleOrDefault(c => Equals(c, v.Model));
                    if (vm != null)
                    {
                        v.Model.Count = vm.Count;
                        v.Model.Runtime = vm.Runtime;
                    } 
                    v.PropertyChanged += RegulatorRuntimeViewModelOnPropertyChanged;
                });
            #endregion
            SetValidationRules();
            PerformCalculate();
        }
        private void RegulatorRuntimeViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Count" && e.PropertyName != "Runtime")
                return;

            var vm = (RegulatorRuntimeViewModel)(sender);
            if (Model.RegulatorRuntimes.Contains(vm.Model))
            {
                Model.RegulatorRuntimes.Remove(vm.Model);
            }
            if (vm.Count != 0 || vm.Runtime != 0)
                Model.RegulatorRuntimes.Add(vm.Model);
            OnPropertyChanged(() => RegulatorConsumption);
            PerformCalculate();
        }
        private void ValveShiftingViewModelOnPropertyChanged(object sender, 
                                                             PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Count")
                return;

            var vm = (ValveShiftingViewModel)(sender);
            if (Model.ValveShiftings.Contains(vm.Model))
            {
                Model.ValveShiftings.Remove(vm.Model);
            }
            if (vm.Count != 0 )
                Model.ValveShiftings.Add(vm.Model);
            OnPropertyChanged(() => ValveConsumption);
            PerformCalculate();
        }
        public List<ValveShiftingViewModel> ValveShiftings { get; set; }
        public List<RegulatorRuntimeViewModel> RegulatorRuntimes { get; set; }
        public double ValveConsumption
        {
            get
            {
                return ValveShiftings.Sum(vt => vt.Q);
            }
        }
        public double RegulatorConsumption
        {
            get
            {
                return RegulatorRuntimes.Sum(vt => vt.Q);
            }
        }
        protected override void SetValidationRules()
        {
          

        }
    }
    public class ValveShiftingViewModel : PropertyChangedBase
    {
        public ValveShiftingViewModel(ValveShifting vs)
        {
            Model = vs;
        }
        public ValveShifting Model { get; private set; }
        public uint Count
        {
            get { return Model.Count; }
            set
            {
                Model.Count = value;
                OnPropertyChanged(() => Count);
                OnPropertyChanged(() => Q);
            }
        }
        public double Q
        {
            get { return Model.Q; }
        }
    }
    public class RegulatorRuntimeViewModel : PropertyChangedBase
    {
        public RegulatorRuntime Model { get; private set; }
        public RegulatorRuntimeViewModel(RegulatorRuntime rr)
        {
            Model = rr;
        }
        public uint Count
        {
            get { return Model.Count; }
            set
            {
                Model.Count = value;
                OnPropertyChanged(() => Count);
                OnPropertyChanged(() => Q);
            }
        }
        public uint Runtime
        {
            get { return Model.Runtime; }
            set
            {
                Model.Runtime = value;
                OnPropertyChanged(() => Runtime);
                OnPropertyChanged(() => Q);
            }
        }
        public double Q
        {
            get { return Model.Q; }
        }
    }
}
