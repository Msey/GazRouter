using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Utils.Units;

namespace GazRouter.Modes.GasCosts.Dialogs.HydrateRemoveCosts
{
    public class HydrateRemoveCostsViewModel : CalcViewModelBase<HydrateRemoveCostsModel>
    {
        #region Constructors and Destructors

        public HydrateRemoveCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues,ShowDayly)
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
                CarbonDioxideContent = defaultValues.CarbonDioxideContent;
                NitrogenContent = defaultValues.NitrogenContent;
                PressureAir = defaultValues.PressureAir;
            }
            
            SetValidationRules();
            PerformCalculate();
        }

        #endregion

        #region Public Properties

        public List<Bleeder> BleederList
        {
            get
            {
                return new List<Bleeder>
                {
                    //new Bleeder { Diameter = 0.02, CriticalLength = 2 },
                    //new Bleeder { Diameter = 0.05, CriticalLength = 3.5 },
                    //new Bleeder { Diameter = 0.08, CriticalLength = 5.5 },
                    //new Bleeder { Diameter = 0.1, CriticalLength = 7.5 },
                    //new Bleeder { Diameter = 0.15, CriticalLength = 12 },
                    //new Bleeder { Diameter = 0.2, CriticalLength = 17 },
                    //new Bleeder { Diameter = 0.3, CriticalLength = 27 },
                    new Bleeder { Diameter = 0.007, CriticalLength = 100},
                    new Bleeder { Diameter = 0.01, CriticalLength = 100 },
                    new Bleeder { Diameter = 0.014, CriticalLength = 100 },
                    new Bleeder { Diameter = 0.02, CriticalLength = 100 },
                    new Bleeder { Diameter = 0.025, CriticalLength = 1000 },
                    new Bleeder { Diameter = 0.05, CriticalLength = 1000 },
                    new Bleeder { Diameter = 0.08, CriticalLength = 1000 },
                    new Bleeder { Diameter = 0.1, CriticalLength = 1000 },
                    new Bleeder { Diameter = 0.15, CriticalLength = 1000 },
                    new Bleeder { Diameter = 0.3, CriticalLength = 1000 }
                };
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
        /// Давление газа в начале участка газопровода, кг/см²
        /// </summary>
        public Pressure PressureIn
        {
            get
            {
                return Model.PressureIn;
            }
            set
            {
                Model.PressureIn = value;
                OnPropertyChanged(() => PressureIn);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Давление газа в конеце участка газопровода, кг/см²
        /// </summary>
        public Pressure PressureOut
        {
            get
            {
                return Model.PressureOut;
            }
            set
            {
                Model.PressureOut = value;
                OnPropertyChanged(() => PressureOut);
                PerformCalculate();
            }
        }

        
        /// <summary>
        /// Время продувки
        /// </summary>
        public int PurgeTime
        {
            get
            {
                return Model.PurgeTime;
            }
            set
            {
                Model.PurgeTime = value;
                OnPropertyChanged(() => PurgeTime);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Длина дренажной линии, м
        /// </summary>
        public double Length
        {
            get
            {
                return Model.Length;
            }
            set
            {
                Model.Length = value;
                OnPropertyChanged(() => Length);
                PerformCalculate();
            }
        }

        public Bleeder SelectedBleeder
        {
            get
            {
                return Model.Bleeder;
            }
            set
            {
                Model.Bleeder = value;
                OnPropertyChanged(() => SelectedBleeder);
                ClearValidations();
                SetValidationRules();
                PerformCalculate();
            }
        }

        /// <summary>
        /// Температура газа в начале участка, Гр.С
        /// </summary>
        public Temperature TemperatureIn
        {
            get
            {
                return Model.TemperatureIn;
            }
            set
            {
                Model.TemperatureIn = value;
                OnPropertyChanged(() => TemperatureIn);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Температура газа в конце участка, Гр.С
        /// </summary>
        public Temperature TemperatureOut
        {
            get
            {
                return Model.TemperatureOut;
            }
            set
            {
                Model.TemperatureOut =value; 
                OnPropertyChanged(() => TemperatureOut);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Геометрический объем метанольного устройства, м³
        /// </summary>
        public double MethanolTankVolume
        {
            get
            {
                return Model.MethanolTankVolume;
            }
            set
            {
                Model.MethanolTankVolume = value;
                OnPropertyChanged(() => MethanolTankVolume);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Кол-во заправок метанольного устройства
        /// </summary>
        public int FillingCount
        {
            get
            {
                return Model.FillingCount;
            }
            set
            {
                Model.FillingCount = value;
                OnPropertyChanged(() => FillingCount);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Давление атмосферное, мм рт.ст.
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
        /// Т.к. расход газа по данной статье не нормируется и не планируется, 
        /// то нужно разрешить ввод только фактических данных
        /// </summary>
        public bool IsInputAllowed
        {
            get { return TargetId == Target.Fact; }
        }
        

        #endregion

        #region Methods

        protected override void SetValidationRules()
        {
            AddValidationFor(() => PurgeTime)
                .When(() => PurgeTime <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => SelectedBleeder)
                .When(() => SelectedBleeder == null)
                .Show("Не выбран диаметр свечного крана");

            AddValidationFor(() => PressureIn)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureIn))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => PressureOut)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureOut))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => TemperatureIn)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureIn))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => TemperatureOut)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureOut))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => Density)
                .When(() => Density < ValueRangeHelper.DensityRange.Min || Density > ValueRangeHelper.DensityRange.Max)
                .Show(ValueRangeHelper.DensityRange.ViolationMessage);

            AddValidationFor(() => PressureAir)
                .When(() => PressureAir < ValueRangeHelper.PressureAirRange.Min || PressureAir > ValueRangeHelper.PressureAirRange.Max)
                .Show(ValueRangeHelper.PressureAirRange.ViolationMessage);

            AddValidationFor(() => NitrogenContent)
                .When(() => NitrogenContent < ValueRangeHelper.ContentRange.Min || NitrogenContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            AddValidationFor(() => CarbonDioxideContent)
                .When(() => CarbonDioxideContent < ValueRangeHelper.ContentRange.Min || CarbonDioxideContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            AddValidationFor(() => MethanolTankVolume)
                .When(() => MethanolTankVolume <= 0 || MethanolTankVolume > 50)
                .Show("Недопустимое значение (интервал допустимых значений от 1 до 50)");
            
            AddValidationFor(() => FillingCount)
                .When(() => FillingCount <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            var maxLength = SelectedBleeder?.CriticalLength ?? 300;
            var str = string.Concat("Недопустимое значение (интервал допустимых значений от 1 до ", maxLength.ToString(), ")");
            AddValidationFor(() => Length)
                .When(() => Length <= 0 || Length > maxLength)
                .Show(str);
        }
        

        #endregion
    }
}