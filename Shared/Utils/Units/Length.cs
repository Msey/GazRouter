namespace Utils.Units
{
    /// <summary>
    /// Структура для хранения расстояния
    /// </summary>
    public struct Length
    {
        private readonly double _meters;

        public Length(double meters)
        {
            _meters = meters;
        }

        public static Length FromKilometers(double kilometers)
        {
            return new Length(kilometers*1000);
        }
    }
}