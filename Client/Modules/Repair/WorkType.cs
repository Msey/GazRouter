namespace GazRouter.Repair
{
    internal static class WorkType
    {
        /// <summary>
        /// Стравливаеться
        /// </summary>
        public const int DistrStationOvergazing = 82;

        /// <summary>
        /// Отключается
        /// </summary>
        public const int DistrStationDeactivating = 81;

        public const int DistrStationWorkViaTemporaryReductionUnit = 83;

        /// <summary>
        /// Отключается
        /// </summary>
        public const int PipelineDeactivating = 341;

        /// <summary>
        /// Стравливаеться
        /// </summary>
        public const int PipelineOvergazing = 342;

        /// <summary>
        /// Ремонтируется
        /// </summary>
        public const int PipelineRepairing = 343;
        
        /// <summary>
        /// Бкз отключения
        /// </summary>
        public const int PipelineWithoutDeactivating = 344;
    }
}