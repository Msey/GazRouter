using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.VM.Model;
using System.Windows;
using System.Windows.Media;

namespace GazRouter.Flobus.VM.Serialization
{
    public class PolyLineJson
    {
        public PolyLineJson()
        {
            Thickness = 2;
            Color = Colors.Black;
        }

        public PolyLineJson(PolyLine polyline) : this()
        {
            Position = polyline.Position;
            StartPoint = polyline.StartPoint;
            EndPoint = polyline.EndPoint;
            Color = polyline.Color;
            Thickness = polyline.Thickness;
            Type = polyline.Type;
            Name = polyline.Name;
            Description = polyline.Description;
        }

        public Point Position { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public Color Color { get; set; }
        public double Thickness { get; set; }
        public LineType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
