using System;
using GazRouter.Application.Helpers;

namespace GazRouter.ManualInput.Hourly.ObjectForms.MeasStation
{
    public class ManualInputMeasLine
    {

        public ManualInputMeasLine()
        {
            PressureInlet = new ManualInputPropertyValue();
            TemperatureInlet = new ManualInputPropertyValue();
            PressureOutlet = new ManualInputPropertyValue();
            TemperatureOutlet = new ManualInputPropertyValue();
            Flow = new ManualInputPropertyValue();


            PressureInlet.GetPropertyValidation()
                .When(
                    () =>
                        PressureInlet.Value < ValueRangeHelper.OldPressureRange.Min ||
                        PressureInlet.Value > ValueRangeHelper.OldPressureRange.Max)
                .Show(ValueRangeHelper.OldPressureRange.ViolationMessage);

            TemperatureInlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureInlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureInlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);


            PressureOutlet.GetPropertyValidation()
                .When(
                    () =>
                        PressureOutlet.Value < ValueRangeHelper.OldPressureRange.Min ||
                        PressureOutlet.Value > ValueRangeHelper.OldPressureRange.Max)
                .Show(ValueRangeHelper.OldPressureRange.ViolationMessage);

            TemperatureOutlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureOutlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureOutlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            Flow.GetPropertyValidation().When(() =>Flow.Value < 0).Show("Недопустимое значение. Должно быть больше 0.");
        }

        public Guid Id { get; set; }
        public string Name { get; set; }


        /// <summary>
        /// Давление газа на входе замерной линии, кг/см²
        /// </summary>
        public ManualInputPropertyValue PressureInlet { get; set; }



        /// <summary>
        /// Температура газа на входе замерной линии, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureInlet { get; set; }

        /// <summary>
        /// Давление газа на выходе замерной линии, кг/см²
        /// </summary>
        public ManualInputPropertyValue PressureOutlet { get; set; }



        /// <summary>
        /// Температура газа на выходе замерной линии, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureOutlet { get; set; }


        /// <summary>
        /// Расход газа через замерную линию, тыс.м3
        /// </summary>
        public ManualInputPropertyValue Flow { get; set; }
    }
}