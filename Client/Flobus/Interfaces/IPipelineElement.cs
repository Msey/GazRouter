using System;
using System.Windows;

namespace GazRouter.Flobus.Interfaces
{
    /// <summary>
    /// Интрерфейс элементов, которые располагается на газопроводе краны, ГРС и т.д.
    /// </summary>
    public interface IPipelineElement
    {
        double Km { get;  }
    }

    public interface IPipelineOmElement : IPipelineElement
    {
        Guid Id { get; }
        Point ContainerPosition { get; set; }
    }

    public interface IDistrStation :  IPipelineOmElement
    {
        object Data { get; set; }
    }

    public interface IValve : IPipelineOmElement
    {
        int TextAngle { get; set; }
        string Tooltip { get;}
    }
}