namespace GazRouter.DTO.Dictionaries.ValvePurposes
{
    /// <summary>
    /// Назначение крана
    /// </summary>
	public enum ValvePurpose
	{
        /// <summary>
        /// Линейный
        /// </summary>
		Linear = 1,

        /// <summary>
        /// Входной охранный КЦ
        /// </summary>
		InletProtectiveCompShop = 19,

        /// <summary>
        /// Секущий КЦ
        /// </summary>
		TransversalCompShop = 20,

        /// <summary>
        /// Выходной охранный КЦ 
        /// </summary>
		OutletProtectiveCompShop = 21,
	
        /// <summary>
        /// Входной КЦ
        /// </summary>
        InletCompShop = 7,

        /// <summary>
        /// Выходной КЦ
        /// </summary>
		OutletCompShop = 8,

        /// <summary>
        /// Кран КПОУ
        /// </summary>
        ReceivingRefiningDevice = 70,

        /// <summary>
        /// Кран КЗОУ
        /// </summary>
        StartingRefiningDevice = 80
    }
}
