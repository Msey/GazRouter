using System;
using System.Windows;
using GazRouter.Flobus.Model;

namespace GazRouter.Flobus.Utilites
{
    public static class PipelinePointExtensions
    { /// <summary>
      ///     Выравнивание точки относительно другой точки
      /// </summary>
      /// <param name="basePoint">Точка относительно которой необходимо вырвнивание</param>
      /// <param name="sensitivity">
      ///     чувствительность - выравнивание происходит если абсолютное значение
      ///     разницы координат двух точек не превышает указанное в этом параметре значние
      /// </param>
        public static void Align(this IPipelinePoint point, IPipelinePoint basePoint, int sensitivity)
        {
            var dx = Math.Abs(point.Position.X - basePoint.Position.X);
            var dy = Math.Abs(point.Position.Y - basePoint.Position.Y);

            // выравниваем точку по одной из составляющх (вертикаль или горизонталь)
            // только в том случае, если разница между этими координатами у точек меньше sensitivity (px),
            // иначе считается, что это так и задумано и точка просто не выравнивается и идем дальше
            if (Math.Min(dx, dy) < sensitivity)
                point.Position = dx < dy ? new Point(basePoint.Position.X, point.Position.Y) : new Point(point.Position.X, basePoint.Position.Y);
        }

        /// <summary>
        ///     Сдвигает точку на указанное значение по горизонтали и вертикали
        /// </summary>
        /// <param name="xShift">Значение сдвига по оси X</param>
        /// <param name="yShift">Значение сдвига по оси Y</param>
        public static void Move(this IPipelinePoint point, double xShift, double yShift)
        {
            point.Position = new Point();
        }
    }
}