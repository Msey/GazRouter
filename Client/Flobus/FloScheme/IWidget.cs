using System.Windows;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.FloScheme
{
    /// <summary>
    /// Описывает виджеты для схемы
    /// </summary>
    public interface IWidget : ISupportMouseOver
    {
        /// <summary>
        /// Слои схемы которым принадлежит виджет
        /// </summary>
        SchemeLayers Layer{ get;}

        /// <summary>
        /// Выбран ли элемент
        /// </summary>
        bool IsSelected {get;set;}

        /// <summary>
        /// Границы
        /// </summary>
        Rect Bounds { get; }

        /// <summary>
        /// Видимость
        /// </summary>
        Visibility Visibility { get; set; }

        /// <summary>
        /// Возвращает состояние виртуализации
        /// </summary>
        ItemVisibility VirtualizationState { get; set; }

        /// <summary>
        /// ZIndex
        /// </summary>
        int ZIndex { get; set; }

        /// <summary>
        /// Включен или выключен
        /// </summary>
        bool IsEnabled { get; set; }
      
    }
}