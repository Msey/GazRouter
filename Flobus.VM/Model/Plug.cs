using GazRouter.Common.ViewModel;
using GazRouter.Flobus.Interfaces;
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

namespace GazRouter.Flobus.VM.Model
{
    public class Plug : PropertyChangedBase, IPipelineElement
    {
        private Point _position;
        private double _km;

        public Plug(double km, Point point)
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
