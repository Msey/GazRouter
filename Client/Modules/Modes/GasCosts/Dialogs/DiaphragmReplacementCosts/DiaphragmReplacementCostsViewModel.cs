using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Controls.Dialogs.PipingVolumeCalculator;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Utils.Units;
using PipingVolumeCalculatorView = GazRouter.Controls.Dialogs.PipingVolumeCalculator.PipingVolumeCalculatorView;

namespace GazRouter.Modes.GasCosts.Dialogs.DiaphragmReplacementCosts
{
    public class DiaphragmReplacementCostsViewModel : CalcViewModelBase<DiaphragmReplacementCostsModel>
    {
       
        #region Constructors and Destructors

        public DiaphragmReplacementCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
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
                //Density = defaultValues.Density;
             //   PressureAir = Pressure.FromMmHg( defaultValues.PressureAir);
                PressureAir = defaultValues.PressureAir;
                CarbonDioxideContent = defaultValues.CarbonDioxideContent;
                NitrogenContent = defaultValues.NitrogenContent;
            }

            OpenPipingVolumeCalculator = new DelegateCommand(
               () =>
               {
                   var vm = new PipingVolumeCalculatorViewModel(
                       Model.Piping,
                       lst =>
                       {
                           Model.Piping = lst;
                           OnPropertyChanged(() => PipingVolume);
                           PerformCalculate();
                       });
                   var v = new PipingVolumeCalculatorView
                   {
                       DataContext = vm
                   };
                   v.ShowDialog();
               });

            CopyValueCommand = new DelegateCommand<object>(
                x =>
                {
                    var way = int.Parse((string)x);
                    switch (way)
                    {
                        case 1:
                            PressureOut = PressureIn;
                            break;
                        case 2:
                            TemperatureOut = TemperatureIn;
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
        /// Геометрический объем замерной линии, м³
        /// </summary>
        public double PipingVolume
        {
            get
            {
                return Model.PipingVolume;
            }
        }

        public DelegateCommand OpenPipingVolumeCalculator { get; private set; }
        

        ///// <summary>
        /////     Давление газа, кг/см²
        ///// </summary>
        //public Pressure Pressure
        //{
        //    get
        //    {
        //        return Model.Pressure;
        //    }
        //    set
        //    {
        //        Model.Pressure = value;
        //        OnPropertyChanged(() => Pressure);
        //        PerformCalculate();
        //    }
        //}

        /// <summary>
        ///     Давление газа в начале участка до опорожнения, кг/см²
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
        ///     Давление газа в конце участка до опорожнения, кг/см²
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


        ///// <summary>
        /////     Температура газа, Гр.С
        ///// </summary>
        //public Temperature Temperature
        //{
        //    get
        //    {
        //        return Model.Temperature;
        //    }
        //    set
        //    {
        //        Model.Temperature = value;
        //        OnPropertyChanged(() => Temperature);
        //        PerformCalculate();
        //    }
        //}

        /// <summary>
        ///     Температура газа в начале участка до опорожнения, Гр.С
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
        ///     Температура газа в конце участка до опорожнения, Гр.С
        /// </summary>
        public Temperature TemperatureOut
        {
            get
            {
                return Model.TemperatureOut;
            }
            set
            {
                Model.TemperatureOut = value;
                OnPropertyChanged(() => TemperatureOut);
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

        ///// <summary>
        /////  Место установки диафрагмы, № линии
        ///// </summary>
        //public string PlaceDescription
        //{
        //    get
        //    {
        //        return Model.PlaceDescription;
        //    }
        //    set
        //    {
        //        Model.PlaceDescription = value;
        //        OnPropertyChanged(() => PlaceDescription);
        //        //PerformCalculate();
        //    }
        //}


        public List<double> PipeDiameterList
        {
            get { return ClientCache.DictionaryRepository.Diameters.Select(d => d.DiameterConv).ToList(); }
        }

        #endregion

        #region Methods

        protected override void SetValidationRules()
        {
            AddValidationFor(() => PipingVolume)
                .When(() => PipingVolume <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            //AddValidationFor(() => Pressure)
            //    .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(Pressure))
            //    .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            //AddValidationFor(() => Temperature)
            //    .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(Temperature))
            //    .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

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

            //AddValidationFor(() => Density)
            //    .When(() => Density < ValueRangeHelper.DensityRange.Min || Density > ValueRangeHelper.DensityRange.Max)
            //    .Show(ValueRangeHelper.DensityRange.ViolationMessage);

            //AddValidationFor(() => PressureAir)
            //    .When(() => ValueRangeHelper.PressureAirRange.IsOutsideRange(PressureAir.MmHg))
            //    .Show(ValueRangeHelper.PressureAirRange.ViolationMessage);
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
        

        #endregion
    }
}