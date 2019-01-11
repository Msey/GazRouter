using Utils.Units;

namespace Utils.Calculations
{
    /// <summary>
    /// ����� ���������� ����������� � �������� ��� ����������� ��������
    /// </summary>
    public static class StandardConditions
    {
        /// <summary>
        /// ����������� ��� ����������� ��������, �
        /// </summary>
        public static readonly Temperature T = Temperature.FromKelvins(293.15);

        /// <summary>
        /// �������� ��� ����������� ��������, ���
        /// </summary>
        public static readonly Pressure P = Pressure.FromMpa(0.101325);

        public static readonly Density DensityOfAir = Density.FromKilogramsPerCubicMeter( 1.2044);
    }
}