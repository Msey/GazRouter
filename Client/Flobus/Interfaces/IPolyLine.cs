using GazRouter.Flobus.EventArgs;
using GazRouter.Flobus.FloScheme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Interfaces
{
    public interface IPolyLineWidget
    {
        Point Position { get; set; }
        Point StartPoint { get; set; }
        Point EndPoint { get; set; }
        IEnumerable<Point> Points { get;}
        double StrokeThickness { get; set; }
        Color Stroke { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        LineType Type { get; set; }

    }

    public enum LineType
    {
        Solid = 0,         
        Dash = 1,           
        DashDot = 2,       
        DashDotDot = 3,          
        Dot = 4
    }

    public interface IPolyLine
    {
        Point Position { get; set; }
        Point StartPoint { get; set; }
        Point EndPoint { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        LineType Type { get; set; }
    }
}
