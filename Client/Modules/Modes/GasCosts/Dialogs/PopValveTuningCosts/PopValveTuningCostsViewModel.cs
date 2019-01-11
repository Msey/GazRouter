using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GazRouter.Application.Helpers;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.PopValveTuningCosts
{
    public class PopValveTuningCostsViewModel : CalcViewModelBase<PopValveTuningCostsModel>
    {
#region Constructor
        public PopValveTuningCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues)
        {
            this.ShowDayly = ShowDayly;
            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);
                var defaultValues = DefaultParamValues.Single(d => d.Target == TargetId);
                Density = defaultValues.Density;
                PressureAir = defaultValues.PressureAir;
                NitrogenContent = defaultValues.NitrogenContent;
                CarbonDioxideContent = defaultValues.CarbonDioxideContent;
            }
            SetValidationRules();
            PerformCalculate();
        }
        #endregion

        public override string FormatType => ((Coef > 1) ? "n3" : "n6");

        protected override void PerformCalculate()
        {
            Result = null;
            ValidateAll();
            if (HasErrors) return;
            Result = Math.Round(Model.Calculate(), 6);
        }

        #region Public Properties
        private bool _isPopValveSelected;
        public bool IsPopValveSelected
        {
            get { return _isPopValveSelected; }
            set
            {
                SetProperty(ref _isPopValveSelected, value);
            }
        }
        public PopValve SelectedPopValve
        {
            get
            {
                if (Model.SelectedPopValve != null)
                {
                    IsPopValveSelected = true;
                }
                return Model.SelectedPopValve; 
            }
            set
            {
                Model.SelectedPopValve = value;
                if (SelectedPopValve == null)
                {
                    ValveDiameter = 0;
                    IsPopValveSelected = false;
                }
                else
                {
                    ValveDiameter = SelectedPopValve.Diameter; 
                    IsPopValveSelected = true;
                }
            }
        }
        public List<PopValve> PopValveList
        {
            get
            {
                return ClientCache.DictionaryRepository.EmergencyValveTypes.Select(v => new PopValve(v)).ToList();
            }
        }
        /// <summary>
        ///     Плотность газа, кг/м³
        /// </summary>
        public double Density
        {
            get
            {
                return Model.Density.KilogramsPerCubicMeter;
            }
            set
            {
                Model.Density = Utils.Units.Density.FromKilogramsPerCubicMeter(value);
                OnPropertyChanged(() => Density);
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
        /// <summary>
        /// Внутренний диаметр предохранительного клапана, м
        /// </summary>
        public double ValveDiameter
        {
            get
            {
                return Model.ValveDiameter;
            }
            set
            {
                Model.ValveDiameter = value;
                OnPropertyChanged(() => ValveDiameter);
                PerformCalculate();
            }
        }
        /// <summary>
        /// Высота подъема предохранительного клапана, м
        /// </summary>
        public double ValveHeight
        {
            get
            {
                return Model.ValveHeight;
            }
            set
            {
                Model.ValveHeight = value;
                OnPropertyChanged(() => ValveHeight);
                PerformCalculate();
            }
        }
        /// <summary>
        ///     Давление газа при настройке, кг/см²
        /// </summary>
        public Pressure Pressure
        {
            get
            {
                return Model.Pressure;
            }
            set
            {
                Model.Pressure = value;
                OnPropertyChanged(() => Pressure);
                PerformCalculate();
            }
        }
        /// <summary>
        ///     Температура газа при настройке, Гр.С
        /// </summary>
        public Temperature Temperature
        {
            get
            {
                return Model.Temperature;
            }
            set
            {
                Model.Temperature = value;
                OnPropertyChanged(() => Temperature);
                PerformCalculate();
            }
        }
        /// <summary>
        /// Атмосферное давление
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
        /// <summary> количество проверок </summary>
        public int CheckCount
        {
            get
            {
                return Model.CheckCount;
            }
            set
            {
                Model.CheckCount = value;
                OnPropertyChanged(() => CheckCount);
                PerformCalculate();
            }
        }
        /// <summary> Продолжительность интервала срабатывания </summary>
        public int CheckTime
        {
            get
            {
                return Model.CheckTime;
            }
            set
            {
                Model.CheckTime = value;
                OnPropertyChanged(() => CheckTime);
                PerformCalculate();
            }
        }
#endregion

        #region Methods
        protected override void SetValidationRules()
        {
            AddValidationFor(() => CheckCount)
                .When(() => CheckCount <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => CheckTime)
                .When(() => CheckTime <= 0 || CheckTime > 600)
                .Show("Недопустимое значение (интервал допустимых значений от 1 до 600)");

            AddValidationFor(() => ValveDiameter)
                .When(() => ValveDiameter <= 0 || ValveDiameter > 1.5)
                .Show("Некорректное значение");

            AddValidationFor(() => ValveHeight)
                .When(() => ValveHeight <= 0)
                .Show("Некорректное значение");

            AddValidationFor(() => Temperature)
               .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(Temperature))
               .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => Pressure)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(Pressure))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => PressureAir)
                .When(() => PressureAir < ValueRangeHelper.PressureAirRange.Min || PressureAir > ValueRangeHelper.PressureAirRange.Max)
                .Show(ValueRangeHelper.PressureAirRange.ViolationMessage);

            AddValidationFor(() => Density)
                .When(() => Density < ValueRangeHelper.DensityRange.Min || Density > ValueRangeHelper.DensityRange.Max)
                .Show(ValueRangeHelper.DensityRange.ViolationMessage);

            AddValidationFor(() => NitrogenContent)
                .When(() => NitrogenContent < ValueRangeHelper.ContentRange.Min || NitrogenContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);
        }
#endregion
    }
}

