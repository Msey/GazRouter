using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.Controls.Dialogs.PipingVolumeCalculator;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Utils.Units;
using PipingVolumeCalculatorView = GazRouter.Controls.Dialogs.PipingVolumeCalculator.PipingVolumeCalculatorView;

namespace GazRouter.Modes.GasCosts.Dialogs.CleaningCosts
{
    public class CleaningCostsViewModel : CalcViewModelBase<CleaningCostsModel>
    {
        
        #region Constructors and Destructors

        public CleaningCostsViewModel(GasCostDTO gasCost, 
                                      Action<GasCostDTO> callback, 
                                      List<DefaultParamValues> defaultParamValues, bool ShowDayly)
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
                PressureAir = defaultValues.PressureAir;
                NitrogenContent = defaultValues.NitrogenContent;
                CarbonDioxideContent = defaultValues.CarbonDioxideContent;
            }
            
            
            FillBleederList();

            OpenStartPipingVolumeCalculator = new DelegateCommand(
               () =>
               {
                   var vm = new PipingVolumeCalculatorViewModel(
                       Model.StartPiping,
                       lst =>
                       {
                           Model.StartPiping = lst;
                           OnPropertyChanged(() => StartPipingVolume);
                           PerformCalculate();
                       });
                   var v = new PipingVolumeCalculatorView
                   {
                       DataContext = vm
                   };
                   v.ShowDialog();
               });


            OpenEndPipingVolumeCalculator = new DelegateCommand(
               () =>
               {
                   var vm = new PipingVolumeCalculatorViewModel(
                       Model.EndPiping,
                       lst =>
                       {
                           Model.EndPiping = lst;
                           OnPropertyChanged(() => EndPipingVolume);
                           PerformCalculate();
                       });
                   var v = new PipingVolumeCalculatorView
                   {
                       DataContext = vm
                   };
                   v.ShowDialog();
               });

            PerformCalculate();
        }

        #endregion

        #region Public Properties

        public DelegateCommand OpenStartPipingVolumeCalculator { get; private set; }
        public DelegateCommand OpenEndPipingVolumeCalculator { get; private set; }



        /// <summary>
        ///     количество пропусков  (циклов очистки)
        /// </summary>
        public int Count
        {
            get
            {
                return Model.Count;
            }
            set
            {
                Model.Count = value;
                OnPropertyChanged(() => Count);
                PerformCalculate();
            }
        }

        /// <summary>
        ///    геометрический объем КЗОУ, м³
        /// </summary>
        public double StartChamberVolume
        {
            get
            {
                return Model.StartChamberVolume;
            }
            set
            {
                Model.StartChamberVolume = value;
                OnPropertyChanged(() => StartChamberVolume);
                PerformCalculate();
            }
        }

        /// <summary>
        ///    геометрический объем газопроводов перед КЗОУ, м³
        /// </summary>
        public double StartPipingVolume
        {
            get
            {
                return Model.StartPipingVolume;
            }
        }

        /// <summary>
        ///    геометрический объем КПОУ, м³
        /// </summary>
        public double EndChamberVolume
        {
            get
            {
                return Model.EndChamberVolume;
            }
            set
            {
                Model.EndChamberVolume = value;
                OnPropertyChanged(() => EndChamberVolume);
                PerformCalculate();
            }
        }

        /// <summary>
        ///    геометрический объем газопроводов перед КПОУ, м³
        /// </summary>
        public double EndPipingVolume
        {
            get
            {
                return Model.EndPipingVolume;
            }
        }

        /// <summary>
        ///    геометрический объем конденсатосборника, м³
        /// </summary>
        public double CondensateTankVolume
        {
            get
            {
                return Model.CondensateTankVolume;
            }
            set
            {
                Model.CondensateTankVolume = value;
                OnPropertyChanged(() => CondensateTankVolume);
                PerformCalculate();
            }
        }


        /// <summary>
        ///     Давление газа в КЗОУ, кг/см²
        /// </summary>
        public Pressure StartChamberPressure
        {
            get
            {
                return Model.StartChamberPressure;
            }
            set
            {
                Model.StartChamberPressure = value;
                OnPropertyChanged(() => StartChamberPressure);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура газа в КЗОУ, Гр.С
        /// </summary>
        public Temperature StartChamberTemperature
        {
            get
            {
                return Model.StartChamberTemperature;
            }
            set
            {
                Model.StartChamberTemperature = value;
                OnPropertyChanged(() => StartChamberTemperature);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление газа в КПОУ, кг/см²
        /// </summary>
        public Pressure EndChamberPressure
        {
            get
            {
                return Model.EndChamberPressure;
            }
            set
            {
                Model.EndChamberPressure = value;
                OnPropertyChanged(() => EndChamberPressure);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура газа в КПОУ, Гр.С
        /// </summary>
        public Temperature EndChamberTemperature
        {
            get
            {
                return Model.EndChamberTemperature;
            }
            set
            {
                Model.EndChamberTemperature = value;
                OnPropertyChanged(() => EndChamberTemperature);
                PerformCalculate();
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


        public List<Bleeder> BleederList { get; set; }

        private void FillBleederList()
        {
            BleederList = new List<Bleeder>
                           {
                               //new Bleeder { Diameter = 0.02, CriticalLength = 2 },
                               //new Bleeder { Diameter = 0.05, CriticalLength = 3.5 },
                               //new Bleeder { Diameter = 0.08, CriticalLength = 5.5 },
                               //new Bleeder { Diameter = 0.1, CriticalLength = 7.5 },
                               //new Bleeder { Diameter = 0.15, CriticalLength = 12 },
                               //new Bleeder { Diameter = 0.2, CriticalLength = 17 },
                               //new Bleeder { Diameter = 0.3, CriticalLength = 27 },
                               // new Bleeder { Diameter = 0.007, CriticalLength = 100},
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
        /// Длина дренажной линии, м
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



        #endregion

        #region Methods

        protected override void SetValidationRules()
        {
            AddValidationFor(() => Count)
                .When(() => Count < 1 || Count > 10)
                .Show("Недопустимое значение (интервал допустимых значений от 1 до 10)");


            AddValidationFor(() => StartChamberVolume)
                .When(() => StartChamberVolume < 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => StartPipingVolume)
                .When(() => StartPipingVolume < 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => EndChamberVolume)
                .When(() => EndChamberVolume < 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => EndPipingVolume)
                .When(() => EndPipingVolume < 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => CondensateTankVolume)
                .When(() => CondensateTankVolume < 0)
                .Show("Недопустимое значение. Не может быть меньше 0.");


            AddValidationFor(() => StartChamberPressure)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(StartChamberPressure))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => StartChamberTemperature)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(StartChamberTemperature))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => EndChamberPressure)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(EndChamberPressure))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => EndChamberTemperature)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(EndChamberTemperature))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => SelectedBleeder)
                .When(() => SelectedBleeder == null && (EndChamberVolume > 0.0 || EndPipingVolume > 0.0))
                .Show("Не выбран диаметр свечного крана");

            var maxLength = SelectedBleeder?.CriticalLength ?? 300;
            var str = string.Concat("Недопустимое значение (интервал допустимых значений от 1 до ", maxLength.ToString(), ")");
            AddValidationFor(() => Length)
                .When(() => (Length <= 0 || Length > maxLength) && (EndChamberVolume > 0.0 || EndPipingVolume > 0.0))
                .Show(str);

            AddValidationFor(() => PurgeTime)
                .When(() => PurgeTime <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");


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