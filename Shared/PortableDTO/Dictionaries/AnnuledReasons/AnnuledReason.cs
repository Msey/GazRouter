namespace GazRouter.DTO.Dictionaries.AnnuledReasons
{
    /// <summary>
    /// Причина отмены задания
    /// </summary>
	public enum AnnuledReason
	{
        /// <summary>
        /// Отмена задания диспетчером ЦПДД
        /// </summary>
		CancelCpdd = 1,

        /// <summary>
        /// Отмена задания диспетчером ПДС
        /// </summary>
		CancelPDS = 2,

        /// <summary>
        /// Другое
        /// </summary>
		Other = 3
	}
}
