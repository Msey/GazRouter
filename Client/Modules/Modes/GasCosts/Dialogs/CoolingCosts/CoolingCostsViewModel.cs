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
namespace GazRouter.Modes.GasCosts.Dialogs.CoolingCosts
{
    public class CoolingCostsViewModel : CalcViewModelBase<CoolingCostsModel>
	{
        public CoolingCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> closeCallback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            :base(gasCost,  closeCallback, defaultParamValues)
        {
            LoadCoolingUnitInfo();
            this.ShowDayly = ShowDayly;

            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);

                var defaultValues = DefaultParamValues.Single(d => d.Target == TargetId);
                CombHeat = CombustionHeat.FromKCal(defaultValues.CombHeat);
                //TemperatureAir = defaultValues.
            }
            SetValidationRules();
        }

        /// <summary>
        /// Тип установки
        /// </summary>
        public CoolingUnitType UnitType
        {
            get { return Model.UnitType; }
        }
        /// <summary>
        /// Время работы установки СОГ, ч
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
        /// Температура наружного воздуха, Гр.С
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
                .When(() => Period < 0 || Period > RangeInHours)
                .Show($"Недопустимое значение (диапазон допустимых значений от 1 до {RangeInHours})");

            AddValidationFor(() => TemperatureAir)

                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(  TemperatureAir) )
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);
            

            AddValidationFor(() => CombHeat)
                .When(() => ValueRangeHelper.CombHeatRange.IsOutsideRange(CombHeat))
                .Show(ValueRangeHelper.CombHeatRange.ViolationMessage);
        }
        private async void LoadCoolingUnitInfo()
        {
            try
            {
                Behavior.TryLock();

                var unit = await new ObjectModelServiceProxy().GetCoolingUnitByIdAsync(Entity.Id);

                Model.UnitType = new CoolingUnitType(
                       ClientCache.DictionaryRepository.CoolingUnitTypes.First(u => u.Id == unit.CoolingUnitTypeId));
                OnPropertyChanged(() => UnitType);
                PerformCalculate();
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }
    }
}
