namespace GazRouter.Flobus.FloScheme
{
    /// <summary>
    /// Интерфейсдля поддержки MouseOver
    /// </summary>
    public interface ISupportMouseOver
    {
        /// <summary>
        /// Указывает находится ли курсор мыши над элементом
        /// </summary>
        bool IsMouseOver { get; set; }
    }
}