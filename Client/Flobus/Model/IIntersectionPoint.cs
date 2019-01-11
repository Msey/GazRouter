using GazRouter.Flobus.FloScheme;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GazRouter.Flobus.Model
{
    public interface IIntersectionPoint
    {
        IPipelineWidget Pipeline { get; }
        IPipelinePoint PipelinePoint { get; set; }
        Visibility Visibility { get; set; }
        Point Position { get; set; }
        double Rotate { get; }
        double InnerRadius { get;}
        double OuterRadius { get; }
    }
}
