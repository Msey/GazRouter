using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Utils.Units;


namespace GazRouter.Modes.GasCosts.Dialogs.MethanolFillingCosts
{
    public class MethanolFillingCostsViewModel : CalcViewModelBase<MethanolFillingCostsModel>
    {
       
        #region Constructors and Destructors

        public MethanolFillingCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback,
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
                //Density = defaultValues.Density;
                PressureAir = defaultValues.PressureAir;
                NitrogenContent = defaultValues.NitrogenContent;
                CarbonDioxideContent = defaultValues.CarbonDioxideContent;
            }
            PerformCalculate();
        }

        #endregion

        #region Public Properties

        
        ///// <summary>
        /////     Плотность газа, кг/м³
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
        /// Геометрический объем устройства, м³
        /// </summary>
        public double Volume
        {
            get
            {
                return Model.Volume;
            }
            set
            {
                Model.Volume = value;
                OnPropertyChanged(() => Volume);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Кол-во заправок устройства
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
        ///     Давление газа, кг/см²
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
        ///     Температура газа, Гр.С
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
        ///     Давление атмосферное, мм рт.ст.
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

            AddValidationFor(() => Volume)
                .When(() => Volume <= 0 || Volume > 50)
                .Show("Недопустимое значение. Должно быть больше 0 и меньше 50.");
            
            AddValidationFor(() => Count)
                .When(() => Count <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");
            
            AddValidationFor(() => Pressure)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(Pressure))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);
            
            AddValidationFor(() => Temperature)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(Temperature))
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
        

        #endregion
    }
}