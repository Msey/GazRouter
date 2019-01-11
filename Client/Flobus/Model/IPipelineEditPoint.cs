using System.Windows;
using GazRouter.Flobus.FloScheme;

namespace GazRouter.Flobus.Model
{
    public interface IPipelineEditPoint
    {
        IPipelineWidget Pipeline { get; }
        IPipelinePoint PipelinePoint { get; set; }
        bool IsManipulating { get; set; }
        Visibility Visibility { get; set; }
        Point Position { get; set; }
        PointType Type { get;  }
    }
}