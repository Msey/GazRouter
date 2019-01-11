using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazRouter.Application.Helpers;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Utils.Units;

namespace GazRouter.Modes.GasCosts.Dialogs.PurgeCosts
{
    public class PurgeCostsViewModel : CalcViewModelBase<PurgeCostsModel>
    {
#region Constructors and Destructors
        public PurgeCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues)
        {

            this.ShowDayly = ShowDayly;
            Header = "Расчет расхода газа при продувке аппаратов";
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
               // maxLength = 300;
            }
            PerformCalculate();
        }
        #endregion

        #region Public Properties
        public string Header { get; set; }
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
                    //new Bleeder { Diameter = 0.3, CriticalLength = 27 }
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
        //public bool HasInnerCover
        //{
        //    get
        //    {
        //        return Model.HasInnerCover;
        //    }
        //    set
        //    {
        //        Model.HasInnerCover = value;
        //        OnPropertyChanged(() => HasInnerCover);
        //        PerformCalculate();
        //    }
        //}

        
        //public double PipeDiameter
        //{
        //    get
        //    {
        //        return Model.PipeDiameter;
        //    }
        //    set
        //    {
        //        Model.PipeDiameter = value;
        //        OnPropertyChanged(() => PipeDiameter);
        //        PerformCalculate();
        //    }
        //}

        /// <summary>
        ///     Длина дренажной линии, м
        /// </summary>
        public double PipeLength
        {
            get
            {
                return Model.PipeLength;
            }
            set
            {
                Model.PipeLength = value;
                OnPropertyChanged(() => PipeLength);
                PerformCalculate();
            }
        }
        /// <summary>
        ///     Давление газа в аппарате, кг/см²
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
        /// <summary>
        ///     количество продувок
        /// </summary>
        public int PurgeCount
        {
            get
            {
                return Model.PurgeCount;
            }
            set
            {
                Model.PurgeCount = value;
                OnPropertyChanged(() => PurgeCount);
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
      //  private double maxLength;
        public Bleeder SelectedBleeder
        {
            get
            {
                return Model.Bleeder;
            }
            set
            {
                Model.Bleeder = value;
                //maxLength = value.CriticalLength;
                OnPropertyChanged(() => SelectedBleeder);
                ClearValidations();
                SetValidationRules();
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура газа в аппарате, Гр.С
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

        #endregion

        #region Methods

        protected override void SetValidationRules()
        {
            AddValidationFor(() => PurgeCount)
                .When(() => PurgeCount <= 0 || PurgeCount > 60)
                .Show("Недопустимое значение (интервал допустимых значений от 1 до 60)");

            AddValidationFor(() => PurgeTime)
                .When(() => PurgeTime <= 0 || PurgeTime > 600)
                .Show("Недопустимое значение (интервал допустимых значений от 1 до 600)");


            

            //AddValidationFor(() => PipeDiameter)
            //    .When(() => PipeDiameter <= 0 || PipeDiameter > 1400)
            //    .Show("Недопустимое значение (интервал допустимых значений от 1 до 1400)");

            AddValidationFor(() => SelectedBleeder)
                .When(() => SelectedBleeder == null)
                .Show("Не выбран диаметр свечного крана");
            
            var maxLength = SelectedBleeder?.CriticalLength ?? 300;
            var str = string.Concat("Недопустимое значение (интервал допустимых значений от 1 до ", maxLength.ToString(), ")");
            AddValidationFor(() => PipeLength)
                .When(() => PipeLength <= 0 || PipeLength > maxLength)
                .Show(str);

            AddValidationFor(() => Pressure)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(Pressure))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => Temperature)
               .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(Temperature))
               .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);


            AddValidationFor(() => PressureAir)
                .When(() => PressureAir < ValueRangeHelper.PressureAirRange.Min || PressureAir > ValueRangeHelper.PressureAirRange.Max)
                .Show(ValueRangeHelper.PressureAirRange.ViolationMessage);

            AddValidationFor(() => Density)
                .When(() => Density < ValueRangeHelper.DensityRange.Min || Density > ValueRangeHelper.DensityRange.Max)
                .Show(ValueRangeHelper.DensityRange.ViolationMessage);

            AddValidationFor(() => NitrogenContent)
                .When(() => NitrogenContent < ValueRangeHelper.ContentRange.Min || NitrogenContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            AddValidationFor(() => CarbonDioxideContent)
                .When(() => CarbonDioxideContent < ValueRangeHelper.ContentRange.Min || CarbonDioxideContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);
        }

       

        #endregion
    }
}