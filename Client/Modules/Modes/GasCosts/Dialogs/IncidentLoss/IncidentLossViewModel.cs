using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Utils.Units;

namespace GazRouter.Modes.GasCosts.Dialogs.IncidentLoss
{
    public class IncidentLossViewModel : CalcViewModelBase<IncidentLossModel>
    {
        public IncidentLossViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback,
            List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues, ShowDayly)
        {
            this.ShowDayly = ShowDayly;
        
            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                {
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);
                }
                
                var defaultValues = DefaultParamValues.Single(d => d.Target == TargetId);
                Density = defaultValues.Density;
                CarbonDioxideContent = defaultValues.CarbonDioxideContent;
                NitrogenContent = defaultValues.NitrogenContent;
                PressureAir = defaultValues.PressureAir;

                Model.BranchParameterList = new List<BranchParameter>
                {
                    new BranchParameter {Number = 1},
                    new BranchParameter {Number = 2},
                    new BranchParameter {Number = 3},
                    new BranchParameter {Number = 4},
                    new BranchParameter {Number = 5},
                };
            }
            BranchParametersWrapper.Clear();
            Model.BranchParameterList.ForEach(e =>
            {
                BranchParametersWrapper.Add(new BranchParameterWrapper(e, PerformCalculate));
            });
            PerformCalculate();
        }
        
        /// <summary>
        ///     Длина участка МГ, примыкающего к узлу подключения площадного объекта (вход/выход)
        /// </summary>
        public double Length
        {
            get { return Model.Length; }
            set
            {
                Model.Length = value;
                OnPropertyChanged(() => Length);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Внутренний диаметр основной трубы МГ, мм
        /// </summary>
        public double PipeDiameter
        {
            get { return Model.PipeDiameter; }
            set
            {
                Model.PipeDiameter = value;
                OnPropertyChanged(() => PipeDiameter);
                PerformCalculate();
            }
        }
        
        /// <summary>
        ///     Время прошедшее с момента аварии до момента полного закрытия запорного крана на КС, с
        /// </summary>
        public int T7
        {
            get { return Model.T7; }
            set
            {
                Model.T7 = value;
                OnPropertyChanged(() => T7);
                PerformCalculate();
            }
        }
        
        /// <summary>
        ///     Плотность газа, кг/м³
        /// </summary>
        public double Density
        {
            get { return Model.Density.KilogramsPerCubicMeter; }
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
            get { return Model.NitrogenContent; }
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
            get { return Model.CarbonDioxideContent; }
            set
            {
                Model.CarbonDioxideContent = value;
                OnPropertyChanged(() => CarbonDioxideContent);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление газа в начале участка газопровода до разрыва, кг/см²
        /// </summary>
        public Pressure PressureIn
        {
            get { return Model.PressureIn; }
            set
            {
                Model.PressureIn = value;
                OnPropertyChanged(() => PressureIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление газа в конце участка газопровода до разрыва, кг/см²
        /// </summary>
        public Pressure PressureOut
        {
            get { return Model.PressureOut; }
            set
            {
                Model.PressureOut = value;
                OnPropertyChanged(() => PressureOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура газа в начале участка газопровода до разрыва, Гр.С
        /// </summary>
        public Temperature TemperatureIn
        {
            get { return Model.TemperatureIn; }
            set
            {
                Model.TemperatureIn = value;
                OnPropertyChanged(() => TemperatureIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура газа в конце участка газопровода до разрыва, Гр.С
        /// </summary>
        public Temperature TemperatureOut
        {
            get { return Model.TemperatureOut; }
            set
            {
                Model.TemperatureOut = value;
                OnPropertyChanged(() => TemperatureOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление атмосферное, мм рт.ст.
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

        private List<BranchParameterWrapper> _w;
        public List<BranchParameterWrapper> BranchParametersWrapper
        {
            get { return _w ?? (_w = new List<BranchParameterWrapper>()); }
        }

        protected override void SetValidationRules()
        {
            //
                AddValidationFor(() => BranchParametersWrapper)
                    .When(() => !BranchParametersWrapper.Any(e => e.Length > 0 && e.Diameter > 0))
                    .Show("Необходимо ввести данные хотя бы для одного участка!");
            // 
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
                .When(
                    () =>
                        PressureAir < ValueRangeHelper.PressureAirRange.Min ||
                        PressureAir > ValueRangeHelper.PressureAirRange.Max)
                .Show(ValueRangeHelper.PressureAirRange.ViolationMessage);

            AddValidationFor(() => NitrogenContent)
                .When(
                    () =>
                        NitrogenContent < ValueRangeHelper.ContentRange.Min ||
                        NitrogenContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            AddValidationFor(() => CarbonDioxideContent)
                .When(
                    () =>
                        CarbonDioxideContent < ValueRangeHelper.ContentRange.Min ||
                        CarbonDioxideContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            AddValidationFor(() => T7)
                .When(() => T7 <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => Length)
                .When(() => Length <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");
            
            AddValidationFor(() => PipeDiameter)
               .When(() => PipeDiameter <= 0)
               .Show("Недопустимое значение. Должно быть больше 0.");
        }
        
    }

    public class BranchParameterWrapper : PropertyChangedBase
    {
        public BranchParameterWrapper(BranchParameter parameter, Action action)
        {
            _parameter = parameter;
            PropertyChanged += (sender, args) => action.Invoke();
        }

        private readonly BranchParameter _parameter;

        public uint Number
        {
            get { return _parameter.Number; }
            set
            {
                SetProperty(ref _parameter.Number, value);                
            }
        }

        public double Length
        {
            get { return _parameter.Length; }
            set
            {
                SetProperty(ref _parameter.Length, value);
                OnPropertyChanged(() => Result);
            }
        }
        public double Diameter
        {
            get { return _parameter.Diameter; }
            set
            {
                SetProperty(ref _parameter.Diameter, value);
                OnPropertyChanged(() => Result);
            }
        }
        public double Lambda
        {
            get { return _parameter.Lambda; }
            set
            {
                SetProperty(ref _parameter.Lambda, value);
                OnPropertyChanged(() => Result);
            }
        }
        public double Result => Diameter > 0 ? Math.Round(Lambda*Length*Math.Pow(Diameter/1000.0, -5.0), 3) : 0.0;
    }
}