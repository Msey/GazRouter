using System;
using GazRouter.Application.Helpers;
using GazRouter.DTO.Dictionaries.StatesModel;

namespace GazRouter.ManualInput.Hourly.ObjectForms.CompShop
{
    public class ManualInputCompUnit
    {

        public ManualInputCompUnit()
        {
            PressureSuperchargerInlet = new ManualInputPropertyValue();
            PressureSuperchargerOutlet = new ManualInputPropertyValue();
            TemperatureSuperchargerInlet = new ManualInputPropertyValue();
            TemperatureSuperchargerOutlet = new ManualInputPropertyValue();
            FuelGasConsumption = new ManualInputPropertyValue();
            Pumping = new ManualInputPropertyValue();
            PressureFallConfusor = new ManualInputPropertyValue();


            RpmSupercharger = new ManualInputPropertyValue();
            RpmHighHeadTurbine = new ManualInputPropertyValue();
            RpmLowHeadTurbine = new ManualInputPropertyValue();

            TemperatureHighHeadTurbineInlet = new ManualInputPropertyValue();
            TemperatureHighHeadTurbineOutlet = new ManualInputPropertyValue();
            TemperatureLowHeadTurbineInlet = new ManualInputPropertyValue();
            TemperatureLowHeadTurbineOutlet = new ManualInputPropertyValue();
            TemperatureFreeTurbineInlet = new ManualInputPropertyValue();
            TemperatureFreeTurbineOutlet = new ManualInputPropertyValue();
            
            PressureAxialFlowCompressorOutlet = new ManualInputPropertyValue();
            TemperatureAxialFlowCompressorInlet = new ManualInputPropertyValue();
            TemperatureBearing = new ManualInputPropertyValue();
            

            SetValidationRules();
        }

