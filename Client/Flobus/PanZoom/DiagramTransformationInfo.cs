using System.Windows;

namespace GazRouter.Flobus.PanZoom
{
    internal struct DiagramTransformationInfo
    {
        public DiagramTransformationInfo(Point panOffset, double zoomLevel, Point zoomCenter) : this()
        {
            PanOffset = panOffset;
            ZoomLevel = zoomLevel;
            ZoomCenter = zoomCenter;
        }

        public Point PanOffset { get; private set; }

        public double ZoomLevel { get; private set; }

        public Point ZoomCenter { get; private set; }
    }
}