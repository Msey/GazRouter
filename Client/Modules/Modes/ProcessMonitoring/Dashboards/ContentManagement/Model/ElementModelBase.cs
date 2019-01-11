using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model
{
    /// <summary>
    /// Базовый класс для всех элементов дашборда
    /// </summary>
    
    public abstract class ElementModelBase
    {

        protected ElementModelBase()
        {
            FontSize = 10;
            FontColor = Colors.Black;
            FontStyle = MyFontStyle.Normal;
        }

        /// <summary>
        /// Z-индекс
        /// </summary>
        public int Z { get; set; }


        /// <summary>
        /// Положение элемента на дашборде
        /// </summary>
        public Point Position { get; set; }


        /// <summary>
        /// Ширина элемента
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Высота элемента
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Размер шрифта
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// Цвет шрифта
        /// </summary>
        public Color FontColor { get; set; }

        /// <summary>
        /// Цвет шрифта
        /// </summary>
        public MyFontStyle FontStyle { get; set; }


        /// <summary>
        /// Список индетификаторов сущностей, которые связаны с данным элементом. 
        /// Необходимо для формирования перечня сущностей для запроса получения данных.
        /// </summary>
        public virtual List<Guid> GetRelatedEntityList()
        {
            return null; 
        }

        public virtual void CopyStyle(ElementModelBase other)
        {
            FontSize = other.FontSize;
            FontColor = other.FontColor;
            FontStyle = other.FontStyle;
        }
    }


    public enum MyFontStyle
    {
        Normal = 0,
        Bold = 1,
        Italic = 2,
        BoldItalic = 3
    }

}