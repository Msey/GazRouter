using System;
using System.ComponentModel;
using System.Windows;
using GazRouter.Flobus.EventArgs;

namespace GazRouter.Flobus.Model
{
    public enum PointType
    {
        Undefined = 0,
        First = 1, // начальная точка
        Last = 2, // конечная точка
        Intermediate = 3, // промежуточная,
        Infra, //краны, прг и т.д
        Turn, // поворот
    }

    public interface IPipelinePoint : INotifyPropertyChanged
    {
        event EventHandler<PositionChangedEventArgs> PositionChanged;

        PointType Type { get; } 

        /// <summary>
        /// Координаты точки излома на схеме (определяют положение газопровда - полилинии)
        /// </summary>
        Point Position { get; set; }

        double Km { get; set; }

        Point Align(Point transformedPoint);
    }

}