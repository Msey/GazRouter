using System;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using Utils.Units;

namespace GazRouter.Application.Helpers
{
    public static class ValueRangeHelper
    {
        private static ValueRangeBase<Temperature> _temperatureRange;

        [Obsolete]
        /// <summary>
        /// Интервал допустимых значений для давления
        /// </summary>
        public static ValueRange OldPressureRange
        {
            get
            {
                // от 0 до 100 кг/см2
                const int min = 0;
                const int max = 100;
                string msg =
                    $"Недопустимое значение (интервал допустимых значений от {min} до {UserProfile.ToUserUnits(max, PhysicalType.Pressure)})";

                return new ValueRange(min, max, msg);
            }
        }


        /// <summary>
        /// Интервал допустимых значений для давления
        /// </summary>
        public static PressureRange PressureRange { get; } = new PressureRange();

        [Obsolete]
        public static ValueRange OldTemperatureRange
        {
            get
            {
                const int min = -50;
                const int max = 50;
                string msg = string.Format("Недопустимое значение (интервал допустимых значений от {0} до {1})",
                    UserProfile.ToUserUnits(min, PhysicalType.Temperature),
                    UserProfile.ToUserUnits(max, PhysicalType.Temperature));

                return new ValueRange(min, max, msg);
            }
        }

        /// <summary>
        /// Интервал допустимых значений для температуры
        /// </summary>
        /// 
        public static TemperatureRange TemperatureRange { get; } = new TemperatureRange();
        public static TemperatureAirRange TemperatureAirRange { get; } = new TemperatureAirRange();

        /// <summary>
        /// Интервал допустимых значений для плотности
        /// </summary>
        public static ValueRange DensityRange
        {
            get
            {
                // от 0.5 до 1 кг/м3
                const double min = 0.5;
                const int max = 1;
                return new ValueRange(min, max,
                    $"Недопустимое значение (интервал допустимых значений от {min} до {max})");
            }
        }


        /// <summary>
        /// Интервал допустимых значений для низшей теплоты сгорания
        /// </summary>
        public static CombHeatRange CombHeatRange { get; set; } = new CombHeatRange();
        

        public static ValueRange CombHeatJouleRange
        {
            get
            {
                // от 25 до 40 МДж/м3
                const double min = 25;
                const double max = 40;
                return new ValueRange(min, max,
                    $"Недопустимое значение (интервал допустимых значений от {min} до {max})");
            }
        }



        /// <summary>
        /// Интервал допустимых значений для плотности
        /// </summary>
        public static ValueRange PressureAirRange
        {
            get
            {
                // от 700 до 800 мм рт.ст.
                const double min = 700;
                const int max = 800;
                return new ValueRange(min, max,
                    $"Недопустимое значение (интервал допустимых значений от {min} до {max})");
            }
        }

        /// <summary>
        /// Интервал допустимых значений молярной доли
        /// </summary>
        public static ValueRange ContentRange
        {
            get
            {
                // от 0 до 10%
                const double min = 0;
                const int max = 10;
                return new ValueRange(min, max,
                    $"Недопустимое значение (интервал допустимых значений от {min} до {max})");
            }
        }


        /// <summary>
        /// Интервал допустимых значений для концентрации диоксида углерода
        /// </summary>
        public static ValueRange ConcentrationRange
        {
            get
            {
                // от 0 до 0.1 г/м3
                const double min = 0;
                const int max = 1;
                return new ValueRange(min, max,
                    $"Недопустимое значение (интервал допустимых значений от {min} до {max})");
            }
        }


        /// <summary>
        /// Интервал допустимых значений для оборотов
        /// </summary>
        public static ValueRange RpmRange { get; } = new ValueRange(1, 10000);
    }
}