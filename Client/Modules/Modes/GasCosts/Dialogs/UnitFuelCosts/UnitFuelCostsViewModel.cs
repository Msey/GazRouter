using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.UnitFuelCosts
{
    public class UnitFuelCostsViewModel : CalcViewModelBase<UnitFuelCostsModel>
    {
#region Constructors and Destructors
        public UnitFuelCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool showDayly)
            : base(gasCost, callback, defaultParamValues, true)
        {
            LoadUnitInfo();
            this.ShowDayly = showDayly;

            if (!IsEdit)
            {
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);

                var defaultValues = DefaultParamValues.Single(d => d.Target == TargetId);
                //Density = defaultValues.Density;
                PressureAir = defaultValues.PressureAir;
                CombHeat = CombustionHeat.FromKCal(defaultValues.CombHeat);
                CarbonDioxideContent = defaultValues.CarbonDioxideContent;
                NitrogenContent = defaultValues.NitrogenContent;
            }
        }
#endregion
#region Public Properties
        /// <summary>
        /// Перекачка газа через ГПА, млн.м³
        /// </summary>
        public double Q
        {
            get
            {
                return Model.Q;
            }
            set
            {
                Model.Q = value;
                OnPropertyChanged(() => Q);
                PerformCalculate();
            }
        }
        public CombustionHeat CombHeat
        {
            get { return Model.CombHeat; }
            set
            {
                Model.CombHeat = value;
                OnPropertyChanged(() => CombHeat);
                PerformCalculate();
            }
        }
        /// <summary>
        /// Давление атмосферное, мм р.ст.
        /// </summary>
        public double PressureAir
        {
            get { return Model.PressureAir.MmHg; }
            set
            {
                Model.PressureAir = Pressure.FromMmHg(value);
                OnPropertyChanged(() => PressureAir);
                PerformCalculate();
            }
        }
        /// <summary>
        /// Температура воздуха, Гр.С
        /// </summary>
        public Temperature TemperatureAir
        {
            get { return Model.TemperatureAir; }
            set
            {
                Model.TemperatureAir = value;
                OnPropertyChanged(() => TemperatureAir);
                PerformCalculate();
            }
        }
        /// <summary>
        /// Давление газа на входе в агрегат, кг/см2
        /// </summary>
        public Pressure PressureInlet
        {
            get { return Model.PressureInlet; }
            set
            {
                Model.PressureInlet = (value);
                OnPropertyChanged(() => PressureInlet);
                PerformCalculate();
            }
        }
        /// <summary>
        /// Давление газа на выходе из агрегата, кг/см2
        /// </summary>
        public Pressure PressureOutlet
        {
            get { return Model.PressureOutlet; }
            set
            {
                Model.PressureOutlet = value;
                OnPropertyChanged(() => PressureOutlet);
                PerformCalculate();
            }
        }
        /// <summary>
        /// Температура газа на входе в агрегат, Гр.С
        /// </summary>
        public Temperature TemperatureInlet
        {
            get { return Model.TemperatureInlet; }
            set
            {
                Model.TemperatureInlet = value;
                OnPropertyChanged(() => TemperatureInlet);
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
#endregion
#region Methods
        protected override void SetValidationRules()
        {
            AddValidationFor(() => Q).When(() => Q <= 0).Show("Недопустимое значение. Не может быть меньше 0.");
            
            AddValidationFor(() => CombHeat)
                .When(() => ValueRangeHelper.CombHeatRange.IsOutsideRange(CombHeat))
                .Show(ValueRangeHelper.CombHeatRange.ViolationMessage);

            AddValidationFor(() => PressureAir)
                .When(() => PressureAir < ValueRangeHelper.PressureAirRange.Min || PressureAir > ValueRangeHelper.PressureAirRange.Max)
                .Show(ValueRangeHelper.PressureAirRange.ViolationMessage);

            AddValidationFor(() => TemperatureAir)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureAir))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => PressureInlet)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureInlet))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => PressureOutlet)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureOutlet))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => PressureOutlet)
                .When(() => PressureOutlet < PressureInlet)
                .Show("Давление на выходе КЦ должно быть больше давления на входе.");

            AddValidationFor(() => TemperatureInlet)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureInlet))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => NitrogenContent)
                .When(() => NitrogenContent < ValueRangeHelper.ContentRange.Min || NitrogenContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            AddValidationFor(() => CarbonDioxideContent)
                .When(() => CarbonDioxideContent < ValueRangeHelper.ContentRange.Min || CarbonDioxideContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);
        }
        private async void LoadUnitInfo()
        {
            var unit = await new ObjectModelServiceProxy().GetCompUnitByIdAsync(Entity.Id);

            Model.GasConsumptionRate =
                ClientCache.DictionaryRepository.CompUnitTypes.Single(ut => ut.Id == unit.CompUnitTypeId)
                    .GasConsumptionRate;
            Model.Unit = new CompUnit(unit);

            OnPropertyChanged(() => UnitType);
            OnPropertyChanged(() => SuperchargerType);

            PerformCalculate();
        }
        protected override void MeasuringLoaded()
        {
            PressureInlet = Pressure.FromKgh(MeasuringLoader.GetValue(PropertyType.PressureSuperchargerInlet) ?? 0);
            PressureOutlet = Pressure.FromKgh(MeasuringLoader.GetValue(PropertyType.PressureSuperchargerOutlet) ?? 0);
            TemperatureInlet = Temperature.FromCelsius(MeasuringLoader.GetValue(PropertyType.TemperatureSuperchargerInlet) ?? 0);
            Q = MeasuringLoader.GetValue(PropertyType.Pumping) / 1000 ?? 0;
        }
#endregion
    }
}
//        protected override void UpdateProperty()
//        {
//            OnPropertyChanged(()=> Q);
//            OnPropertyChanged(()=> CombHeat);
//            OnPropertyChanged(()=> PressureAir);
//            OnPropertyChanged(()=> TemperatureAir);
//            OnPropertyChanged(()=> PressureInlet);
//            OnPropertyChanged(()=> PressureOutlet);
//            OnPropertyChanged(()=> TemperatureInlet);
//            OnPropertyChanged(()=> NitrogenContent);
//            OnPropertyChanged(()=> CarbonDioxideContent);
//            OnPropertyChanged(()=> UnitType);
//            OnPropertyChanged(()=> SuperchargerType);
//            PerformCalculate();
//        }