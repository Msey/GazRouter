using System.Windows;

namespace GazRouter.Flobus.VM.Serialization
{
    public class PointJson 
    {
        public PointJson(Point point, double km)
        {
            Point = point;
            Km = km;
        }

        public Point Point { get; set; }

        public double Km { get; set; }
    }
}