using System.Windows;
using GazRouter.Common.ViewModel;
using GazRouter.Flobus.Interfaces;

namespace GazRouter.Flobus.VM.Model
{
    public class GeometryPoint : PropertyChangedBase, IGeometryPoint
    {
        private Point _position;
        private double _km;

        public GeometryPoint(double km, Point point)
        {
            Km = km;
            Position = point;
        }

        public Point Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        public double Km
        {
            get { return _km; }
            set { SetProperty(ref _km, value); }
        }
    }
}