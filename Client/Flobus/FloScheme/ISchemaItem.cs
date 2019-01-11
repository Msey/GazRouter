using System.Windows;

namespace GazRouter.Flobus.FloScheme
{

    /// <summary>
    /// Интерфейс для элементов лежащих непосредственно на схеме
    /// </summary>
    public interface ISchemaItem  : IWidget, ISupportManipulation
    {

        /// <summary>
        /// Позиция элемента на схеме
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// Веделяем элемент
        /// </summary>
        void Select();

        /// <summary>
        /// Снимает выделение
        /// </summary>
        void Deselect();
     
    }
}