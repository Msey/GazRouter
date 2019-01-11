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

namespace GazRouter.Modes.GasCosts.Dialogs.HeatingCosts
{
    public class HeatingCostsViewModel : CalcViewModelBase<HeatingCostsModel>
	{
        public HeatingCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> closeCallback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            :base(gasCost,  closeCallback, defaultParamValues)
        {
            
            this.ShowDayly = ShowDayly;
            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);

                var defaultValues = DefaultParamValues.Single(d => d.Target == TargetId);
                CombHeat = CombustionHeat.FromKCal(defaultValues.CombHeat);
            }
            
            LoadBoilerInfo();
            
        }

        
        
        public string TypeName
        {
            get { return Model.BoilerType.IsSmall ? "Котел малой мощности" : "Котельная"; }
        }


        
        /// <summary>
        /// Выбранная модель котла
        /// </summary>
        public BoilerType BoilerType
        {
            get { return Model.BoilerType; }
        }
        

        
        /// <summary>
        /// Время работы агрегата, ч
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
        /// Кол-во растопок
        /// </summary>
        public int LightingCount
        {
            get { return Model.LightingCount; }
            set
            {
                Model.LightingCount = value;
                OnPropertyChanged(() => LightingCount);
                PerformCalculate();
            }
        }
        

        /// <summary>
        /// Длительность остановки между пусками котла, ч
        /// </summary>
        public int ShutdownPeriod
        {
            get { return Model.ShutdownPeriod; }
            set
            {
                Model.ShutdownPeriod = value;
                OnPropertyChanged(() => ShutdownPeriod);
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
                .Show(string.Format("Недопустимое значение (интервал допустимых значений 1 - {0})", RangeInHours));

            AddValidationFor(() => LightingCount)
                .When(() => !BoilerType.IsSmall && (LightingCount < 0 || LightingCount > 60))
                .Show("Недопустимое значение (интервал допустимых значений 0 - 60)");

            AddValidationFor(() => ShutdownPeriod)
                .When(() => !BoilerType.IsSmall && (ShutdownPeriod < 0 || ShutdownPeriod > RangeInHours))
                .Show(string.Format("Недопустимое значение (интервал допустимых значений 0 - {0})", RangeInHours));

            AddValidationFor(() => CombHeat)
                .When(() => ValueRangeHelper.CombHeatRange.IsOutsideRange(CombHeat))
                .Show(ValueRangeHelper.CombHeatRange.ViolationMessage);

            

        }


        private async void LoadBoilerInfo()
        {
            try
            {
                Behavior.TryLock();

                var boiler = await new ObjectModelServiceProxy().GetBoilerByIdAsync(Entity.Id);

                var boilerType =
                    ClientCache.DictionaryRepository.BoilerTypes.FirstOrDefault(b => b.Id == boiler.BoilerTypeId);
                Model.BoilerType = new BoilerType(boilerType);

                Model.HeatLossFactor = boiler.HeatLossFactor;
                Model.HeatSupplySystemLoad = boiler.HeatSupplySystemLoad;

                OnPropertyChanged(() => BoilerType);
                OnPropertyChanged(() => TypeName);

                PerformCalculate();

            }
            finally 
            {
                Behavior.TryUnlock();
            }
            
        }
        

        
    }
}
