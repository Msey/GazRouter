using System;
using System.Collections.Generic;
using System.Windows;

namespace GazRouter.Flobus.Interfaces
{
    public interface IPipeline
    {
        Guid Id { get; }
        Point StartPoint { get; set; }
        Point EndPoint { get; set; }

        IEnumerable<IGeometryPoint> IntermediatePoints { get; }
        List<Segment> OverlaySegments { get; }
        IEnumerable<IValve> Valves { get; }
        IEnumerable<IDistrStation> DistributingStations { get; }
        IEnumerable<IPipelineOmElement> MeasuringLines { get; }
        IEnumerable<IPipelineOmElement> ReducingStations { get; }
        double KmBegining { get; }
        double KmEnd { get; }
        IEnumerable<IPipelineConnectionHint> PipelineConnections { get; }
        ICollection<PipelineMarkup> Markups { get;  }

        IGeometryPoint AddPoint(double km, Point point);
        void RemovePoint(double km);
        void RemoveMeasuringLine(Guid id);
    }

    public interface IPipelineConnectionHint : IPipelineElement
    {
        Guid DestinationPipileneId { get; }
        bool ConnectToStart { get; }
    }

    public interface IGeometryPoint
    {
        Point Position { get; set; }

        double Km { get; set; }
    }

    public interface IDistributingStation
    {
        Guid Id { get; }

        object Data { get; set; }
    }
}