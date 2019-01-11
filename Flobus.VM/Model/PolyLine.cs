using GazRouter.Common.ViewModel;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.UiEntities.FloModel;
using GazRouter.Flobus.VM.Serialization;
using System;
using System.Windows;
using System.Windows.Media;

namespace GazRouter.Flobus.VM.Model
{
    public class PolyLine : PropertyChangedBase, ISearchable, IPolyLine
    {
        private Color _color = Colors.Blue;
        private double _thickness = 2;
        private Point _position;
        private Point _startPoint;
        private Point _endPoint;
        private string _name;
        private string _description;
        private LineType _type = LineType.Solid;
        private bool _isFound;

        public PolyLine()
        {
            EndPoint = new Point(StartPoint.X + 100, StartPoint.Y);
        }
                
        public PolyLine(PolyLineJson pl)
        {
            Position = pl.Position;
            StartPoint = pl.StartPoint;
            EndPoint = pl.EndPoint;
            Color = pl.Color;
            Thickness = pl.Thickness;
            Type = pl.Type;
            Name = pl.Name;
            Description = pl.Description;
        }
        public Point StartPoint
        {
            get { return _startPoint; }
            set { SetProperty(ref _startPoint, value); }
        }
        public Point EndPoint
        {
            get { return _endPoint; }
            set { SetProperty(ref _endPoint, value); }
        }
        
        public Color Color
        {
            get { return _color; }
            set { SetProperty(ref _color, value); }
        }

        public double Thickness
        {
            get { return _thickness; }
            set
            {
                if (value < 1)
                {
                    return;
                }
                SetProperty(ref _thickness, value);
            }
        }

        public Point Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }
        
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        
        public LineType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }
        public Guid Id => new Guid();

        public string ShortPath => Name != null ? Name : "Без наименования";

        public string TypeName => "Объект НСИ";

        public bool IsFound
        {
            get { return _isFound; }
            set { SetProperty(ref _isFound, value); }
        }
    }
}
