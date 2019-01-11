using System;
using System.Collections.Generic;
using System.Windows;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.VM.Model;

namespace ClientTests.Flobus.Services
{
    public class PipelineStub : IPipeline
    {
        public Guid Id { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public IEnumerable<IGeometryPoint> IntermediatePoints { get; }
        public IEnumerable<IValve> Valves { get; set; }
        public IEnumerable<IDistrStation> DistributingStations { get; }
        public IEnumerable<IPipelineOmElement> MeasuringLines { get; }
        public List<Segment> OverlaySegments { get; }
        public IEnumerable<IPipelineOmElement> ReducingStations { get; }
        public double KmBegining { get; set; }
        public double KmEnd { get; set; }
        public IEnumerable<IPipelineConnectionHint> PipelineConnections { get; }
        public ICollection<PipelineMarkup> Markups { get; }

        public IGeometryPoint AddPoint(double km, Point point)
        {
            return new GeometryPoint(km,point);
        }

        public void RemovePoint(double km)
        {
            throw new NotImplementedException();
        }
        
        public void RemoveMeasuringLine(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}