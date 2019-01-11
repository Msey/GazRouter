using System;
using System.Windows;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.UiEntities
{
    public static class Support2D
    {
        /// <summary>
        /// Расчитывет длину отрезка в 2-х мерном пространстве
        /// </summary>
        /// <param name="pt1">Точка начала</param>
        /// <param name="pt2">Точка конца</param>
        /// <returns>Длина</returns>
        public static double Length(Point pt1, Point pt2)
        {
            var xd = pt2.X - pt1.X;
            var yd = pt2.Y - pt1.Y;
            return Math.Sqrt(xd * xd + yd * yd);
        }


        public static Point PointOnLine(Point startPoint, Point endPoint, double distance)
        {
            var lineLength = startPoint.Distance(endPoint);
            return new Point(Math.Round( endPoint.X + (endPoint.X - startPoint.X)/lineLength*distance),
                Math.Round(endPoint.Y + (endPoint.Y - startPoint.Y)/lineLength*distance));
        }

        /// <summary>
        /// Расчитывает точку в середине отрезка
        /// </summary>
        /// <param name="pt1">Точка начала</param>
        /// <param name="pt2">Точка конца</param>
        /// <returns>Точка в середине отрезка</returns>
        public static Point BisectingPoint(Point pt1, Point pt2)
        {
            var x = pt1.X + (pt2.X - pt1.X) / 2;
            var y = pt1.Y + (pt2.Y - pt1.Y) / 2;
            return new Point(x, y);
        }

        /// <summary>
        /// Расчитывает угол наклона отрезка относительно горизонтальной прямой
        /// </summary>
        /// <param name="pt1">Точка начала отрезка</param>
        /// <param name="pt2">Точка конца отрезка</param>
        /// <returns>Угол наклона в градусах</returns>
        public static double AngleX(Point pt1, Point pt2)
        {
            double xd = pt2.X - pt1.X;
            double yd = pt2.Y - pt1.Y;

            return Math.Atan2(yd, xd);
        }


        public static double AngleY(Point start, Point end)
        {
            if (start == end) return double.NaN;

            var sngXComp = end.X - start.X;
            var sngYComp = start.Y - end.Y ;

            if (sngYComp >= 0) return sngXComp < 0 ? Math.Atan(sngXComp / sngYComp) + (2 * Math.PI) : Math.Atan(sngXComp / sngYComp);
            return Math.Atan(sngXComp / sngYComp) + Math.PI;

        }

        public static double Radians2Degrees(double radians)
        {
            var degrees = radians * 180 / Math.PI;
            return degrees;
        }

        internal static Point PointToScreen( UIElement element, Point point)
        {
            return element.TransformToVisual(System.Windows.Application.Current.RootVisual).Transform(point);
        }

        public static double Degrees2Radians(double degrees)
        {
            return degrees * Math.PI / 180;
        }


        public static bool PointAtLine(Point startLine, Point endLine, Point pt)
        {
            double minX = Math.Min(startLine.X, endLine.X) - 2;
            double maxX = Math.Max(startLine.X, endLine.X) + 2;
            double minY = Math.Min(startLine.Y, endLine.Y) - 2;
            double maxY = Math.Max(startLine.Y, endLine.Y) + 2;
            double dist = 2828282;
            if (minX < pt.X && pt.X < maxX && minY < pt.Y && pt.Y < maxY)
            {
                double xd = Math.Abs(endLine.X - startLine.X);
                double yd = Math.Abs(endLine.Y - startLine.Y);
                double t = Math.Min(xd, yd);
                xd = Math.Max(xd, yd);
                yd = t;
                double normalLength = xd * Math.Sqrt(1 + (yd / xd) * (yd / xd));
                dist = Math.Abs((pt.X - startLine.X) * (endLine.Y - startLine.Y) - (pt.Y - startLine.Y) * (endLine.X - startLine.X)) / normalLength;
            }
            return dist <= 4;
        }

        public static double RectangularPathLength(Point startPoint, Point endPoint)
        {
          return  Math.Abs(endPoint.X - startPoint.X) + Math.Abs(endPoint.Y - startPoint.Y);
        }

        public static Point PointOnRectangularLine(Point beginPoint, Point endPoint, double dist)
        {

            double newX;
            double newY = beginPoint.Y;
            double yDist = 0;
            if (beginPoint.X < endPoint.X)
            {
                newX = beginPoint.X + dist;
                if (newX > endPoint.X)
                {
                    yDist = newX - endPoint.X;
                    newX = endPoint.X;
                }
            }
            else
            {
                newX = beginPoint.X - dist;
                if (newX < endPoint.X)
                {
                    yDist =  endPoint.X - newX;
                    newX = endPoint.X;
                }
            }

        
            if (yDist != 0)
            {
                if (beginPoint.Y < endPoint.Y)
                {
                    newY = beginPoint.Y + yDist;
                }
                else
                {
                    newY = beginPoint.Y - yDist;
                }
            }
            return new Point(newX, newY);
        }
    }
}