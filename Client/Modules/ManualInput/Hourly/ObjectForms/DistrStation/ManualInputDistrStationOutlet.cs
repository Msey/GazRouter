using System;
using GazRouter.Application.Helpers;

namespace GazRouter.ManualInput.Hourly.ObjectForms.DistrStation
{
    public class ManualInputDistrStationOutlet
    {

        public ManualInputDistrStationOutlet()
        {
            PressureOutlet = new ManualInputPropertyValue();
            TemperatureOutlet = new ManualInputPropertyValue();
            Flow = new ManualInputPropertyValue();


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
        /// Давление газа на выходе ГРС, кг/см²
        /// </summary>
        public ManualInputPropertyValue PressureOutlet { get; set; }



        /// <summary>
        /// Температура газа на выходе ГРС, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureOutlet { get; set; }


        /// <summary>
        /// Расход газа на выходе ГРС, тыс.м3
        /// </summary>
        public ManualInputPropertyValue Flow { get; set; }
    }
}