using System.Windows.Media;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model
{

    public class ShapeElementModel : BoxedElementModel
    {
        public ShapeElementModel()
        {
            Type = ShapeType.Ellipse;
            StrokeColor = Colors.Black;
            StrokeThickness = 2;
            FillColor = Colors.Transparent;
        }

        public ShapeType Type { get; set; }

        public double StrokeThickness { get; set; }

        public Color StrokeColor { get; set; }

        public Color FillColor { get; set; }

        public bool IsStrokeDotted { get; set; }

        public double RotateAngle { get; set; }

        public override void CopyStyle(ElementModelBase other)
        {
            var e = other as ShapeElementModel;
            if (e != null)
            {
                Type = e.Type;
                StrokeThickness = e.StrokeThickness;
                StrokeColor = e.StrokeColor;
                FillColor = e.FillColor;
                IsStrokeDotted = e.IsStrokeDotted;
                RotateAngle = e.RotateAngle;

                Width = e.Width;
                Height = e.Height;

            }
            base.CopyStyle(other);
        }
    }


    public enum ShapeType
    {
        Ellipse = 0,
        Rectangle = 1,
        Triangle = 2
    }
}