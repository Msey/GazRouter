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
        /// ��������� ����� �����������
        /// </summary>
        IPipelinePoint Start { get; }

        /// <summary>
        /// �������� ����� �����������
        /// </summary>
        IPipelinePoint End { get; }

        /// <summary>
        ///     ReadOnly ������ �����
        /// </summary>
        IList<IPipelinePoint> Points { get; }

        /// <summary>
        ///     ������ ��������� �����������
        /// </summary>
        List<GeometrySegment> GeometrySegments { get; }

        /// <summary>
        ///     ������ �������� �����������
        /// </summary>
        List<PointSegment> OverlaySegments { get; }

        /// <summary>
        ///     ���-�� ����� �����������
        /// </summary>
        int Count { get; }

        IEnumerable<Point> GeometryPoints { get; }

        void Move(Vector offset);

        /// <summary>
        ///     �������� ����� �����������
        /// </summary>
        /// <param name="point">��������� �����</param>
        /// <returns>True - ���� ����� ������� �������, false - ���� ������� ����������</returns>
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
        ///     ���������� ������ PipelineSegment ��� ��������� �����
        /// </summary>
        /// <param name="p">���������� ����� �� �����������</param>
        /// <returns>������ ���� PipelineSegment</returns>
        PipelineSegment FindSegment(Point p);

        IPipelinePoint CreatePoint(PointType intermediate, double km, Point p);
        void Arrange();
        IList<GeometrySegment> CreateGeometrySegments(IList<Point> list);

        /// <summary>
        ///     ��������� ����� ����� �����������
        /// </summary>
        /// <param name="newItem">����� �����</param>
        void Add(IPipelinePoint newItem);

        IPipelinePoint CreatePoint(IGeometryPoint geometryPoint, PointType infra);

        /// <summary>
        ///     ���������� ������ PipelineSegment ��� ���������� ���������
        /// </summary>
        /// <param name="km">�������� �������� �� �����������</param>
        /// <param name="b"></param>
        /// <returns>������ ���� PipelineSegment</returns>
        PipelineSegment FindSegment(double km, bool skipInfra = false);
        
        void MakeInfra(IPipelinePoint point);
        PipelineSegment FindGeometrySegment(IPipelinePoint point);
        List<Point> GetGeometryPoints(double startKm, double endKm);
    }
}