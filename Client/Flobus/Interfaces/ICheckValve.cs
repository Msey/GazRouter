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

namespace GazRouter.Flobus.Interfaces
{
    public interface ICheckValve 
    {
        Point Position { get; set; }

        string Tooltip { get; set; }

        int Angle { get; set; }

    }
    public interface ICheckValveWidget
    {
        string Tooltip { get; set; }
    }

}
