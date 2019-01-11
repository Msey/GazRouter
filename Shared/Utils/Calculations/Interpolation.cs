using System;
namespace Utils.Calculations
{
    /// <summary>
    ///            y
    /// 
    ///         y1 | y2
    ///   _________|_____
    ///   X1 | t11 | t12
    /// x ---|-----|----- 
    ///   X2 | t21 | t22
    ///   ---------------
    /// 
    /// </summary>
    public class Interpolation
    {
        static Interpolation()
        {
            Delta = .0001;
        }
        public static double Delta { get; set; }
        /// <summary>  </summary>
        /// <param name="x"></param>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        /// <param name="fx0"></param>
        /// <param name="fx1"></param>
        /// <returns></returns>
        public static double Interpolate(double x, double x0, double x1, double fx0, double fx1)
        {
            return fx0 + (fx1 - fx0) / (x1 - x0) * (x - x0);
        }
        public static double Interpolate2(double x, double x0, double x1, double y, double y0, double y1,
                                          double t11, double t12, double t21, double t22)
        {
            var t0 = Math.Abs(x0 - x1) < Delta ? t11 : Interpolate(x, x0, x1, t11, t21);
            var t1 = Math.Abs(x0 - x1) < Delta ? t12 : Interpolate(x, x0, x1, t12, t22);
            return Math.Abs(y0 - y1) < Delta ? t0 : Interpolate(y, y0, y1, t0, t1);
        }
    }
}
