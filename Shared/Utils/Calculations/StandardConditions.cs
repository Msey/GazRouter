using Utils.Units;

namespace Utils.Calculations
{
    /// <summary>
    /// Класс определяет температуру и давление при стандартных условиях
    /// </summary>
    public static class StandardConditions
    {
        /// <summary>
        /// Температура при стандартных условиях, К
        /// </summary>
        public static readonly Temperature T = Temperature.FromKelvins(293.15);

        /// <summary>
        /// Давление при стандартных условиях, МПа
        /// </summary>
        public static readonly Pressure P = Pressure.FromMpa(0.101325);

        public static readonly Density DensityOfAir = Density.FromKilogramsPerCubicMeter( 1.2044);
    }
}