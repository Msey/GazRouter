namespace GazRouter.DTO.Dictionaries.PipelineTypes
{
  
    /// <summary>
    /// Тип газопровода
    /// </summary>
	public enum PipelineType
	{
		/// <summary>
		/// Магистральный 
		/// </summary>
		Main = 21,

		/// <summary>
		/// Распределительный 
		/// </summary>
		Distribution = 22,

		/// <summary>
		/// Лупинг
		/// </summary>
		Looping = 23,

		/// <summary>
		///  Межсистемная перемычка
		/// </summary>
		Bridge = 24,

		/// <summary>
		/// Резервная нитка подводного перехода
		/// </summary>
		Booster = 25,

		/// <summary>
		/// Газопровод-отвод
		/// </summary>
		Branch = 26,

        /// <summary>
        /// Газопровод подключения
        /// </summary>
        Inlet = 27,

        /// <summary>
        /// Входной газопровод КЦ
        /// </summary>
        CompressorShopInlet = 28,

        /// <summary>
        /// Выходной газопровод КЦ
        /// </summary>
        CompressorShopOutlet = 29,

        /// <summary>
        /// Межцеховая перемычка
        /// </summary>
        CompressorShopBridge = 30,

        /// <summary>
        /// Камера приема/запуска очистного устройства
        /// </summary>
        RefiningDeviceChamber = 31
    }
}
