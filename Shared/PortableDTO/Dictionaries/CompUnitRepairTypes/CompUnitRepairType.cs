namespace GazRouter.DTO.Dictionaries.CompUnitRepairTypes
{

    /// <summary>
    /// Тип останова ГПА
    /// </summary>
    public enum CompUnitRepairType
    {
        /// <summary>
        /// Регламентные работы
        /// </summary>
        Scheduled = 11,

        /// <summary>
        /// ТО
        /// </summary>
        Maintenance = 12,

        /// <summary>
        /// ТО двигателя
        /// </summary>
        EngineMaintenance = 13,

        /// <summary>
        /// Текущий ремонт
        /// </summary>
        Current = 21,

        /// <summary>
        /// Средний ремонт
        /// </summary>
        MidLife = 22,

        /// <summary>
        /// Капитальный ремонт
        /// </summary>
        Complete = 23,


        /// <summary>
        /// Внеплановый ремонт
        /// </summary>
        Unplanned = 31,


        /// <summary>
        /// Аварийный ремонт
        /// </summary>
        Emergency = 32,


        /// <summary>
        /// Нерегламентированный ремонт
        /// </summary>
        Unregulated = 80,


        /// <summary>
        /// Реконструкция
        /// </summary>
        Reconstruction = 90,

        /// <summary>
        /// Демонтирован
        /// </summary>
        Demounted = 100,

        /// <summary>
        /// Простой
        /// </summary>
        Idle = 101,

        /// <summary>
        /// Замена двигателя
        /// </summary>
        EngineReplace = 102



        
    }
}