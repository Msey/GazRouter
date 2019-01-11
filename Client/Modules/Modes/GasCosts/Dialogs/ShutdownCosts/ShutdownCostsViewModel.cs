using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Controls.Dialogs.PipingVolumeCalculator;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Utils.Units;
using PipingVolumeCalculatorView = GazRouter.Controls.Dialogs.PipingVolumeCalculator.PipingVolumeCalculatorView;

namespace GazRouter.Modes.GasCosts.Dialogs.ShutdownCosts
{
    public class ShutdownCostsViewModel : CalcViewModelBase<ShutdownCostsModel>
    {
        #region Constructors and Destructors

        public ShutdownCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues, true)
        {
            this.ShowDayly = ShowDayly;

            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);

                var defaultValues = DefaultParamValues.Single(d => d.Target == TargetId);
                //   Density = defaultValues.Density;
                PressureAir = defaultValues.PressureAir;
                CarbonDioxideContent = defaultValues.CarbonDioxideContent;
                NitrogenContent = defaultValues.NitrogenContent;
            }

            if (gasCost.Entity.EntityType == DTO.Dictionaries.EntityTypes.EntityType.CompShop)
            {
                CompShopDTO CompShop = gasCost.Entity as CompShopDTO;
                if (CompShop != null)
                {
                    if (CompShop.PipingVolumeIn.HasValue&& PipingVolumeIn==0.0) PipingVolumeIn = CompShop.PipingVolumeIn.Value;
                    if (CompShop.PipingVolumeOut.HasValue&& PipingVolumeOut==0.0) PipingVolumeOut = CompShop.PipingVolumeOut.Value;
                }
            }

            OpenPipingVolumeInCalculator = new DelegateCommand(
                () =>
                {
                    var vm = new PipingVolumeCalculatorViewModel(
                        Model.PipingIn,
                        lst =>
                        {
                            Model.PipingIn = lst;
                            //Model.PipingVolume = lst.Sum(p => p.Volume);

                            Model.PipingVolumeIn = Model.PipingIn.Sum(p => p.Volume);

                            OnPropertyChanged(() => PipingVolumeIn);
                            PerformCalculate();
                        });
                    var v = new PipingVolumeCalculatorView
                    {
                        DataContext = vm
                    };
                    v.ShowDialog();
                });

            OpenPipingVolumeOutCalculator = new DelegateCommand(
                () =>
                {
                    var vm = new PipingVolumeCalculatorViewModel(
                        Model.PipingOut,
                        lst =>
                        {
                            Model.PipingOut = lst;
                            //Model.PipingVolume = lst.Sum(p => p.Volume);
                            Model.PipingVolumeOut = Model.PipingOut.Sum(p => p.Volume);
                            OnPropertyChanged(() => PipingVolumeOut);
                            PerformCalculate();
                        });
                    var v = new PipingVolumeCalculatorView
                    {
                        DataContext = vm
                    };
                    v.ShowDialog();
                });

            ClientCache.DictionaryRepository.Diameters
             .Select(d =>
                 new PipeSection
                 {
                     Diameter = d.DiameterConv,
                     Length = 0
                 }
                 ).ToList();

            CopyValueCommand = new DelegateCommand<object>(
                x =>
                {
                    var way = int.Parse((string)x);
                    switch (way)
                    {
                        case 1:
                            PressureInitialOut = PressureInitialIn;
                            break;
                        case 2:
                            TemperatureInitialOut = TemperatureInitialIn;
                            break;
                        case 3:
                            PressureFinalOut = PressureFinalIn;
                            break;
                        case 4:
                            TemperatureFinalOut = TemperatureFinalIn;
                            break;
                    }
                });
            PerformCalculate();
        }
        #endregion

        #region Public Properties

        /// <summary>
        ///     Копирует данные из одного поля в другое
        /// </summary>
        public DelegateCommand<object> CopyValueCommand { get; private set; }

        public DelegateCommand OpenPipingVolumeInCalculator { get; private set; }
        public DelegateCommand OpenPipingVolumeOutCalculator { get; private set; }

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

        ///// <summary>
        ///// Плотность газа, кг/м³
        ///// </summary>
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
        ///     Геометрический объем входных коммуникаций цеха, м³
        /// </summary>
        public double PipingVolumeIn
        {
            get
            {
                return Model.PipingVolumeIn;
            }
            set
            {
                Model.PipingVolumeIn = value;
                OnPropertyChanged(() => PipingVolumeIn);
                PerformCalculate();
            }
        }// => Model.PipingVolumeIn;

        /// <summary>
        ///     Геометрический объем выходных коммуникаций цеха, м³
        /// </summary>
        public double PipingVolumeOut
        {
            get
            {
                return Model.PipingVolumeOut;
            }
            set
            {
                Model.PipingVolumeOut = value;
                OnPropertyChanged(() => PipingVolumeOut);
                PerformCalculate();
            }
        } //=> Model.PipingVolumeOut;

        /// <summary>
        ///     Давление газа на входе цеха до отключения, кг/см²
        /// </summary>
        public Pressure PressureInitialIn
        {
            get
            {
                return Model.PressureInitialIn;
            }
            set
            {
                Model.PressureInitialIn = value;
                OnPropertyChanged(() => PressureInitialIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление газа на выходе цеха до отключения, кг/см²
        /// </summary>
        public Pressure PressureInitialOut
        {
            get
            {
                return Model.PressureInitialOut;
            }
            set
            {
                Model.PressureInitialOut = value;
                OnPropertyChanged(() => PressureInitialOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление газа на входе цеха после стравливания, кг/см²
        /// </summary>
        public Pressure PressureFinalIn
        {
            get
            {
                return Model.PressureFinalIn;
            }
            set
            {
                Model.PressureFinalIn = value;
                OnPropertyChanged(() => PressureFinalIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление газа на выходе цеха после стравливания, кг/см²
        /// </summary>
        public Pressure PressureFinalOut
        {
            get
            {
                return Model.PressureFinalOut;
            }
            set
            {
                Model.PressureFinalOut = value;
                OnPropertyChanged(() => PressureFinalOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     количество остановов КЦ
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

        ///// <summary>
        /////     количество продувок
        ///// </summary>
        //public int PurgeCount
        //{
        //    get
        //    {
        //        return Model.PurgeCount;
        //    }
        //    set
        //    {
        //        Model.PurgeCount = value;
        //        OnPropertyChanged(() => PurgeCount);
        //        PerformCalculate();
        //    }
        //}

        /// <summary>
        ///     Температура газа на входе цеха до отключения, Гр.С
        /// </summary>
        public Temperature TemperatureInitialIn
        {
            get
            {
                return Model.TemperatureInitialIn;
            }
            set
            {
                Model.TemperatureInitialIn = value;
                OnPropertyChanged(() => TemperatureInitialIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Тмпература газа на выходе цеха до отключения, Гр.С
        /// </summary>
        public Temperature TemperatureInitialOut
        {
            get
            {
                return Model.TemperatureInitialOut;
            }
            set
            {
                Model.TemperatureInitialOut = value;
                OnPropertyChanged(() => TemperatureInitialOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура газа на входе цеха после стравливания, Гр.С
        /// </summary>
        public Temperature TemperatureFinalIn
        {
            get
            {
                return Model.TemperatureFinalIn;
            }
            set
            {
                Model.TemperatureFinalIn = value;
                OnPropertyChanged(() => TemperatureFinalIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура газа на выходе цеха после стравливания, Гр.С
        /// </summary>
        public Temperature TemperatureFinalOut
        {
            get
            {
                return Model.TemperatureFinalOut;
            }
            set
            {
                Model.TemperatureFinalOut = value;
                OnPropertyChanged(() => TemperatureFinalOut);
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
        #endregion
        #region Methods

        protected override void SetValidationRules()
        {

            AddValidationFor(() => StopCount)
                .When(() => StopCount <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            //AddValidationFor(() => PurgeCount)
            //    .When(() => PurgeCount < 0 || PurgeCount > 10)
            //    .Show("Недопустимое значение (диапазон допустимых значений от 0 до 10)");
            
            AddValidationFor(() => PipingVolumeIn)
                 .When(() => PipingVolumeIn <= 0)
                 .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => PipingVolumeOut)
                 .When(() => PipingVolumeOut <= 0)
                 .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => PressureInitialIn)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureInitialIn))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => PressureInitialOut)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureInitialOut))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => PressureFinalIn)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureFinalIn))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => PressureFinalOut)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureFinalOut))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            //AddValidationFor(() => Density)
            //    .When(() => Density < ValueRangeHelper.DensityRange.Min || Density > ValueRangeHelper.DensityRange.Max)
            //    .Show(ValueRangeHelper.DensityRange.ViolationMessage);

            AddValidationFor(() => TemperatureInitialIn)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureInitialIn))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => TemperatureInitialOut)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureInitialOut))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => TemperatureFinalIn)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureFinalIn))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => TemperatureFinalOut)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureFinalOut))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

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


        //protected override void MeasuringLoaded()
        //{
        //    PressureInlet = Pressure.FromKgh(MeasuringLoader.GetValue(PropertyType.PressureInlet) ?? 0);
        //    PressureOutlet = Pressure.FromKgh(MeasuringLoader.GetValue(PropertyType.PressureOutlet) ?? 0);
        //    TemperatureInlet = Temperature.FromCelsius(MeasuringLoader.GetValue(PropertyType.TemperatureInlet) ?? 0);
        //    TemperatureOutlet = Temperature.FromCelsius(MeasuringLoader.GetValue(PropertyType.TemperatureOutlet) ?? 0);
        //}

        #endregion
    }
}