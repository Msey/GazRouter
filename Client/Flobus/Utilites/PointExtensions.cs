using System;
using System.Windows;

namespace GazRouter.Flobus.Utilites
{
    public  static  class PointExtensions
    {
        public static Point Round(this Point point)
        {
            return new Point(Math.Round(point.X), Math.Round(point.Y));
        } 
    }
}