using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.ValveControlCosts;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.UnitStopCosts
{
    public class UnitStopCostsViewModel : CalcViewModelBase<UnitStopCostsModel>
    {
        #region Constructor
        public UnitStopCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues, true)
        {

            this.ShowDayly = ShowDayly;
            LoadValveTypeList();
            LoadUnitInfo();
            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);
                var defaultValues = DefaultParamValues.Single(d => d.Target == TargetId);
                //Density = defaultValues.Density;
                PressureAir = defaultValues.PressureAir;
                CarbonDioxideContent = defaultValues.CarbonDioxideContent;
                NitrogenContent = defaultValues.NitrogenContent;
                NormalShifting = true;
                PressureInlet = Pressure.Zero;
                PressureOutlet = Pressure.Zero;
                TemperatureInlet = Temperature.FromCelsius(0);
                TemperatureOutlet = Temperature.FromCelsius(0);
            }

            SetValidationRules();
        }
        #endregion
        #region Public Properties
        /// <summary>
        ///     Плотность газа, кг/м³
        /// </summary>
        //public double Density
        //{
        //    get
        //    {
        //        return Model.Density.KilogramsPerCubicMeter;
        //    }
        //    set
        //    {
        //        Model.Density = Utils.Units.Density.FromKilogramsPerCubicMeter(value);
        //        OnPropertyChanged(() => Density);
        //        PerformCalculate();
        //    }
        //}
        /// <summary>
        /// Давление атмосферное
        /// </summary>
        public double PressureAir
        {
            get
            {
                return Model.PressureAir.MmHg;
            }
            set
            {
                Model.PressureAir = Pressure.FromMmHg(value);
                OnPropertyChanged(() => PressureAir);
                PerformCalculate();
            }
        }
        /// <summary>
        ///     Давление газа на входе нагнетателя, кг/см²
        /// </summary>
        public Pressure PressureInlet
        {
            get
            {
                return Model.PressureInlet;
            }
            set
            {
                Model.PressureInlet = value;
                OnPropertyChanged(() => PressureInlet);
                PerformCalculate();
            }
        }
        /// <summary>
        ///     Давление газа на выходе нагнетателя, кг/см²
        /// </summary>
        public Pressure PressureOutlet
        {
            get
            {
                return Model.PressureOutlet;
            }
            set
            {
                Model.PressureOutlet = value;
                OnPropertyChanged(() => PressureOutlet);
                PerformCalculate();
            }
        }
        /// <summary>
        ///     количество остановов ГПА
        /// </summary>
        public int StopCount
        {
            get
            {
                return Model.StopCount;
            }
            set
            {
                Model.StopCount = value;
                OnPropertyChanged(() => StopCount);
                PerformCalculate();
            }
        }
        /// <summary>
        ///     Температура газа на входе нагнетателя, Гр.С
        /// </summary>
        public Temperature TemperatureInlet
        {
            get
            {
                return Model.TemperatureInlet;
            }
            set
            {
                Model.TemperatureInlet = value;
                OnPropertyChanged(() => TemperatureInlet);
                PerformCalculate();
            }
        }
        /// <summary>
        ///     Температура газа на выходе нагнетателя, Гр.С
        /// </summary>
        public Temperature TemperatureOutlet
        {
            get
            {
                return Model.TemperatureOutlet;
            }
            set
            {
                Model.TemperatureOutlet = value;
                OnPropertyChanged(() => TemperatureOutlet);
                PerformCalculate();
            }
        }
        /// <summary>
        ///     Содержание азота, мол.доля %
        /// </summary>
        public double NitrogenContent
        {
            get
            {
                return Model.NitrogenContent;
            }
            set
            {
                Model.NitrogenContent = value;
                OnPropertyChanged(() => NitrogenContent);
                PerformCalculate();
            }
        }
        /// <summary>
        ///     Содержание двуокиси углерода, мол.доля %
        /// </summary>
        public double CarbonDioxideContent
        {
            get
            {
                return Model.CarbonDioxideContent;
            }
            set
            {
                Model.CarbonDioxideContent = value;
                OnPropertyChanged(() => CarbonDioxideContent);
                PerformCalculate();
            }
        }
        public double ValveConsumption
        {
            get
            {
                return ValveShiftings.Sum(vt => vt.Q);
            }
        }
        public List<ValveShiftingViewModel> ValveShiftings { get; set; }
        public string UnitType
        {
            get
            {
                return Model.Unit.TypeId != 0 ? ClientCache.DictionaryRepository.CompUnitTypes.First(cut => cut.Id == Model.Unit.TypeId).Name : "";
            }
        }
        public string SuperchargerType
        {
            get
            {
                return Model.Unit.SuperchargerTypeId != 0 ? ClientCache.DictionaryRepository.SuperchargerTypes.First(st => st.Id == Model.Unit.SuperchargerTypeId).Name : "";
            }
        }
        /// <summary>
        /// Переключения выполняются в соотв. с алгоритмом останова
        /// </summary>
        public bool NormalShifting
        {
            get { return Model.NormalShifting; }
            set
            {
                Model.NormalShifting = value;
                OnPropertyChanged(() => NormalShifting);
                PerformCalculate();
            }
        }
        #endregion

        #region Methods

        protected override void SetValidationRules()
        {
            AddValidationFor(() => StopCount)
                .When(() => StopCount <= 0 || StopCount > 100)
                .Show("Недопустимое значение (интервал допустимых значений от 1 до 100)");
            
            AddValidationFor(() => PressureInlet)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureInlet))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => PressureOutlet)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureOutlet))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => TemperatureInlet)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureInlet))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => TemperatureOutlet)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureOutlet))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            //AddValidationFor(() => Density)
            //    .When(() => Density < ValueRangeHelper.DensityRange.Min || Density > ValueRangeHelper.DensityRange.Max)
            //    .Show(ValueRangeHelper.DensityRange.ViolationMessage);

            AddValidationFor(() => PressureAir)
                .When(() => PressureAir < ValueRangeHelper.PressureAirRange.Min || PressureAir > ValueRangeHelper.PressureAirRange.Max)
                .Show(ValueRangeHelper.PressureAirRange.ViolationMessage);

            AddValidationFor(() => NitrogenContent)
                .When(() => NitrogenContent < ValueRangeHelper.ContentRange.Min || NitrogenContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            AddValidationFor(() => CarbonDioxideContent)
                .When(() => CarbonDioxideContent < ValueRangeHelper.ContentRange.Min || CarbonDioxideContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);
        }
        private async void LoadUnitInfo()
        {
            try
            {
                Behavior.TryLock();

                var unit = await new ObjectModelServiceProxy().GetCompUnitByIdAsync(Entity.Id);
                Model.Unit = new CompUnit(unit);
                OnPropertyChanged(() => UnitType);
                OnPropertyChanged(() => SuperchargerType);
                PerformCalculate();
            }
            finally
            {
                Behavior.TryUnlock();
            }

        }
        private void LoadValveTypeList()
        {
            ValveShiftings = ClientCache.DictionaryRepository.ValveTypes
                    .Select(v => new ValveShifting { Id = v.Id, Name = v.Name, RatedConsumption = v.RatedConsumption, Count = 0 })
                    .Select(vs => new ValveShiftingViewModel(vs)).ToList();
            ValveShiftings.ForEach(
                v =>
                {
                    var vm =
                    Model.ValveShiftings.SingleOrDefault(c => Equals(c, v.Model));
                    if (vm != null) v.Model.Count = vm.Count;
                    v.PropertyChanged += ValveShiftingViewModelOnPropertyChanged;
                });
        }
        private void ValveShiftingViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Count")
                return;

            var vm = (ValveShiftingViewModel)(sender);
            if (Model.ValveShiftings.Contains(vm.Model))
            {
                Model.ValveShiftings.Remove(vm.Model);
            }
            if (vm.Count != 0)
                Model.ValveShiftings.Add(vm.Model);
            OnPropertyChanged(() => ValveConsumption);
            PerformCalculate();
        }
        protected override void MeasuringLoaded()
        {
            PressureInlet = Pressure.FromKgh(MeasuringLoader.GetValue(PropertyType.PressureSuperchargerInlet) ?? 0);
            PressureOutlet = Pressure.FromKgh(MeasuringLoader.GetValue(PropertyType.PressureSuperchargerOutlet) ?? 0);
            TemperatureInlet = Temperature.FromCelsius(MeasuringLoader.GetValue(PropertyType.TemperatureSuperchargerInlet) ?? 0);
            TemperatureOutlet = Temperature.FromCelsius(MeasuringLoader.GetValue(PropertyType.TemperatureSuperchargerOutlet) ?? 0);
        }
#endregion
    }
}