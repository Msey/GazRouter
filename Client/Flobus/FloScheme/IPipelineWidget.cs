using System.Collections.Generic;
using System.Windows;
using GazRouter.Flobus.Model;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.FloScheme
{
    /// <summary>
    /// Представляет газопровод
    /// </summary>
    public interface IPipelineWidget : ISchemaItem
    {
        /// <summary>
        /// Точки газопровода
        /// </summary>
        IEnumerable<IPipelinePoint> Points { get;  }

        /// <summary>
        /// Возвращает точки для манипуляции газопроводом
        /// </summary>
        IList<IPipelineEditPoint> ManipulationPoints { get; }

        /// <summary>
        /// Позиция начала газопровода
        /// </summary>
        Point StartPoint { get; set; }

        /// <summary>
        /// Позиция конца газопровода
        /// </summary>
        Point EndPoint { get; set; }

        /// <summary>
        /// возращает дочерний элемент газопровода находящийся под курсором мыши
        /// </summary>
        IWidget PipelineElementUnderMouse { get; }

        /// <summary>
        /// Обновляет предварительную (пунктирную) геометрию газопровода 
        /// </summary>
        /// <param name="startPoint">начальная точка</param>
        /// <param name="endPoint">конечная точка</param>
        /// <param name="middlePoints">промежуточные точки</param>
        void UpdateDefferedGeometry(Point startPoint, Point endPoint, Point[] middlePoints);

        /// <summary>
        /// Добавляет промежуточную точку на газопровод
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        IPipelinePoint AddPoint(Point p);

        /// <summary>
        /// Обновляет газопровод
        /// </summary>
        void Update();
                

        /// <summary>
        /// удаляет промежуточную точку с газопровода
        /// </summary>
        /// <param name="point"></param>
        void RemovePoint(IPipelinePoint point);
        
        void MovePoint(IPipelinePoint point, Point prev_point);
        void RecalculateIntermediateKm(IPipelinePoint point);

        /// <summary>
        /// двигает газопровод 
        /// </summary>
        /// <param name="offset">смещение</param>
        void Move(Vector offset);
    }
}