        private void SetValidationRules()
        {
            PressureSuperchargerInlet.GetPropertyValidation()
                .When(
                    () =>
                        PressureSuperchargerInlet.Value < ValueRangeHelper.OldPressureRange.Min ||
                        PressureSuperchargerInlet.Value > ValueRangeHelper.OldPressureRange.Max)
                .Show(ValueRangeHelper.OldPressureRange.ViolationMessage);

            PressureSuperchargerOutlet.GetPropertyValidation()
                .When(
                    () =>
                        PressureSuperchargerOutlet.Value < ValueRangeHelper.OldPressureRange.Min ||
                        PressureSuperchargerOutlet.Value > ValueRangeHelper.OldPressureRange.Max)
                .Show(ValueRangeHelper.OldPressureRange.ViolationMessage);


            TemperatureSuperchargerInlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureSuperchargerInlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureSuperchargerInlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            TemperatureSuperchargerOutlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureSuperchargerOutlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureSuperchargerOutlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            FuelGasConsumption.GetPropertyValidation().When(() => FuelGasConsumption.Value < 0).Show("Недопустимое значение. Должно быть больше 0.");
            Pumping.GetPropertyValidation().When(() => Pumping.Value < 0).Show("Недопустимое значение. Должно быть больше 0.");

            PressureFallConfusor.GetPropertyValidation()
                .When(
                    () =>
                        PressureFallConfusor.Value < ValueRangeHelper.OldPressureRange.Min ||
                        PressureFallConfusor.Value > ValueRangeHelper.OldPressureRange.Max)
                .Show(ValueRangeHelper.OldPressureRange.ViolationMessage);

            RpmSupercharger.GetPropertyValidation()
                .When(
                    () => ValueRangeHelper.RpmRange.IsOutsideRange(RpmSupercharger.Value ?? 0)
                      )
                .Show(ValueRangeHelper.RpmRange.ViolationMessage);

            RpmHighHeadTurbine.GetPropertyValidation()
                .When(
                    () => ValueRangeHelper.RpmRange.IsOutsideRange(RpmHighHeadTurbine.Value ?? 0)
                       )
                .Show(ValueRangeHelper.RpmRange.ViolationMessage);

            RpmLowHeadTurbine.GetPropertyValidation()
                .When(() => ValueRangeHelper.RpmRange.IsOutsideRange(RpmLowHeadTurbine.Value ?? 0))
                .Show(ValueRangeHelper.RpmRange.ViolationMessage);


            TemperatureHighHeadTurbineInlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureHighHeadTurbineInlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureHighHeadTurbineInlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            TemperatureHighHeadTurbineOutlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureHighHeadTurbineOutlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureHighHeadTurbineOutlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            TemperatureLowHeadTurbineInlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureLowHeadTurbineInlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureLowHeadTurbineInlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            TemperatureFreeTurbineInlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureFreeTurbineInlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureFreeTurbineInlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            TemperatureFreeTurbineOutlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureFreeTurbineOutlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureFreeTurbineOutlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            TemperatureAxialFlowCompressorInlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureAxialFlowCompressorInlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureAxialFlowCompressorInlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            TemperatureBearing.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureBearing.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureBearing.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            PressureAxialFlowCompressorOutlet.GetPropertyValidation()
                .When(
                    () =>
                        PressureAxialFlowCompressorOutlet.Value < ValueRangeHelper.OldPressureRange.Min ||
                        PressureAxialFlowCompressorOutlet.Value > ValueRangeHelper.OldPressureRange.Max)
                .Show(ValueRangeHelper.OldPressureRange.ViolationMessage);
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Состояние ГПА
        /// </summary>
        public CompUnitState State { get; set; }

        
        /// <summary>
        /// Давление газа перед нагнетателем, кг/см²
        /// </summary>
        public ManualInputPropertyValue PressureSuperchargerInlet { get; set; }
        
        /// <summary>
        /// Давление газа за нагнетателем, кг/см²
        /// </summary>
        public ManualInputPropertyValue PressureSuperchargerOutlet { get; set; }
        
        /// <summary>
        /// Температура газа перед нагнетателем, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureSuperchargerInlet { get; set; }

        /// <summary>
        /// Температура газа за нагнетателем, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureSuperchargerOutlet { get; set; }
        
        /// <summary>
        /// Расход топливного газа, тыс.м3
        /// </summary>
        public ManualInputPropertyValue FuelGasConsumption { get; set; }

        /// <summary>
        /// Перекачка, тыс.м3
        /// </summary>
        public ManualInputPropertyValue Pumping { get; set; }


        /// <summary>
        /// Обороты ЦБН, об/мин
        /// </summary>
        public ManualInputPropertyValue RpmSupercharger { get; set; }

        /// <summary>
        /// Обороты ТВД, об/мин
        /// </summary>
        public ManualInputPropertyValue RpmHighHeadTurbine { get; set; }

        /// <summary>
        /// Обороты ТНД, об/мин
        /// </summary>
        public ManualInputPropertyValue RpmLowHeadTurbine { get; set; }


        /// <summary>
        /// Температура газа перед ТВД, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureHighHeadTurbineInlet { get; set; }

        /// <summary>
        /// Температура газа за ТВД, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureHighHeadTurbineOutlet { get; set; }

        /// <summary>
        /// Температура газа перед ТНД, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureLowHeadTurbineInlet { get; set; }

        /// <summary>
        /// Температура газа за ТНД, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureLowHeadTurbineOutlet { get; set; }

        /// <summary>
        /// Температура газа перед СТ, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureFreeTurbineInlet { get; set; }

        /// <summary>
        /// Температура газа за СТ, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureFreeTurbineOutlet { get; set; }


        /// <summary>
        /// Давление газа за ОК, кг/см²
        /// </summary>
        public ManualInputPropertyValue PressureAxialFlowCompressorOutlet { get; set; }
        
        /// <summary>
        /// Температура газа перед ОК, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureAxialFlowCompressorInlet { get; set; }

        /// <summary>
        /// Температура подшипника, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureBearing { get; set; }


        /// <summary>
        /// Перепад давления на конфузоре, кг/см²
        /// </summary>
        public ManualInputPropertyValue PressureFallConfusor { get; set; }


        public void ZeroValues()
        {
            if (PressureSuperchargerInlet.Value.HasValue) PressureSuperchargerInlet.Value = 0;
            if (PressureSuperchargerOutlet.Value.HasValue) PressureSuperchargerOutlet.Value = 0;
            if (TemperatureSuperchargerInlet.Value.HasValue) TemperatureSuperchargerInlet.Value = 0;
            if (TemperatureSuperchargerOutlet.Value.HasValue) TemperatureSuperchargerOutlet.Value = 0;
            if (FuelGasConsumption.Value.HasValue) FuelGasConsumption.Value = 0;
            if (Pumping.Value.HasValue) Pumping.Value = 0;
            if (PressureFallConfusor.Value.HasValue) PressureFallConfusor.Value = 0;
            if (RpmSupercharger.Value.HasValue) RpmSupercharger.Value = 0;
            if (RpmHighHeadTurbine.Value.HasValue) RpmHighHeadTurbine.Value = 0;
            if (RpmLowHeadTurbine.Value.HasValue) RpmLowHeadTurbine.Value = 0;
            if (TemperatureHighHeadTurbineInlet.Value.HasValue) TemperatureHighHeadTurbineInlet.Value = 0;
            if (TemperatureHighHeadTurbineOutlet.Value.HasValue) TemperatureHighHeadTurbineOutlet.Value = 0;
            if (TemperatureLowHeadTurbineInlet.Value.HasValue) TemperatureLowHeadTurbineInlet.Value = 0;
            if (TemperatureLowHeadTurbineOutlet.Value.HasValue) TemperatureLowHeadTurbineOutlet.Value = 0;
            if (TemperatureFreeTurbineInlet.Value.HasValue) TemperatureFreeTurbineInlet.Value = 0;
            if (TemperatureFreeTurbineOutlet.Value.HasValue) TemperatureFreeTurbineOutlet.Value = 0;
            if (PressureAxialFlowCompressorOutlet.Value.HasValue) PressureAxialFlowCompressorOutlet.Value = 0;
            if (TemperatureAxialFlowCompressorInlet.Value.HasValue) TemperatureAxialFlowCompressorInlet.Value = 0;
            if (TemperatureBearing.Value.HasValue) TemperatureBearing.Value = 0;
        }
    }
}