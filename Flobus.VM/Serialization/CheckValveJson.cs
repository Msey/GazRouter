using GazRouter.Flobus.VM.Model;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GazRouter.Flobus.VM.Serialization
{
    public class CheckValveJson
    {
        public CheckValveJson()
        {
        }
        public CheckValveJson(CheckValve cv) : this()
        {
            Position = cv.Position;
            Tooltip = cv.Tooltip;
            Angle = cv.Angle;
        }
        
        public Point Position { get; set; }
        public string Tooltip { get; set; }
        public int Angle { get; set; }
    }

}
