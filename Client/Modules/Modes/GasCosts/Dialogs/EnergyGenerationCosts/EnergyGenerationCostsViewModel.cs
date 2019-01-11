using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.EnergyGenerationCosts
{
    public class EnergyGenerationCostsViewModel : CalcViewModelBase<EnergyGenerationCostsModel>
	{
        public EnergyGenerationCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> closeCallback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            :base(gasCost, closeCallback, defaultParamValues)
        {

            this.ShowDayly = ShowDayly;
            LoadPowerUnitInfo();
            
            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);

                var defaultValues = DefaultParamValues.Single(d => d.Target == TargetId);
                CombHeat = CombustionHeat.FromKCal(defaultValues.CombHeat);
            }
            SetValidationRules();
        }
       
        /// <summary>
        /// Выбранный тип двигателей
        /// </summary>
        public PowerUnitType PowerUnitType
        {
            get { return Model.PowerUnitType; }
        }
        /// <summary>
        /// Время работы энергоблока, ч
        /// </summary>
        public int Period
        {
            get { return Model.Period; }
            set
            {
                Model.Period = value;
                OnPropertyChanged(() => Period);
                PerformCalculate();
            }
        }
        /// <summary>
        /// Кол-во запусков ГТУ
        /// </summary>
        public int TurbineStartCount
        {
            get { return Model.TurbineStartCount; }
            set
            {
                Model.TurbineStartCount = value;
                OnPropertyChanged(() => TurbineStartCount);
                PerformCalculate();
            }
        }
        /// <summary>
        /// Электрическая мощность энергоблока, кВт
        /// </summary>
        public int Power
        {
            get { return Model.Power; }
            set
            {
                Model.Power = value;
                OnPropertyChanged(() => Power);
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
        /// Низшая теплота сгорания газа, ккал/м³
        /// </summary>
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

        protected override void SetValidationRules()
        {
            AddValidationFor(() => Period)
                .When(() => Period <= 0 || Period > RangeInHours)
                .Show($"Недопустимое значение (интервал допустимых значений от 1 до {RangeInHours})");

            AddValidationFor(() => TurbineStartCount)
                .When(() => TurbineStartCount < 0 || TurbineStartCount > 60)
                .Show("Недопустимое значение (интервал допустимых значений от 0 до 60)");
            
            var powerMax = 1.5 * ClientCache.DictionaryRepository.PowerUnitTypes.Max(p => p.RatedPower);
            AddValidationFor(() => Power)
                .When(() => Power < 1 || Power > powerMax)
                .Show($"Недопустимое значение (интервал допустимых значений от 1 до {powerMax})");

            AddValidationFor(() => TemperatureAir)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureAir))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);
           
            AddValidationFor(() => CombHeat)
                .When(() => ValueRangeHelper.CombHeatRange.IsOutsideRange(CombHeat))
                .Show(ValueRangeHelper.CombHeatRange.ViolationMessage);
        }
        private async void LoadPowerUnitInfo()
        {
            try
            {
                Behavior.TryLock();

                var pu = await new ObjectModelServiceProxy().GetPowerUnitByIdAsync(Entity.Id);

                Model.PowerUnitType =
                    new PowerUnitType(
                        ClientCache.DictionaryRepository.PowerUnitTypes.FirstOrDefault(p => p.Id == pu.PowerUnitTypeId));
                Model.RunningTimeCoefficient = pu.OperatingTimeFactor;
                Model.TurbineConsumption = pu.TurbineConsumption;
                Model.TurbineRuntime = pu.TurbineRuntime;

                OnPropertyChanged(() => PowerUnitType);
                PerformCalculate();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
    }
}
