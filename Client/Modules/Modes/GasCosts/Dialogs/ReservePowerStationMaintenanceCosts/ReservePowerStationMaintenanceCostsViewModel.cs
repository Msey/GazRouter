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

namespace GazRouter.Modes.GasCosts.Dialogs.ReservePowerStationMaintenanceCosts
{
    public class ReservePowerStationMaintenanceCostsViewModel : CalcViewModelBase<ReservePowerStationMaintenanceCostsModel>
    {
        public ReservePowerStationMaintenanceCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> closeCallback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
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
          //  SetValidationRules();
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
        /// Расход газа на прокрутку вала ГТУ энергоблока, м³
        /// </summary>
        public double Qpr
        {
            get { return Model.Qpr; }
            set
            {
                Model.Qpr = value;
                OnPropertyChanged(() => Qpr);
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

        /// <summary>
        /// Количество пусков энергоблока для работы под нагрузкой
        /// </summary>
        public double TurbineLoadCount
        {
            get { return Model.TurbineLoadCount; }
            set
            {
                Model.TurbineLoadCount = value;
                OnPropertyChanged(() => TurbineLoadCount);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Количество пусков энергоблока для работы при холодной прокрутке
        /// </summary>
        public double TurbineColdCount
        {
            get { return Model.TurbineColdCount; }
            set
            {
                Model.TurbineColdCount = value;
                OnPropertyChanged(() => TurbineColdCount);
                PerformCalculate();
            }
        }

        protected override void SetValidationRules()
        {
            AddValidationFor(() => Period)
                .When(() => Period <= 0 || Period > RangeInHours)
                .Show($"Недопустимое значение (интервал допустимых значений от 1 до {RangeInHours})");
            
            AddValidationFor(() => TurbineLoadCount)
                .When(() => TurbineLoadCount < 0 || TurbineLoadCount > 60)
                .Show("Недопустимое значение (интервал допустимых значений от 0 до 60)");

            AddValidationFor(() => TurbineColdCount)
                .When(() => TurbineColdCount < 0 || TurbineColdCount > 60)
                .Show("Недопустимое значение (интервал допустимых значений от 0 до 60)");

            AddValidationFor(() => CombHeat)
                .When(() => ValueRangeHelper.CombHeatRange.IsOutsideRange(CombHeat))
                .Show(ValueRangeHelper.CombHeatRange.ViolationMessage);

            AddValidationFor(() => Qpr)
                    .When(() => Qpr < 0)
                    .Show("Недопустимое значение. Должно быть больше или равно 0.");
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
