namespace GazRouter.DTO.Dictionaries.CompUnitFailureFeatures
{
    /// <summary>
    ///     Признак отказа ГПА
    /// </summary>
    public enum CompUnitFailureFeature
    {
        /// <summary>
        ///     Не определен
        /// </summary>
        None = 0,

        /// <summary>
        ///     Система электроснабжения
        /// </summary>
        R1 = 11,

        /// <summary>
        ///     САУ ГПА
        /// </summary>
        R21 = 21,

        /// <summary>
        ///     САУ КЦ
        /// </summary>
        R22 = 22,

        /// <summary>
        ///     CCC
        /// </summary>
        R23 = 23,

        /// <summary>
        ///     АМП (СУМП)
        /// </summary>
        R24 = 24,

        /// <summary>
        ///     приборы
        /// </summary>
        R25 = 25,

        /// <summary>
        ///     датчики
        /// </summary>
        R26 = 26,

        /// <summary>
        ///     обрывы
        /// </summary>
        R27 = 27,

        /// <summary>
        ///     прочие
        /// </summary>
        R28 = 28,

        /// <summary>
        ///     привод нагнетателя
        /// </summary>
        R31 = 31,

        /// <summary>
        ///     нагнетатели (компрессора) газа
        /// </summary>
        R32 = 32,

        /// <summary>
        ///     система маслоснабжения ГПА
        /// </summary>
        R33 = 33,

        /// <summary>
        ///     система уплотнения нагнетателя
        /// </summary>
        R34 = 34,

        /// <summary>
        ///     система регулирования ГПА
        /// </summary>
        R35 = 35,

        /// <summary>
        ///     Стационарные системы топливного, пускового, технологического газа и т.д.
        /// </summary>
        R4 = 4,

        /// <summary>
        ///     Нарушение ПТЭ
        /// </summary>
        R5 = 5,

        /// <summary>
        ///     ВАО с разрушением узлов и деталей
        /// </summary>
        R6 = 6,

        /// <summary>
        ///     Прочие вынужденные нормальные и аварийные остановы
        /// </summary>
        R7 = 7
    }
}