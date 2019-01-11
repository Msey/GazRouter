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
using Pressure = Utils.Units.Pressure;
namespace GazRouter.Modes.GasCosts.Dialogs.UnitBleedingCosts
{
    public class UnitBleedingCostsViewModel : CalcViewModelBase<UnitBleedingCostsModel>
    {
#region Constructors and Destructors
        public UnitBleedingCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues)
        {
            this.ShowDayly = ShowDayly;
            LoadUnitInfo();
            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);
                var defaultValues = DefaultParamValues.Single(d => d.Target == TargetId);
                Density = defaultValues.Density;
            }           
        }
        #endregion
        #region Public Properties
        /// <summary> Время работы ГПА, ч </summary>
        public int Runtime
        {
            get
            {
                return Model.Runtime;
            }
            set
            {
                Model.Runtime = value;
                OnPropertyChanged(() => Runtime);
                PerformCalculate();
            }
        }
        public bool IsSealingTypeUnknown
        {
            get { return !Model.Unit.SealingType.HasValue; }
        }
        /// <summary> Давление уплотняемого газа </summary>
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
        /// <summary> Плотность газа при стандартных условиях, кг/м³ </summary>
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
        /// <summary> Мощность выброса, г/с </summary>
        public double EmissionPower
        {
            get
            {
                return Model.EmissionPower;
            }
            set
            {
                Model.EmissionPower = value;
                OnPropertyChanged(() => EmissionPower);
                PerformCalculate();
            }
        }
        public bool IsEmissionPowerKnown
        {
            get { return Model.IsEmissionPowerKnown; }
            set
            {                
                Model.IsEmissionPowerKnown = value;
                OnPropertyChanged(()=> Model.IsEmissionPowerKnown);        
                //
                ClearValidations();
                SetValidationRules();
                PerformCalculate();
            }
        }
        public bool IsFact
        {
            get { return TargetId == Target.Fact; }
        }

        public bool IsEmissionsFactKnown
        {
            get { return Model.IsEmissionsFactKnown; }
            set
            {
                Model.IsEmissionsFactKnown = value;
                OnPropertyChanged(() => Model.IsEmissionsFactKnown);
                //
                ClearValidations();
                SetValidationRules();
                PerformCalculate();
            }
        }
        /// <summary>
        /// Объемный расход выбросов, м3/ч
        /// </summary>
        public double EmissionsFact
        {
            get
            {
                return Model.EmissionsFact;
            }
            set
            {
                Model.EmissionsFact = value;
                OnPropertyChanged(() => EmissionsFact);
                PerformCalculate();
            }
        }
        #endregion
        #region Methods
        protected override void SetValidationRules()
        {
            AddValidationFor(() => Runtime)
                .When(() => Runtime <= 0 || Runtime > RangeInHours)
                .Show($"Недопустимое значение (интервал допустимых значений от 1 до {RangeInHours})");

            if (IsEmissionPowerKnown)
            {
                AddValidationFor(() => Density)
                    .When(
                        () => Density < ValueRangeHelper.DensityRange.Min || Density > ValueRangeHelper.DensityRange.Max)
                    .Show(ValueRangeHelper.DensityRange.ViolationMessage);

                AddValidationFor(() => EmissionPower)
                    .When(() => EmissionPower < 0 || EmissionPower > 10)
                    .Show($"Недопустимое значение (интервал допустимых значений от 0 до 10)");
            }
            else
            {
                AddValidationFor(() => Pressure)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(Pressure))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);
                if (IsEmissionsFactKnown)
                {
                    AddValidationFor(() => EmissionsFact)
                    .When(() => EmissionsFact < 0)
                    .Show($"Недопустимое значение. Должно быть больше 0");
                }
            }
        }
        private async void LoadUnitInfo()
        {
            try
            {
                Behavior.TryLock();
                var unit = await new ObjectModelServiceProxy().GetCompUnitByIdAsync(Entity.Id);
                Model.Unit = new CompUnit(unit);
                OnPropertyChanged(() => IsSealingTypeUnknown);
                PerformCalculate();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
#endregion
    }
}