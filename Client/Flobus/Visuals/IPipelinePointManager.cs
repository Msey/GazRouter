using System.Collections.Generic;
using System.Windows;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.UiEntities.FloModel;
using JetBrains.Annotations;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Visuals
{
    public interface IPipelinePointManager
    {
        /// <summary>
        /// Начальная точка газопровода
        /// </summary>
        IPipelinePoint Start { get; }

        /// <summary>
        /// Конечная точка газопровода
        /// </summary>
        IPipelinePoint End { get; }

        /// <summary>
        ///     ReadOnly список точек
        /// </summary>
        IList<IPipelinePoint> Points { get; }

        /// <summary>
        ///     Список сегментов газопровода
        /// </summary>
        List<GeometrySegment> GeometrySegments { get; }

        /// <summary>
        ///     Список разрывов газопровода
        /// </summary>
        List<PointSegment> OverlaySegments { get; }

        /// <summary>
        ///     Кол-во точек газопровода
        /// </summary>
        int Count { get; }

        IEnumerable<Point> GeometryPoints { get; }

        void Move(Vector offset);

        /// <summary>
        ///     Удаление точки газопровода
        /// </summary>
        /// <param name="point">Удаляемая точка</param>
        /// <returns>True - если точка удалена успешно, false - если удалить невозможно</returns>
        bool RemovePoint([NotNull] IPipelinePoint point);

        void MoveAlong(IPipelinePoint pipelinePoint, Vector offset);
        void MovePoint(IPipelinePoint pipelinePoint, IPipelinePoint point);
        void RecalculateIntermediateKm(IPipelinePoint point);
        void EnsureRectangularLine();
        List<List<IPipelinePoint>> GetPointsSeparateGaps();
        List<IPipelinePoint> GetPointsOnKm(double km_begin, double km_end);
        double MaxAlloweKm([NotNull] IPipelinePoint pipelinePoint);
        double MinAllowedKm(IPipelinePoint pipelinePoint);
        IPipelinePoint FindPoint(double km);
        Point Km2Point(double km);

        /// <summary>
        ///     Возвращает объект PipelineSegment для указанной точки
        /// </summary>
        /// <param name="p">координаты точки на газопроводе</param>
        /// <returns>объект типа PipelineSegment</returns>
        PipelineSegment FindSegment(Point p);

        IPipelinePoint CreatePoint(PointType intermediate, double km, Point p);
        void Arrange();
        IList<GeometrySegment> CreateGeometrySegments(IList<Point> list);

        /// <summary>
        ///     Добавляет новую точку газопровода
        /// </summary>
        /// <param name="newItem">новая точка</param>
        void Add(IPipelinePoint newItem);

        IPipelinePoint CreatePoint(IGeometryPoint geometryPoint, PointType infra);

        /// <summary>
        ///     Возвращает объект PipelineSegment для указанного километра
        /// </summary>
        /// <param name="km">пикетный километр на газопроводе</param>
        /// <param name="b"></param>
        /// <returns>объект типа PipelineSegment</returns>
        PipelineSegment FindSegment(double km, bool skipInfra = false);
        
        void MakeInfra(IPipelinePoint point);
        PipelineSegment FindGeometrySegment(IPipelinePoint point);
        List<Point> GetGeometryPoints(double startKm, double endKm);
    }
}