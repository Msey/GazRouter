using System;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Flobus.Model;

namespace GazRouter.Flobus.UiEntities.FloModel
{
    /// <summary>
    ///     Класс описывающий отрезки, из которых состоит газопровод
    /// </summary>
    public class PipelineSegment
    {
        public PipelineSegment(IPipelinePoint beginingPoint, IPipelinePoint endPoint)
        {
            BeginingPoint = beginingPoint;
            EndPoint = endPoint;
        }

        public IPipelinePoint BeginingPoint { get; set; }
        public IPipelinePoint EndPoint { get; set; }

        /// <summary>
        ///     Угол наклона относительно горизонта
        /// </summary>
        public double Angle => BeginingPoint.Position.Y == EndPoint.Position.Y
            ? BeginingPoint.Position.X < EndPoint.Position.X ? 0 : 180
            : BeginingPoint.Position.Y < EndPoint.Position.Y ? 90 : 270;

        public Orientation Orientation => Angle%180 == 0 ? Orientation.Horizontal : Orientation.Vertical;

        /// <summary>
        ///     Расчитывет точку на отрезке по значению пикетного километра
        /// </summary>
        /// <param name="km">Пикетный километр</param>
        /// <returns>Точка на отрезке</returns>
        public Point Km2Point(double km)
        {
            if ((km < BeginingPoint.Km) || (km > EndPoint.Km))
            {
                throw new ArgumentOutOfRangeException(nameof(km),
                    $"Километр {km} не попадает в границы сегмента {BeginingPoint.Km} - {EndPoint.Km} ");
            }
            if (km == BeginingPoint.Km)
            {
                return BeginingPoint.Position;
            }
            if (km == EndPoint.Km)
            {
                return EndPoint.Position;
            }

            var dist =
                Math.Round(
                    Support2D.RectangularPathLength(BeginingPoint.Position, EndPoint.Position)/
                    (EndPoint.Km - BeginingPoint.Km)*(km - BeginingPoint.Km), 0);

            /*    Support2D.Length(BeginingPoint.Position, EndPoint.Position)
                       / (EndPoint.Km - BeginingPoint.Km) * (km - BeginingPoint.Km);*/
            return Support2D.PointOnRectangularLine(BeginingPoint.Position, EndPoint.Position, dist);
        }

        /// <summary>
        ///     Рассчитывает значение пикетного километра в зависимости от координат точки на отрезке
        /// </summary>
        /// <param name="pt">Координаты точки</param>
        /// <returns>Пикетный километр</returns>
        public double Point2Km(Point pt)
        {
            return Math.Round(BeginingPoint.Km
                              + (EndPoint.Km - BeginingPoint.Km)
                              /Support2D.RectangularPathLength(BeginingPoint.Position, EndPoint.Position)
                              *Support2D.RectangularPathLength(BeginingPoint.Position, pt), 3);
        }

    }
}