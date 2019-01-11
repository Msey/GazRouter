namespace GazRouter.Flobus.FloScheme
{
    /// <summary>
    ///Интерфейс для поддержки манипуляции объектами
    /// </summary>
    public interface ISupportManipulation
    {
        /// <summary>
        /// можно ли таскать
        /// </summary>
        bool IsDraggingEnabled { get;  }

        /// <summary>
        /// Виден ли ManipulationAdorner 
        /// </summary>
        bool IsManipulationAdornerVisible { get; set; }

        /// <summary>
        /// можно ли изменять размер
        /// </summary>
        bool IsResizingEnabled { get; set; }
    }
}