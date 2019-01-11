using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Utils.Units;
using Pressure = Utils.Units.Pressure;

namespace GazRouter.Modes.GasCosts.Dialogs.FuelGasHeatingCosts
{
    public class FuelGasHeatingCostsViewModel : CalcViewModelBase<FuelGasHeatingCostModel>
    {
        public FuelGasHeatingCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues)
        {

            this.ShowDayly = ShowDayly;
            if (!IsEdit)
            {
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);

                var defaultValues = DefaultParamValues.Single(d => d.Target == TargetId);
                Density = defaultValues.Density;
                PressureAir = defaultValues.PressureAir;
                CombustionHeat = CombustionHeat.FromKCal(defaultValues.CombHeat);
                NitrogenContent = defaultValues.NitrogenContent;
                CarbonDioxideContent = defaultValues.CarbonDioxideContent;
            }
            SetValidationRules();
            PerformCalculate();
        }
        #region HeaterTypeList

        public List<HeaterType> HeaterTypeList
        {
            get
            {
	            return
		            ClientCache.DictionaryRepository.HeaterTypes.Select(
			            t =>
			            new HeaterType
				            {
					            EffeciencyFactorRated = t.EffeciencyFactorRated,
					            GasConsumptionRate = t.GasConsumptionRate,
					            Id = t.Id,
					            Name = t.Name
				            })
		                       .Where(h => h.EffeciencyFactorRated.HasValue).OrderBy(h => h.Name)
		                       .ToList();
            }
        }


        public HeaterType SelectedHeaterType
        {
            get { return Model.HeaterType; }
            set
            {
                Model.HeaterType = value;
                OnPropertyChanged(() => SelectedHeaterType);
                
                Model.HeaterEfficiency = value.EffeciencyFactorRated.HasValue ? value.EffeciencyFactorRated.Value : 0;
                PerformCalculate();
            }
        }
        
        #endregion
        #region Values

        /// <summary>
        /// Плотность газа
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
        /// Теплота сгорания низшая
        /// </summary>

        public CombustionHeat CombustionHeat
        {
            get { return Model.CombustionHeat; }
            set
            {
                Model.CombustionHeat = value;
                OnPropertyChanged(() => CombustionHeat);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Расход подогреваемого газа
        /// </summary>

        public double Q
        {
            get { return Model.Q; }
            set
            {
                Model.Q = value;
                OnPropertyChanged(() => Q);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Давление газа на входе в ПТПГ
        /// </summary>
        public Pressure PIn
        {
            get { return Model.PIn; }
            set
            {
                Model.PIn = value;
                OnPropertyChanged(() => PIn);
                PerformCalculate();
            }
        }
        

        /// <summary>
        /// Температура газа на входе в ПТПГ
        /// </summary>

        public Temperature TIn
        {
            get { return Model.TIn; }
            set
            {
                Model.TIn = value;
                OnPropertyChanged(() => TIn);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Температрура газа на выходе из ПТПГ
        /// </summary>

        public Temperature TOut
        {
            get { return Model.TOut; }
            set
            {
                Model.TOut = value;
                OnPropertyChanged(() => TOut);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Время работы
        /// </summary>

        //public int Period
        //{
        //    get { return Model.Period; }
        //    set
        //    {
        //        Model.Period = value;
        //        OnPropertyChanged(() => Period);
        //        PerformCalculate();
        //    }
        //}
        
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
        protected override void SetValidationRules()
        {
            AddValidationFor(() => SelectedHeaterType)
                .When(() => SelectedHeaterType == null)
                .Show("Не выбран тип подогревателя");

            AddValidationFor(() => CombustionHeat)
                .When(() => ValueRangeHelper.CombHeatRange.IsOutsideRange(CombustionHeat))
                .Show(ValueRangeHelper.CombHeatRange.ViolationMessage);

            AddValidationFor(() => PressureAir)
                .When(() => PressureAir < ValueRangeHelper.PressureAirRange.Min || PressureAir > ValueRangeHelper.PressureAirRange.Max)
                .Show(ValueRangeHelper.PressureAirRange.ViolationMessage);

            AddValidationFor(() => Density)
                .When(() => Density < ValueRangeHelper.DensityRange.Min || Density > ValueRangeHelper.DensityRange.Max)
                .Show(ValueRangeHelper.DensityRange.ViolationMessage);

            AddValidationFor(() => Q)
                .When(() => Q <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => PIn)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PIn))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);
            
            AddValidationFor(() => TIn)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TIn))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => TOut)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TOut))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => TOut)
                .When(() => TIn >= TOut)
                .Show("Норматив температуры газа на выходе из ПТПГ должен быть больше температуры газа на входе");

            //AddValidationFor(() => Period)
            //    .When(() => Period <= 0 || Period > RangeInHours)
            //    .Show(string.Format("Недопустимое значение (интервал допустимых значений 1 - {0})", RangeInHours));

            AddValidationFor(() => NitrogenContent)
                .When(() => NitrogenContent < ValueRangeHelper.ContentRange.Min || NitrogenContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            AddValidationFor(() => CarbonDioxideContent)
                .When(() => CarbonDioxideContent < ValueRangeHelper.ContentRange.Min || CarbonDioxideContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

        }
    }
}
