using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GazRouter.Flobus.VM.Model;

namespace GazRouter.Flobus.VM.Serialization
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PipelineJson
    {
        public PipelineJson()
        {
        }
        
        public PipelineJson(Pipeline pipeline)
        {
            Id = pipeline.Id;
            StartPoint = pipeline.StartPoint;
            EndPoint = pipeline.EndPoint;

            IntermediatePoints = pipeline.IntermediatePoints.Select(p => new PointJson(p.Position, p.Km)).ToList();
            Valves = pipeline.Valves.Where(v => v.TextAngle != 0).Select(v => new ValveJson(v.Id, v.TextAngle)).ToList();
            DistributingStations =
                pipeline.DistributingStations
                    .Select(ds => new DistributingStationJson(ds.Id, ds.TextAngle,ds.ContainerPosition, ds.Hidden))
                    .ToList();
            MeasuringLines = pipeline.MeasuringLines
                    .Select(e => new MeasuringLineJson(e.Id, e.TextAngle, e.ContainerPosition, e.Hidden))
                    .ToList();
            ReducingStations = pipeline.ReducingStations
                .Select(e => new ReducingStationJson(e.Id, e.TextAngle, e.ContainerPosition, e.Hidden))
                .ToList();

            Color = pipeline.Color;
            Thickness = pipeline.Thickness;
        }

        public List<ReducingStationJson> ReducingStations { get; set; } = new List<ReducingStationJson>();

        public List<MeasuringLineJson> MeasuringLines { get; set; } = new List<MeasuringLineJson>();

        public List<DistributingStationJson> DistributingStations { get; set; } = new List<DistributingStationJson>();

        public List<ValveJson> Valves { get; set; } = new List<ValveJson>();

        public List<UiEntities.FloModel.PipelineDiameterSegment> DiameterSegments { get; set; } = new List<UiEntities.FloModel.PipelineDiameterSegment>();

        public Guid Id { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public List<PointJson> IntermediatePoints { get; set; }
        public Color Color { get; set; }
        public double Thickness { get; set; }
    }
}